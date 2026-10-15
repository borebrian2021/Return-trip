"use strict";
(function ($) {
    $(document).ready(function () {
        var currentSelect;

        $("select:not(.hide, .filter-select):not(.wc-blocks-components-select select)").each(function () {
            var selectClass = $(this).attr('class');
            var selectElement = $(this);

            if (selectClass && selectClass.includes('add_a_car-select') && typeof allowDealerAddCategory != 'undefined' && allowDealerAddCategory == 1) {
                selectElement.select2({
                    width: '100%',
                    dropdownParent: $('body'),
                    matcher: matchCustom,
                    minimumResultsForSearch: Infinity,
                    "language": {
                        "noResults": function(){
                            return noFoundSelect2;
                        }
                    }
                });
            } else {
                selectElement.select2({
                    width: '100%',
                    dropdownParent: $('body'),
                    minimumResultsForSearch: Infinity,
                    "language": {
                        "noResults": function(){
                            return noFoundSelect2;
                        }
                    }
                });
            }
        });

        $("select:not(.hide):not(.wc-blocks-components-select select)").on("select2:open", function() {
            var stmClass = $(this).data('class');
            stmClass = (typeof stmClass == 'undefined') ? $(this).attr('name') : stmClass;

            currentSelect = $(this);

            var parentContainer = $(this).closest('[data-elementor-widget-class]');
            var widgetClass = parentContainer.data('elementor-widget-class');

            if (widgetClass) {
                $('.select2-dropdown--below .select2-results').addClass(widgetClass);
                $('.select2-dropdown--below').addClass(widgetClass)
            }

            $('.select2-dropdown--below').parent().addClass(stmClass);

            window.scrollTo(0, $(window).scrollTop() + 1);
            window.scrollTo(0, $(window).scrollTop() - 1);
        });

        $("select:not(.hide):not(.wc-blocks-components-select select)").on("select2:closing", function() {
            $('.select2-search--dropdown').removeClass('plus-added-emeht-mts');
            $('.add-new-term').remove();

            var parentContainer = $(this).closest('[data-elementor-widget-class]');
            var widgetClass = parentContainer.data('elementor-widget-class');
            if (widgetClass) {
                $('.select2-dropdown--below .select2-results').removeClass(widgetClass);
            }
        });

        $('.single-product .product-type-variable table.variations select').on("change", function() {
            $(this).parent().find('.select2-selection__rendered').text($(this).find('option[value="'+ $(this).val() +'"]').text());
        });

        $("select[name='stm-multi-currency']").on("select2:select", function () {
            var currency = $(this).val();

            $.cookie('stm_current_currency', currency, { expires: 7, path: '/' });
            var data = $(this).select2('data');
            var selectedText = $(this).attr("data-translate").replace("%s", data[0].text);

            $(".stm-multiple-currency-wrap").find("span.select2-selection__rendered").text(selectedText);
            location.reload();
        });
    });
    
    function matchCustom(params, data) {
        var empty = false;
        if ($.trim(params.term) === '') {
            return data;
        }

        if (typeof data.text === 'undefined') {
            return null;
        }

        var searchTerm = params.term.toLowerCase();
        var optionText = data.text.toLowerCase();

        if (optionText.includes(searchTerm)) {
            var modifiedData = $.extend({}, data, true);

            $('.select2-search--dropdown').removeClass('plus-added-emeht-mts');
            $('.add-new-term').remove();

            return modifiedData;
        }

        if (optionText.indexOf(searchTerm) == -1) {
            if (!$('.select2-search--dropdown').hasClass('plus-added-emeht-mts')) {
                $('.select2-search--dropdown').append('<i class="fas fa-plus-square add-new-term"></i>');
            }
            $('.select2-search--dropdown').addClass('plus-added-emeht-mts');
        }

        // Return `null` if the term should not be displayed
        return null;
    }
})(jQuery);