"use strict";
(function ($) {
    $(document).ready(function () {
        $.datetimepicker.setLocale(currentLocale);

        // Skip elements inside .mvl-form - Forms Editor handles them
        $('.stm-date-timepicker').not('.mvl-form .stm-date-timepicker').datetimepicker({minDate: 0, lang: stm_lang_code});

        $('.stm-years-datepicker').datetimepicker({
            timepicker: false,
            format: 'd/m/Y',
            lang: stm_lang_code,
            closeOnDateSelect: true
        });
    });
})(jQuery);
