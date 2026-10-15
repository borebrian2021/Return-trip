if (typeof (STMListings) == 'undefined') {
    var STMListings = {};

    STMListings.$extend = function (object, methods) {
        methods.prototype = jQuery.extend({}, object.prototype);
        object.prototype = methods;
    };
}

function stm_test_drive_car_title(id, title) {
    var $ = jQuery;

    $('.test-drive-car-name').text(title);
    $('input[name=vehicle_id]').val(id);
    $('input[name=vehicle_name]').val(title);
    $('.modal-body-fields').removeClass('hidden');
    $('#request-test-drive-form').find('.alert-modal').remove();
    $('#request-test-drive-form').find('.form-error').removeClass('form-error');
}
(function ($) {
    "use strict";
    var timer;

    STMListings.resetFields = function () {
        $(document).on('reset', 'select', function (e) {
            $(this).val('');
            $(this).find('option').prop('disabled', false);
        });
    };

    STMListings.save_user_settings_success = function (data) {
        $(this).find('.stm-listing-loader').removeClass('visible');
        $('.stm-user-message').text(data.error_msg);

        $('.stm-image-avatar img').attr('src', data.new_avatar);

        if (data.new_avatar === '') {
            $('.stm-image-avatar').removeClass('hide-empty').addClass('hide-photo');
        } else {
            $('.stm-image-avatar').addClass('hide-empty').removeClass('hide-photo');
        }

    };

    STMListings.save_user_settings = function () {
        $('#stm_user_settings_edit').on('submit', function (e) {

            var formData = new FormData();

            /*Add image*/
            var avatarInput = $('input[name="stm-avatar"]')[0];
            if (avatarInput && avatarInput.files && avatarInput.files[0]) {
                formData.append('stm-avatar', avatarInput.files[0]);
            }

            /*Add file upload fields from form editor*/
            $(this).find('input[type="file"]').each(function() {
                var $input = $(this);
                var inputName = $input.attr('name');
                
                // Skip avatar field (already handled)
                if (inputName === 'stm-avatar') {
                    return;
                }
                
                // Handle multiple files
                if (this.files && this.files.length > 0) {
                    for (var i = 0; i < this.files.length; i++) {
                        formData.append(inputName, this.files[i]);
                    }
                }
            });

            /*Add text fields*/
            var formInputs = $(this).serializeArray();

            for (var key in formInputs) {
                if (formInputs.hasOwnProperty(key)) {
                    formData.append(formInputs[key]['name'], formInputs[key]['value']);
                }
            }

            formData.append('action', 'stm_listings_ajax_save_user_data');
            formData.append('security', stm_listings_user_data_nonce);

            e.preventDefault();

            $.ajax({
                type: "POST",
                url: ajaxurl,
                dataType: 'json',
                context: this,
                data: formData,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('.stm-user-message').empty();
                    $(this).find('.stm-listing-loader').addClass('visible');
                },
                success: STMListings.save_user_settings_success
            });
        })
    };

    // STMListings.become_dealer_file_input = function () {
    //     $('body').on('change', 'input[name="stm-avatar"]', function () {
    //         var length = $(this)[0].files.length;
    //         if (length == 1) {
    //             $('.stm-new-file-label').text($(this).val());
    //         } else {
    //             $('.stm-new-file-label').text('No File Chosen');
    //         }
    //     });
    // };

    STMListings.stm_logout = function () {
        $('body').on('click', '.stm_logout a', function (e) {
            e.preventDefault();
            $.ajax({
                url: ajaxurl,
                type: "POST",
                dataType: 'json',
                context: this,
                data: {
                    'action': 'stm_logout_user',
                    'security': stm_logout_user_nonce
                },
                beforeSend: function () {
                    $('.stm_add_car_form .stm-form-checking-user .stm-form-inner').addClass('activated');
                },
                success: function (data) {
                    if (data.exit) {
                        $('#stm_user_info').slideUp('fast', function () {
                            $(this).empty();
                            $('.stm-not-enabled, .stm-not-disabled').slideDown('fast');
                            $("html, body").animate({ scrollTop: $('.stm-form-checking-user').offset().top }, "slow");
                        });

                        $('.stm-form-checking-user button[type="submit"]').removeClass('enabled').addClass('disabled');
                        window.location.reload();
                    }
                    $('.stm_add_car_form .stm-form-checking-user .stm-form-inner').removeClass('activated');
                }
            });
        })
    };

    STMListings.stm_ajax_registration = function () {
        if (0 === $(".stm-register-form form").length) {
            return;
        }
        $(".stm-register-form form").on('submit', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: ajaxurl,
                dataType: 'json',
                context: this,
                data: $(this).serialize() + '&action=stm_custom_register&security=' + stm_custom_register_nonce,
                beforeSend: function () {
                    $(this).find('input').removeClass('form-error');
                    $(this).find('.stm-listing-loader').addClass('visible');
                    $('.stm-validation-message').empty();
                },
                success: function (data) {
                    if (data.user_html) {
                        $('.add-car-btns-wrap').remove();
                        var $user_html = $(data.user_html).appendTo('#stm_user_info');
                        $('.stm-not-disabled, .stm-not-enabled').slideUp('fast', function () {
                            $('#stm_user_info').slideDown('fast');
                        });
                        $("html, body").animate({ scrollTop: $('.stm-form-checking-user').offset().top }, "slow");

                        $('.stm-form-checking-user button[type="submit"]').removeClass('disabled').addClass('enabled');

                        // insert plans select
                        if (data.plans_select && $('#user_plans_select_wrap').length > 0) {
                            $('#user_plans_select_wrap').html(data.plans_select);
                            $('#user_plans_select_wrap select').select2();
                        }
                    }

                    if (data.restricted && data.restricted) {
                        $('.btn-add-edit').remove();
                    }

                    $(this).find('.stm-listing-loader').removeClass('visible');
                    for (var err in data.errors) {
                        $(this).find('input[name=' + err + ']').addClass('form-error');
                    }

                    if (data.redirect_url) {
                        window.location = data.redirect_url;
                    }

                    if (data.message) {
                        var message = $('<div class="stm-message-ajax-validation heading-font">' + data.message + '</div>').hide();

                        $(this).find('.stm-validation-message').append(message);
                        message.slideDown('fast');
                    }
                }
            });
        });
    };

    STMListings.initVideoIFrame = function () {
        $('.light_gallery_iframe').lightGallery({
            selector: 'this',
            iframeMaxWidth: '70%'
        });
    };

    /**
     * checks form slider values
     * @param currentForm
     * @returns array - from form elements
     */
    STMListings.prepare_filter_params = function (currentForm) {
        let search_radius;
        let range = $(".stm-search_radius-range");

        if (range.length && range.slider("instance") !== undefined) {
            search_radius = range.slider("option", "max");
        }

        let data = currentForm.serializeArray();

        data = data.filter(function (field) {
            let value = parseInt(field.value);

            if ('max_search_radius' === field.name && (value > parseInt(search_radius) || isNaN(value))) {
                return
            }

            if ((['stm_lat', 'stm_lng'].includes(field.name) && 0 === value)) {
                return
            }

            return field.value;
        });

        return data;
    }

    /** remove form field with empty value, slider value if range wasn't changed **/
    STMListings.on_submit_filter_form = function () {
        $('form.search-filter-form.v8-inventory-form').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            var formActiveFields = STMListings.prepare_filter_params(form);
            var formActiveFieldNames = formActiveFields.map(function (value) { return value.name; });
            $.each($(this).serializeArray(), function (k, field) {
                if (false === formActiveFieldNames.includes(field.name)) {
                    form.find('[name="' + field.name + '"]').val('');
                }
            });
        })
    }

    /** init Select2 for filter select , on other select2 inits was added exception for .filter-select **/
    STMListings.init_select = function () {
        $("select.filter-select").each(function () {
            let selectElement = $(this),
                selectClass = selectElement.attr('class');

            let proDropdown = selectElement.hasClass('stm-pro-filter-select');
            let parent = $('body');
            if (selectElement.hasClass('stm-pro-filter-select')) {
                parent = $('#' + selectElement.attr('data-dropdown'));
            }
            selectElement.select2({
                width: '100%',
                dropdownParent: parent,
                minimumResultsForSearch: 0,
                containerCssClass: 'filter-select',
                closeOnSelect: true,
                dropdownCssClass: selectClass + ' ' + selectElement.closest('.filter-sidebar').attr('class'),
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

                    let $wrapper = $('<span class="stm-filter-pro-item-content"></span>');

                    if (image) {
                        $wrapper.append($('<img src="' + image + '" class="select2-option-image" />'));
                    }
                    $wrapper.append($('<span class="select2-option-text">' + data.text + '</span>'));

                    if (count !== undefined) {
                        if (count == 0) {
                            $wrapper.addClass('disabled');
                        }
                        $wrapper.append($('<span class="option-count">(' + count + ')</span>'));
                    }

                    return $wrapper;
                },
                templateSelection: function (data) {
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
                if (proDropdown) {
                    $('.select2-search__field').attr('placeholder', 'Search');
                }
            });

            selectElement.find('option').each(function () {
                let count = $(this).data('option-count');
                if (count !== undefined && count == 0) {
                    $(this).prop('disabled', true);
                }
            });
        });

        /** Not open multiple select if unselected value **/
        $("select.stm-multiple-select").on("select2:unselecting", event => {
            event.params.args.originalEvent.stopPropagation();
        });
    }

    /** Will remove earlier choosen child value if parent was changed **/
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
                child_select.val('');
            }
        }
    }

STMListings.ajaxGetCarPrice = function () {
        // Если FormsEditor вариант присутствует, не вешаем легаси обработчик
        if ($('.mvl-fe-form-request-car-price').length) {
            return;
        }
        $('#get-car-price form').on("submit", function (event) {
            event.preventDefault();
            $.ajax({
                url: ajaxurl,
                type: "POST",
                dataType: 'json',
                context: this,
                data: $(this).serialize() + '&action=stm_ajax_get_car_price&security=' + stm_car_price_nonce,
                beforeSend: function () {
                    $('.alert-modal').remove();
                    $(this).closest('form').find('input').removeClass('form-error');
                    $(this).closest('form').find('.stm-ajax-loader').addClass('loading');
                },
                success: function (data) {
                    $(this).closest('form').find('.stm-ajax-loader').removeClass('loading');
                    $(this).closest('form').find('.modal-body').append('<div class="alert-modal alert alert-' + data.status + ' text-left">' + data.response + '</div>')
                    for (var key in data.errors) {
                        $('#get-car-price input[name="' + key + '"]').addClass('form-error');
                    }
                }
            });
            $(this).closest('form').find('.form-error').on('hover', function () {
                $(this).removeClass('form-error');
            });
        });
    };

    var Favorites = STMListings.Favorites = function () {
        $('body.logged-in').on('click', '.stm-listing-favorite, .stm-listing-favorite-action', this.clickUser);
        $('body.stm-user-not-logged-in').on('click', '.stm-listing-favorite, .stm-listing-favorite-action', this.clickGuest);

        this.ids = $.cookie('stm_car_favourites');

        if (this.ids) {
            this.ids = this.ids.split(',');
        } else {
            this.ids = [];
        }

        var _this = this;
        if ($('body').hasClass('logged-in')) {
            $.getJSON(ajaxurl, { action: 'stm_ajax_get_favourites', security: stm_security_nonce }, function (data) {
                _this.ids = data;
                _this.activateLinks();
            });
        } else {
            this.activateLinks();
        }
    };

    Favorites.prototype.clickUser = function (e) {
        e.preventDefault();

        if ($(this).hasClass('disabled')) {
            return false;
        }

        var $button = $(this);
        $button.tooltip('hide');

        $button.toggleClass('active');
        var stm_car_add_to = $button.data('id');

        $.ajax({
            url: ajaxurl,
            type: "POST",
            dataType: 'json',
            data: '&car_id=' + stm_car_add_to + '&action=stm_ajax_add_to_favourites&security=' + stm_security_nonce,
            context: this,
            beforeSend: function (data) {
                $button.addClass('disabled');
            },
            success: function (data) {
                if (data.count !== undefined) {
                    $('.stm-my-favourites span').text(data.count);
                }
                $button.removeClass('disabled');
                updateTooltip($button);
            }
        });
    };

    Favorites.prototype.clickGuest = function (e) {
        e.preventDefault();

        var $button = $(this);

        $button.toggleClass('active');

        $button.tooltip('hide');

        var stm_cookies = $.cookie();
        var stm_car_add_to = $button.data('id');
        var stm_car_favourites = [];

        if (typeof (stm_cookies['stm_car_favourites']) !== 'undefined') {
            stm_car_favourites = stm_cookies['stm_car_favourites'].split(',');
            var index = stm_car_favourites.indexOf(stm_car_add_to.toString());
            if (index !== -1) {
                stm_car_favourites.splice(index, 1);
            } else {
                stm_car_favourites.push(stm_car_add_to.toString());
            }

            stm_car_favourites = stm_car_favourites.join(',');
            $.cookie('stm_car_favourites', stm_car_favourites, { expires: 7, path: '/' });

        } else {
            $.cookie('stm_car_favourites', stm_car_add_to.toString(), { expires: 7, path: '/' });
        }

        updateTooltip($button);
    };

    Favorites.prototype.activateLinks = function (ctx) {
        $.each(this.ids, function (key, value) {
            if (!value) {
                return;
            }

            $('.stm-listing-favorite, .stm-listing-favorite-action', ctx)
                .filter('[data-id=' + value + ']')
                .addClass('active')
                .tooltip('hide')
                .attr('title', stm_i18n.remove_from_favorites)
                .tooltip({ placement: 'auto' });

            $('.stm-listing-favorite.active')
                .tooltip('destroy')
                .tooltip();

            $('.stm-user-private-main').find('.stm-listing-favorite.active')
                .tooltip('destroy')
                .tooltip()
                .tooltip({ placement: 'auto' });
        });
    };

    function updateTooltip($button) {
        if ($button.hasClass('active')) {
            $button.attr('data-original-title', stm_i18n.remove_from_favorites);
            $button.attr('title', stm_i18n.remove_from_favorites);
        } else {
            $button.attr('data-original-title', stm_i18n.add_to_favorites);
            $button.attr('title', stm_i18n.add_to_favorites);
        }
    }

    STMListings.initTooltips = function () {
        $('[data-toggle="tooltip"]').each(function () {
            var $this = $(this);
            var placement = $this.data('placement') || 'auto';

            $this.tooltip({
                trigger: 'hover',
                placement: placement,
                html: true,
                container: 'body'
            });
        });
    };

    $(document).ready(function () {
        if (typeof elementorFrontend !== "undefined" && typeof elementorFrontend.hooks !== "undefined") {
            elementorFrontend.hooks.addAction('frontend/element_ready/widget', function ($scope) {
                if ($scope.find('select.filter-select').length) {
                    STMListings.init_select();
                }
            });
        } else {
            STMListings.init_select();
        }

        STMListings.save_user_settings();
        STMListings.stm_logout();
        STMListings.resetFields();
        STMListings.stm_ajax_registration();
        STMListings.ajaxGetCarPrice();
        STMListings.on_submit_filter_form();
        STMListings.initTooltips();
        // STMListings.become_dealer_file_input();

        window.stm_favourites = new Favorites();

        $(document).on('change', '.stm-sort-by-options select', function () {
            var $form = $('.search-filter-form');
            if ($form.length) {
                $form.find('input[name="sort_order"]').val($(this).val());
                $form.trigger('submit');
            }
        });

        $(document).on('change', '.ajax-filter select, .stm-sort-by-options select, .stm-slider-filter-type-unit, .stm-filter-pro-options-list input[type="checkbox"]', function () {
            STMListings.clean_select_child_if_parent_changed($(this));
            if ($(this).closest('form').length) {
                $(this).closest('form').trigger('submit');
            } else {
                if (this.closest('.stm-select-sorting')) {
                    $('form[data-trigger="filter"] [name="sort_order"]').val(this.value);
                    requestAnimationFrame(function () {
                        $('form[data-trigger="filter"]').trigger('submit');
                    });
                }
            }
        });

        $(document).on('slidestop', '.ajax-filter .stm-filter-type-slider', function (event, ui) {
            $(this).closest('form').trigger('submit');
        });

        $(document).ajaxComplete(function () {
            STMListings.initTooltips();
        });

        $('.stm_login_me a').on('click', function (e) {
            e.preventDefault();
            $('.stm-add-a-car-login-overlay,.stm-add-a-car-login').toggleClass('visiblity');
        });

        $('.stm-add-a-car-login-overlay').on('click', function (e) {
            $('.stm-add-a-car-login-overlay,.stm-add-a-car-login').toggleClass('visiblity');
        });

        $('.stm-big-car-gallery').lightGallery({
            selector: '.stm_light_gallery',
            mode: 'lg-fade'
        });

        STMListings.initVideoIFrame();
    });

    jQuery(document).ready(function ($) {
        $('input[name="stm_accept_terms"]').on('click', function () {
            if ($(this).is(':checked')) {
                $(
                    '.stm-login-register-form .stm-register-form form input[type="submit"]'
                ).removeAttr('disabled')
            } else {
                $(
                    '.stm-login-register-form .stm-register-form form input[type="submit"]'
                ).attr('disabled', '1')
            }
        });
    });



    jQuery(document).ready(function ($) {

        $('.stm-show-password .fas, .stm-show-password .far ').mousedown(function () {
            $(this).closest('.stm-show-password').find('input').attr('type', 'text');
            $(this).addClass('fa-eye');
            $(this).removeClass('fa-eye-slash');
        });

        $(document).mouseup(function () {
            $('.stm-show-password').find('input').attr('type', 'password');
            $('.stm-show-password .fas').addClass('fa-eye-slash');
            $('.stm-show-password .fas').removeClass('fa-eye');
            $('.stm-show-password .far').addClass('fa-eye-slash');
            $('.stm-show-password .far').removeClass('fa-eye');
        });

        $("body").on('touchstart', '.stm-show-password .fas, .stm-show-password .far', function () {
            $(this).closest('.stm-show-password').find('input').attr('type', 'text');
            $(this).addClass('fa-eye');
            $(this).removeClass('fa-eye-slash');
        });

        $('body').on('click', '.stm-show-number, .single-listing-phone-show', function () {
            var parent = $(this).parent()
            var phone_owner_id = $(this).attr('data-id')
            var listing_id = $(this).attr('data-listing-id')

            if (typeof listing_id === 'undefined' || listing_id === false) {
                listing_id = '0'
            }

            parent.find('.stm-show-number').text('').addClass('load_number')
            $.ajax({
                url: ajaxurl,
                type: 'GET',
                dataType: 'json',
                context: this,
                data:
                    'phone_owner_id=' +
                    phone_owner_id +
                    '&listing_id=' +
                    listing_id +
                    '&action=stm_ajax_get_seller_phone&security=' +
                    stm_security_nonce,
                success: function (data) {
                    parent.find('.stm-show-number').hide()
                    parent
                        .find('.phone')
                        .html('')
                        .append(
                            $('<a>')
                                .attr('href', 'tel:' + data)
                                .text(data)
                        )
                        .html('')
                        .append(
                            $('<a>')
                                .attr('href', 'tel:' + data)
                                .text(data)
                        )
                    parent.find('.single-listing-phone-number').show();
                    parent.find('.single-listing-hidden-phone-number').hide();
                    parent.find('.single-listing-phone-show').hide();
                },
            })
        })

    });

    $('body').on(
        'click',
        '.listing-car-item-meta .price[data-target="#get-car-price"], .stm_moto_hover_unit .price [data-target="#get-car-price"], #listings-result .image .price [data-target="#get-car-price"]',
        function (e) {
            if ($(e.target).closest('a').length) {
                e.preventDefault()
            }
        }
    )

    $(document).ready(function () {
        function updateCarListing() {
            var selectedOption = $('.stm-select-sorting select').val()
            if (selectedOption === 'popular') {
                $('.user-listings-wrapper#popular').addClass('active')
                $('.user-listings-wrapper#recent').removeClass('active')
                $('.popular.user-listings-pagination').addClass('active')
                $('.recent.user-listings-pagination').removeClass('active')
            } else {
                $('.user-listings-wrapper#popular').removeClass('active')
                $('.user-listings-wrapper#recent').addClass('active')
                $('.popular.user-listings-pagination').removeClass('active')
                $('.recent.user-listings-pagination').addClass('active')
                $('.stm-select-sorting').removeClass('mvl-selected');
                $('.stm-select-sorting').addClass('mvl-selected');
                $('.stm-sort-by-options').find('i').addClass('mvl-selected');
            }
        }
        $('.stm-select-sorting select').on('change', function () {
            updateCarListing()
        })

    })

})(jQuery);

function stm_get_price_view(price, currency, currencyPos, priceDel) {
    return price;
}

function stmMotorsCaptcha() {
    jQuery('.g-recaptcha').each(function (index, el) {
        var $ = jQuery;
        grecaptcha.render(el, { 'sitekey': $(el).data('sitekey') });
    });
};

