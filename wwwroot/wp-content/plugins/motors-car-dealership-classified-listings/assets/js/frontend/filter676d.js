if (typeof (STMListings) == 'undefined') {
    var STMListings = {};
}

(function ($) {
    "use strict";

    function Filter(form) {
        this.form = form;
        this.ajax_action = ($(this.form).data('action')) ? $(this.form).data('action') : 'listings-result';
        this.init();
    }

    Filter.prototype.init = function () {
        $(this.form).on("submit", $.proxy(this.submit, this));
        this.getTarget().on('click', 'a.page-numbers', $.proxy(this.paginationClick, this));

        let badgeWrapper = $('.stm-filter-chosen-units');
        let listBadgesWrapper = $('.stm-filter-chosen-units-list')
        if (!window.matchMedia('(max-width: 768px)').matches) {
            if (listBadgesWrapper.find('li').length > 0) {
                badgeWrapper.show()
                $('.search-results-actions-result').show()
            } else {
                badgeWrapper.hide()
                $('.search-results-actions-result').hide()
            }
        } else {
            badgeWrapper.hide()
            $('.search-results-actions-result').hide()
        }

    };

    Filter.prototype.getFormParams = function () {
        let url = new URL($(this.form).attr('action'));
        let currentUrl = new URL(window.location.href);
        let viewType = currentUrl.searchParams.get('view_type');

        if (viewType) {
            url.searchParams.delete('view_type');
        }

        $.each(
            $(this.form).serializeArray(),
            function (i, field) {
                if (field.value !== '') {
                    if (['stm_lat', 'stm_lng'].includes(field.name)) {
                        if (field.value !== 0) {
                            url.searchParams.append(field.name, field.value);
                        }
                    } else {
                        url.searchParams.append(field.name, field.value);
                    }
                }
            }
        );

        if (viewType) {
            url.searchParams.append('view_type', viewType);
        }

        return url;
    };

    Filter.prototype.submit = function (event) {
        event.preventDefault();
        if (typeof stm_elementor_editor_mode === "undefined") {
            this.performAjax(this.getFormParams());
        }
    };

    Filter.prototype.paginationClick = function (event) {
        event.preventDefault();
        let url = new URL($(event.target).closest('a').attr('href'));

        url.searchParams.set('security', stm_security_nonce);
        this.performAjax(url.toString());
        $('html, body').animate({
            scrollTop: 0
        }, 500);
    };

    Filter.prototype.pushState = function (url) {
        window.history.pushState('', '', decodeURI(url));
    };

    Filter.prototype.performAjax = function (url) {
        var target = this.getTarget();
        var custom_img_size = target.data('custom-img-size');
        var listings_grid_view_skin = target.data('listings-grid-view-skin');
        var listings_list_view_skin = target.data('listings-list-view-skin');
        var posts_per_page = target.data('posts-per-page');
        var data = {
            ajax_action: this.ajax_action,
            custom_img_size: custom_img_size,
            listings_grid_view_skin: listings_grid_view_skin,
            listings_list_view_skin: listings_list_view_skin,
        };
        if (posts_per_page) {
            data.posts_per_page = posts_per_page;
        }
        $.ajax({
            url: url,
            dataType: 'json',
            context: this,
            type: 'POST',
            data: data,
            beforeSend: this.ajaxBefore,
            success: this.ajaxSuccess,
            complete: this.ajaxComplete,
        })
    }

    Filter.prototype.ajaxBefore = function () {
        this.getTarget().addClass('stm-loading');
    };

    Filter.prototype.ajaxSuccess = function (res) {
        if (res.total == 0 || (typeof items_per_page != 'undefined' && res.total <= items_per_page)) {
            $('.stm-inventory-items-per-page-wrap').hide();
        } else {
            $('.stm-inventory-items-per-page-wrap').show();
        }

        this.getTarget().html(res.html);
        this.disableOptions(res);
        if (res.url) {
            this.pushState(res.url);
        }

        //Update Filter Badges
        updateFilterBadges(res.filter_badges);
        //Update Total Founded
        updateTotalfounded(res.total);
    };

    Filter.prototype.ajaxComplete = function () {
        this.getTarget().removeClass('stm-loading');

        $('.car-title').each(function () {
            let $title = $(this),
                maxChar = parseInt($title.attr('data-max-char')),
                $labels = $title.find('.labels');

            if ($labels.length > 0 && ($(this).text().length > maxChar)) {
                let originalLabels = $labels.clone();
                $labels.remove();
                let originalText = $title.contents().filter(function () {
                    return this.nodeType === 3;
                }).text().trim();

                if (originalText.length > maxChar) {
                    var truncatedText = originalText.substr(0, maxChar) + '...';

                    $title.html('').append(originalLabels).append(truncatedText);
                } else {
                    $title.html(originalText).prepend($labels)
                }
            } else {
                if ($(this).attr('data-max-char') != 'undefined' && $(this).text().length > $(this).attr('data-max-char')) {
                    $(this).text($(this).text().trim().substr(0, $(this).attr('data-max-char')) + '...');
                }
            }
        });

        // hoverable interactive gallery preview swiper
        this.reInitSwipeEvents();
    };

    Filter.prototype.disableOptions = function (res) {
        if (typeof res.options != 'undefined') {
            $.each(res.options, function (key, options) {
                var selector = 'select[name=' + key + ']';

                if (typeof res.filters !== 'undefined' && typeof res.filters[key] === "object" && res.filters[key].is_multiple_select) {
                    selector = 'select[name="' + key + '[]"]';
                }

                $(selector + ' > option', this.form).each(function () {
                    var slug = $(this).val();
                    if (options.hasOwnProperty(slug)) {
                        $(this).prop('disabled', options[slug].disabled);
                        $(this).data('base-disabled', options[slug].disabled ? 1 : 0);
                    } else {
                        // When backend doesn't return a slug, reset to enabled.
                        $(this).prop('disabled', false);
                        $(this).data('base-disabled', 0);
                    }
                });
                $('input[name="' + key + '\[\]"], input[name="' + key + '"]', this.form).each(function () {
                    var slug = $(this).val();
                    if (options.hasOwnProperty(slug)) {
                        if (options[slug].disabled) {
                            $(this).closest('label').addClass('stashed');
                        } else {
                            $(this).closest('label').removeClass('stashed');
                        }
                    }
                });
            });
        }

        if (typeof STMListings.applyParentDependencies === 'function') {
            STMListings.applyParentDependencies(this.form);
        }
    };

    Filter.prototype.getTarget = function () {
        var target = $(this.form).data('target');
        if (!target) {
            target = '#listings-result';
        }
        return $(target);
    };

    Filter.prototype.reInitSwipeEvents = function () {
        if ($('.stm-hoverable-interactive-galleries .interactive-hoverable .hoverable-wrap').length > 0) {
            $('.stm-hoverable-interactive-galleries .interactive-hoverable .hoverable-wrap').each((index, el) => {
                let galleryPreviewSwiper = new SwipeEvent(el);

                galleryPreviewSwiper.onRight(function () {
                    let active_index = $(this.element).find('.hoverable-unit.active').index();
                    $(this.element).find('.hoverable-unit').removeClass('active');
                    $(this.element).siblings('.hoverable-indicators').find('.indicator.active').removeClass('active');
                    if (active_index === 0) {
                        $(this.element).find('.hoverable-unit:last-child').addClass('active');
                        $(this.element).siblings('.hoverable-indicators').find('.indicator:last-child').addClass('active');
                    } else {
                        $(this.element).find('.hoverable-unit').eq(active_index - 1).addClass('active');
                        $(this.element).siblings('.hoverable-indicators').find('.indicator').eq(active_index - 1).addClass('active');
                    }
                });

                galleryPreviewSwiper.onLeft(function () {
                    let active_index = $(this.element).find('.hoverable-unit.active').index();
                    let total_items = $(this.element).find('.hoverable-unit');
                    $(this.element).find('.hoverable-unit').removeClass('active');
                    $(this.element).siblings('.hoverable-indicators').find('.indicator.active').removeClass('active');
                    if (active_index === parseInt(total_items.length - 1)) {
                        $(this.element).find('.hoverable-unit:first-child').addClass('active');
                        $(this.element).siblings('.hoverable-indicators').find('.indicator:first-child').addClass('active');
                    } else {
                        $(this.element).find('.hoverable-unit').eq(active_index + 1).addClass('active');
                        $(this.element).siblings('.hoverable-indicators').find('.indicator').eq(active_index + 1).addClass('active');
                    }
                });

                galleryPreviewSwiper.run();
            });
        }
    };

    STMListings.Filter = Filter;

    $(function () {

        $('form[data-trigger=filter]').each(function () {
            $(this).data('Filter', new Filter(this));
        });

        $('.car-title').each(function () {
            var $title = $(this);
            var maxChar = parseInt($title.attr('data-max-char'));
            var $labels = $title.find('.labels');

            if ($labels.length > 0 && $(this).text().length > maxChar) {
                var originalLabels = $labels.clone();
                $labels.remove();
                var originalText = $title.contents().filter(function () {
                    return this.nodeType === 3;
                }).text().trim();

                if (originalText.length > maxChar) {
                    var truncatedText = originalText.substr(0, maxChar) + '...';
                    $title.html('').append(originalLabels).append(truncatedText);
                } else {
                    $title.html(originalText).prepend($labels)
                }
            } else {
                if ($(this).attr('data-max-char') != 'undefined' && $(this).text().length > $(this).attr('data-max-char')) {
                    $(this).text($(this).text().trim().substr(0, $(this).attr('data-max-char')) + '...');
                }
            }
        });
    });

    // swipe events using vanilla js
    var SwipeEvent = (function () {
        function SwipeEvent(element) {
            this.xDown = null;
            this.yDown = null;
            this.element = typeof (element) === 'string' ? document.querySelector(element) : element;
            this.element.addEventListener('touchstart', function (evt) {
                this.xDown = evt.touches[0].clientX;
                this.yDown = evt.touches[0].clientY;
            }.bind(this), false);
        }

        SwipeEvent.prototype.onLeft = function (callback) {
            this.onLeft = callback;
            return this;
        };
        SwipeEvent.prototype.onRight = function (callback) {
            this.onRight = callback;
            return this;
        };
        SwipeEvent.prototype.onUp = function (callback) {
            this.onUp = callback;
            return this;
        };
        SwipeEvent.prototype.onDown = function (callback) {
            this.onDown = callback;
            return this;
        };

        SwipeEvent.prototype.handleTouchMove = function (evt) {
            if (!this.xDown || !this.yDown) {
                return;
            }
            var xUp = evt.touches[0].clientX;
            var yUp = evt.touches[0].clientY;
            this.xDiff = this.xDown - xUp;
            this.yDiff = this.yDown - yUp;

            if (Math.abs(this.xDiff) !== 0) {
                if (this.xDiff > 2) {
                    typeof (this.onLeft) === "function" && this.onLeft();
                } else if (this.xDiff < -2) {
                    typeof (this.onRight) === "function" && this.onRight();
                }
            }

            if (Math.abs(this.yDiff) !== 0) {
                if (this.yDiff > 2) {
                    typeof (this.onUp) === "function" && this.onUp();
                } else if (this.yDiff < -2) {
                    typeof (this.onDown) === "function" && this.onDown();
                }
            }
            // Reset values.
            this.xDown = null;
            this.yDown = null;
        };

        SwipeEvent.prototype.run = function () {
            this.element.addEventListener('touchmove', function (evt) {
                this.handleTouchMove(evt);
            }.bind(this), false);
        };

        return SwipeEvent;
    }());

    STMListings.clean_select_child_if_parent_changed = function (changed_item) {
        let list = $('#stm_parent_slug_list');
        if (0 === list.length) {
            return;
        }
        let stm_parent_slug_list = list.attr('data-value');
        let name = changed_item.attr('name');
        if ($(changed_item).length && name && name.length > 0) {
            let name = changed_item.attr('name').replace(/[\[\]']+/g, '');

            if (stm_parent_slug_list && stm_parent_slug_list.split(',').includes(name)) {
                var child_select = $('.filter-select option[data-parent="' + name + '"]').parent();
                child_select.each(function () {
                    var $select = $(this);
                    if ($select.prop('multiple')) {
                        $select.val(null);
                    } else {
                        $select.val('');
                    }
                    $select.trigger('change');
                });
            }
        }
    }

    STMListings.applyParentDependencies = function (form) {
        var $forms = form ? $(form) : $('form[data-trigger=filter]');
        $forms.each(function () {
            var $form = $(this);
            $form.find('select').each(function () {
                var $select = $(this);
                var $firstOption = $select.find('option[data-parent]').first();
                var parentSlug = $firstOption.data('parent');

                if (!parentSlug) {
                    return;
                }

                var $parent = $form.find('select[name="' + parentSlug + '"], select[name="' + parentSlug + '[]"]');
                if (!$parent.length) {
                    return;
                }

                var parentValues = $parent.val();
                if (parentValues && !Array.isArray(parentValues)) {
                    parentValues = [parentValues];
                }
                parentValues = parentValues ? parentValues.filter(Boolean) : [];

                $select.find('option').each(function () {
                    var $option = $(this);
                    var baseDisabled = $option.data('base-disabled');
                    if (typeof baseDisabled === 'undefined') {
                        baseDisabled = $option.prop('disabled');
                        $option.data('base-disabled', baseDisabled ? 1 : 0);
                    } else {
                        baseDisabled = Boolean(baseDisabled);
                    }

                    var deps = ($option.data('deps') || '').toString().split(',').filter(Boolean);
                    var shouldDisable = baseDisabled;

                    if (parentValues.length && deps.length > 0) {
                        shouldDisable = baseDisabled || !deps.some(function (dep) {
                            return parentValues.indexOf(dep) !== -1;
                        });
                    }

                    $option.prop('disabled', shouldDisable);
                });
            });
        });
    };

    STMListings.stm_disable_rest_filters = function ($_this, action, tabs_filter) {
        var $_form = $_this.closest('form');

        var data = [],
            url = $_form.attr('action'),
            sign = url.indexOf('?') < 0 ? '?' : '&';

        $.each(
            $_form.serializeArray(),
            function (i, field) {
                if (field.value != '') {
                    data.push(field.name + '=' + field.value)
                }
            }
        );

        url = url + sign + data.join('&');

        $.ajax(
            {
                url: url,
                dataType: 'json',
                context: this,
                data: '&ajax_action=' + action + '&security=' + stm_security_nonce,
                beforeSend: function () {
                    if (action == 'listings-items') {
                        $('.stm-ajax-row').addClass('stm-loading');
                    } else {
                        $('.classic-filter-row .filter-sidebar .select2-container--default .select2-selection--single .select2-selection__arrow b').addClass('stm-preloader');
                        $('.mobile-search-filter .filter-sidebar .select2-container--default .select2-selection--single .select2-selection__arrow b').addClass('stm-preloader');
                        $('.mobile-search-filter .filter-sidebar .select2-container--default .selection').addClass('stm-overlay');
                        $('.search-filter-form .filter-sidebar .select2-container--default .selection').addClass('stm-overlay');
                        $('.stm-listing-directory-total-matches').hide();
                    }
                },
                success: function (res) {
                    if (action == 'listings-items') {
                        $('.stm-ajax-row').removeClass('stm-loading');
                        $('#listings-result').html(res.html);
                        if (res.total == 0 || (typeof items_per_page != 'undefined' && res.total <= items_per_page)) {
                            $('.stm-inventory-items-per-page-wrap').hide();
                        }
                        $("img.lazy").lazyload();
                        $('.stm-tooltip-link, div[data-toggle="tooltip"]').tooltip();
                        window.history.pushState('', '', decodeURI(url));
                    } else {
                        /*Remove select preloaders*/
                        $('.classic-filter-row .filter-sidebar .select2-container--default .select2-selection--single .select2-selection__arrow b').removeClass('stm-preloader');
                        $('.classic-filter-row .filter-sidebar select').prop("disabled", false);
                        /*Disable options*/
                        if (typeof res.options != 'undefined') {
                            $.each(
                                res.options,
                                function (key, options) {
                                    $('select[name=' + key + '] > option', $_form).each(
                                        function () {
                                            var slug = $(this).val();
                                            if (options.hasOwnProperty(slug)) {
                                                $(this).prop('disabled', options[slug].disabled);
                                            }
                                        }
                                    );
                                }
                            );
                        }

                        if (!tabs_filter) {
                            $('select', $_form).select2('destroy');
                            $('select', $_form).each(function () {
                                let dropdownParent = $('body');
                                let proDropdown = false;
                                if ($(this).parent().next().hasClass('stm-pro-filter-dropdown-box')) {
                                    dropdownParent = $(this).parent().next();
                                    proDropdown = true;
                                }
                                $(this).select2({
                                    dropdownParent: dropdownParent,
                                    closeOnSelect: true,
                                    width: '100%',
                                    minimumResultsForSearch: 0,
                                    containerCssClass: 'filter-select',
                                    dropdownCssClass: $(this).attr('class'),
                                    "language": {
                                        "noResults": function () {
                                            return noFoundSelect2;
                                        }
                                    },
                                    matcher: function (params, data) {
                                        if (data.element && data.element.index === 0 && proDropdown === true) {
                                            return null;
                                        }
                                        if (!params.term) {
                                            return data;
                                        }
                                        if (data.text.toLowerCase().indexOf(params.term.toLowerCase()) > -1) {
                                            return data;
                                        }

                                        return null;
                                    },
                                    templateResult: function (data) {
                                        if (!data.id) {
                                            return data.text;
                                        }

                                        let $option = $(data.element);
                                        let count = $option.data('option-count');
                                        let image = $option.data('option-image');
                                        let isDisabled = $option.attr('data-disabled') === '1' || (count !== undefined && parseInt(count, 10) === 0);

                                        let $wrapper = $('<span class="stm-filter-pro-item-content"></span>');

                                        if (image) {
                                            $wrapper.append($('<img src="' + image + '" class="select2-option-image" />'));
                                        }

                                        $wrapper.append($('<span class="select2-option-text">' + data.text + '</span>'));

                                        if (isDisabled && proDropdown) {
                                            $wrapper.addClass('disabled');
                                        }
                                        if (count !== undefined) {
                                            $wrapper.append($('<span class="option-count">(' + count + ')</span>'));
                                        }

                                        return $wrapper;
                                    },
                                    templateSelection: function (data) {
                                        if (!data.id && proDropdown) {
                                            return null;
                                        }
                                        if (!data.id) {
                                            return data.text;
                                        }

                                        let $option = $(data.element);
                                        let count = $option.data('option-count');
                                        let image = $option.data('option-image');

                                        let $wrapper = $('<span class="stm-filter-pro-item-content"></span>');

                                        if (image) {
                                            $wrapper.append($('<img src="' + image + '" class="select2-option-image" />'));
                                        }

                                        $wrapper.append($('<span class="select2-option-text">' + data.text + '</span>'));

                                        if (count !== undefined && count != 0) {
                                            $wrapper.append($('<span class="option-count">(' + count + ')</span>'));
                                        }

                                        return $wrapper;
                                    }
                                }).on('select2:open', function () {
                                    $('.select2-search__field').attr('placeholder', stm_i18n.mvl_search_placeholder);
                                });
                            });
                        }

                        /*Change total*/
                        $('.stm-horizontal-filter-sidebar #stm-classic-filter-submit span').text(res.total);
                        $('.search-filter-form #show-car-btn-mobile span').text(res.total);
                        $('.mobile-search-filter #show-car-btn-mobile span').text(res.total);
                        $('.filter-listing.motors_dynamic_listing_filter .stm-filter-tab-selects .search-submit span').text(res.total);
                        $('.filter-listing.stm_dynamic_listing_filter .stm-filter-tab-selects .search-submit span').text(res.total);
                        $('.filter-listing.motors_dynamic_listing_filter .stm-filter-tab-selects .stm-search-count').text(res.total);
                        $('.filter-listing.stm_dynamic_listing_filter .stm-filter-tab-selects .stm-search-count').text(res.total);
                        $('.stm-inventory-pro-total-found').text(res.total);

                        $('.stm-listing-directory-total-matches').show();
                    }
                }
            }
        );
    };

    $(document).on(
        'change',
        '.archive-listing-page form input, .archive-listing-page form select, .stm-inventory-pro-filter form input, .stm-inventory-pro-filter form select',
        function () {
            if (typeof STMListings.clean_select_child_if_parent_changed === "function") {
                STMListings.clean_select_child_if_parent_changed($(this));
            }
            STMListings.stm_disable_rest_filters($(this), 'listings-binding');
        }
    );

    $('.filter-listing select:not(.hide)').select2().on(
        'select2:select',
        function (event) {
            STMListings.stm_disable_rest_filters($(this), 'listings-binding', true);
        }
    );

    $(document).on(
        'click',
        '.stm-ajax-checkbox-button .button, .stm-ajax-checkbox-instant .stm-option-label input',
        function (e) {

            if ($(this)[0].className == 'button') {
                e.preventDefault();
            }

            $(this).closest('form').trigger('submit');

        }
    );

    $(document).on('click', '.action-reset', function () {
        let _box = $(this).closest('.stm-filter-pro-item-heading').parent()
        _box
            .find('.stm-filter-pro-options-list label input[type="checkbox"]')
            .prop('checked', false)
        _box.find('.stm-filter-chosen-units li .stm-clear-listing-one-unit').click()
        $(this).closest('form').trigger('submit')
    })

    /*Remove badge*/
    $(document).on(
        'click',
        'ul.stm-filter-chosen-units-list li > i',
        function () {
            let $this = $(this),
                form = $('form[data-trigger=filter]').data('Filter')

            let stmType = $this.data('type')
            let stmSlug = $this.data('slug')

            $('input[name="' + stmSlug + '[]"]:checked').each(function () {
                let $input = $(this)
                $input.parent().removeClass('checked')
                $input.prop('checked', false)
                $input.closest('.stm-option-label').removeClass('checked')
                let heading = $input.closest('.stm-filter-item').find('.stm-filter-pro-item-heading')
                heading.removeClass('selected').find('.results-count').html('0')
            })

            if (stmType == 'select') {
                let $selectMulti = $('select[name="' + stmSlug + '[]"]');
                let $select = $selectMulti.length ? $selectMulti : $('select[name="' + stmSlug + '"]');
                $select.val(null);
                $select.find('option').prop('disabled', false);
                $select.find('option[value=""]').prop('selected', false);
                $select.trigger('change');
                $('select[name="' + stmSlug + '"]').val('');

                $select.select2('destroy');
                let dropdownParent = $('body');
                let closeOnSelect = true;
                let proDropdown = false;

                if ($select.parent().next().hasClass('stm-pro-filter-dropdown-box')) {
                    dropdownParent = $select.parent().next();
                    proDropdown = true;
                }

                $select.select2({
                    dropdownParent: dropdownParent,
                    closeOnSelect: closeOnSelect,
                    width: '100%',
                    minimumResultsForSearch: 0,
                    containerCssClass: 'filter-select',
                    dropdownCssClass: $select.attr('class'),
                    "language": {
                        "noResults": function () {
                            return noFoundSelect2;
                        }
                    },
                    matcher: function (params, data) {
                        if (data.element && data.element.index === 0 && proDropdown === true) {
                            return null;
                        }
                        if (!params.term) {
                            return data;
                        }
                        if (data.text.toLowerCase().indexOf(params.term.toLowerCase()) > -1) {
                            return data;
                        }
                        return null;
                    },
                    templateResult: function (data) {
                        if (!data.id) {
                            return data.text;
                        }
                        let $option = $(data.element);
                        let count = $option.data('option-count');
                        let image = $option.data('option-image');
                        let isDisabled = $option.attr('data-disabled') === '1' || (count !== undefined && parseInt(count, 10) === 0);

                        let $wrapper = $('<span class="stm-filter-pro-item-content"></span>');

                        if (image) {
                            $wrapper.append($('<img src="' + image + '" class="select2-option-image" />'));
                        }

                        $wrapper.append($('<span class="select2-option-text">' + data.text + '</span>'));

                        if (isDisabled && proDropdown) {
                            $wrapper.addClass('disabled');
                        }
                        if (count !== undefined) {
                            $wrapper.append($('<span class="option-count">(' + count + ')</span>'));
                        }
                        return $wrapper;
                    },
                    templateSelection: function (data) {
                        if (!data.id && proDropdown) {
                            return null;
                        }
                        if (!data.id) {
                            return data.text;
                        }
                        let $option = $(data.element);
                        let count = $option.data('option-count');
                        let image = $option.data('option-image');

                        let $wrapper = $('<span class="stm-filter-pro-item-content"></span>');

                        if (image) {
                            $wrapper.append($('<img src="' + image + '" class="select2-option-image" />'));
                        }

                        $wrapper.append($('<span class="select2-option-text">' + data.text + '</span>'));

                        if (count !== undefined && count != 0) {
                            $wrapper.append($('<span class="option-count">(' + count + ')</span>'));
                        }
                        return $wrapper;
                    }
                }).on('select2:open', function () {
                    $('.select2-search__field').attr('placeholder', stm_i18n.mvl_search_placeholder);
                });

                let heading = $select.closest('.stm-filter-item').find('.stm-filter-pro-item-heading');
                heading.removeClass('selected').find('.results-count').html('0');
            }

            if (stmType == 'number') {
                $('select[name="' + stmSlug + '[]"]').val(null)
                $('select[name="' + stmSlug + '[]"]').trigger('change')
                $('select[name="' + stmSlug + '"]').val('')
                let $select = $('select[name="' + stmSlug + '"]')
                $select
                    .find('option')
                    .prop('disabled', false)
                $select
                    .select2('destroy')
                    .select2()
                    .select2('val', '')

                $('input[name="min_' + stmSlug + '"]')
                    .val('')
                $('input[name="max_' + stmSlug + '"]')
                    .val('')

                $('select[name="min_' + stmSlug + '"]').find('option').prop('disabled', false)
                $('select[name="min_' + stmSlug + '"]').select2('destroy').select2().select2('val', '')

                let heading = $select.closest('.stm-filter-item').find('.stm-filter-pro-item-heading')
                heading.removeClass('selected').find('.results-count').html('0')

            }

            if (stmType == 'slider') {
                let sliderObj = $('.stm-' + stmSlug + '-range').slider('instance')
                if (typeof sliderObj !== 'undefined') {
                    $('.stm-' + stmSlug + '-range').slider('values', [
                        sliderObj.options.min,
                        sliderObj.options.max,
                    ])
                    $('input[name="min_' + stmSlug + '"]')
                        .val('')
                        .attr('placeholder', sliderObj.options.min)
                    $('input[name="max_' + stmSlug + '"]')
                        .val('')
                        .attr('placeholder', sliderObj.options.max)
                }
            }

            $(this).closest('li').hide()

            let form_url = form.getFormParams()
            var hasSearchParam = hasSearchParams(form_url)
            if (!hasSearchParam) {
                $('.stm-listing-directory-total-matches').hide()
            }
            form.performAjax(form_url)
        }

    )

    function hasSearchParams(url) {
        const urlObj = new URL(url)
        const searchParams = urlObj.searchParams

        for (let key of searchParams.keys()) {
            if (key !== 'security' && key !== 'ajax_action' && key !== 'posttype') {
                return true
            }
        }

        return false
    }

    function updateFilterBadges(filterBadges) {
        let badgeContainer = $('.stm-filter-chosen-units-list')
        badgeContainer.empty()

        if (filterBadges && Object.keys(filterBadges).length > 0) {

            $.each(filterBadges, function (slug, badge_info) {
                let badge = $(`
                <li>
                    <span class="stm-filter-chosen-units-list-name">${badge_info.name}: </span><span class="stm-filter-chosen-units-list-value">${badge_info.value}</span>
                    <i data-url="${badge_info.url}"
                       data-type="${badge_info.type}"
                       data-slug="${badge_info.slug}"
                       data-multiple="${badge_info.multiple ? badge_info.multiple : ''
                    }"
                       class="motors-icons-cross-ico stm-clear-listing-one-unit stm-clear-listing-one-unit-classic"></i>
                </li>
            `)

                badgeContainer.append(badge)
            })

            $('.stm-filter-chosen-units').show()
            $('.search-results-actions-result').show()
        } else {
            $('.search-results-actions-result').hide()
            $('.stm-filter-chosen-units').hide()
        }
    }

    function updateTotalfounded(total) {
        $('span.mvl-total-count').text(total)
    }

    $(document).on('click', '.mvl-reset-all', function () {
        if (typeof stm_i18n !== 'undefined' && stm_i18n.mvl_current_page_url) {
            let url = new URL(stm_i18n.mvl_current_page_url);
            window.location = url.toString();
        } else {
            let currentUrl = window.location.href.split('?')[0];
            let url = new URL(currentUrl);
            let params = new URLSearchParams(window.location.search);
            if (params.has('posttype')) {
                url.searchParams.set('posttype', params.get('posttype'));
            }

            window.location = url.toString();
        }
    })

    $(document).ready(function () {
        $('.stm-inventory-pro-filter-mobile-apparent .stm-filter-item-search-input input').on('input', function () {
            let searchValue = $(this).val();
            $('form.search-filter-form input[name="stm_keywords"]').val(searchValue).trigger('change');
        });

        $('.stm-inventory-pro-filter-mobile-apparent .stm-filter-item-search-input .motors-icons-mvl-search').on('click', function () {
            $(this).closest('form').trigger('submit');
        });

        $('.stm-inventory-pro-filter-mobile-apparent .stm-filter-item-search-input input').on('keypress', function (e) {
            if (e.which == 13) {
                e.preventDefault();
                $('form.search-filter-form').trigger('submit');
            }
        });
    });

})(jQuery);
