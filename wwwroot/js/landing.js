/* ---- inline <script id="wc-add-to-cart-js-extra"> ---- */
var wc_add_to_cart_params = {"ajax_url": "/motorcycles/wp-admin/admin-ajax.php", "wc_ajax_url": "/motorcycles/?wc-ajax=%%endpoint%%", "i18n_view_cart": "View cart", "cart_url": "https://motors.stylemixthemes.com/motorcycles/cart/", "is_cart": "", "cart_redirect_after_add": "no"};
//# sourceURL=wc-add-to-cart-js-extra

/* ---- inline <script id="woocommerce-js-extra"> ---- */
var woocommerce_params = {"ajax_url": "/motorcycles/wp-admin/admin-ajax.php", "wc_ajax_url": "/motorcycles/?wc-ajax=%%endpoint%%"};
//# sourceURL=woocommerce-js-extra

/* ---- inline <script id="bookly-globals-js-extra"> ---- */
var BooklyL10nGlobal = {"csrf_token": "9facfd6f9c", "ajax_url_backend": "https://motors.stylemixthemes.com/motorcycles/wp-admin/admin-ajax.php", "ajax_url_frontend": "https://motors.stylemixthemes.com/motorcycles/wp-admin/admin-ajax.php", "mjsTimeFormat": "h:mm a", "datePicker": {"format": "MMMM D, YYYY", "monthNames": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], "daysOfWeek": ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], "firstDay": 1, "monthNamesShort": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], "dayNames": ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], "dayNamesShort": ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], "meridiem": {"am": "am", "pm": "pm", "AM": "AM", "PM": "PM"}}, "dateRange": {"format": "MMMM D, YYYY", "applyLabel": "Apply", "cancelLabel": "Cancel", "fromLabel": "From", "toLabel": "To", "customRangeLabel": "Custom range", "tomorrow": "Tomorrow", "today": "Today", "anyTime": "Any time", "yesterday": "Yesterday", "last_7": "Last 7 days", "last_30": "Last 30 days", "next_7": "Next 7 days", "next_30": "Next 30 days", "thisMonth": "This month", "nextMonth": "Next month", "lastMonth": "Last month", "firstDay": 1}, "l10n": {"apply": "Apply", "cancel": "Cancel", "areYouSure": "Are you sure?"}, "addons": [], "cloud_products": "", "data": {}};
//# sourceURL=bookly-globals-js-extra

/* ---- inline <script id="bookly-bookly.min.js-js-extra"> ---- */
var BooklyL10n = {"ajaxurl": "https://motors.stylemixthemes.com/motorcycles/wp-admin/admin-ajax.php", "csrf_token": "9facfd6f9c", "months": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], "days": ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], "daysShort": ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], "monthsShort": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], "show_more": "Show more", "sessionHasExpired": "Your session has expired. Please press \"Ok\" to refresh the page"};
//# sourceURL=bookly-bookly.min.js-js-extra

/* ---- inline <script type="text/javascript"> (stm globals) ---- */
var stm_lang_code = 'en';
var ajaxurl = 'wp-admin/admin-ajax.html';
var mvl_current_ajax_url = 'index.html';
var stm_site_blog_id = "5";
var stm_added_to_compare_text = "Added to compare";
var stm_removed_from_compare_text = "was removed from compare";
var stm_already_added_to_compare_text = "You have already added 3 cars";
var reset_all_txt = "Reset All";
var photo_remaining_singular = 'more photo';
var photo_remaining_plural = 'more photos';

/* ---- inline <script type="text/javascript"> (stm_wpcfto_ajaxurl) ---- */
var stm_wpcfto_ajaxurl = 'wp-admin/admin-ajax.html';

/* ---- inline <script> (stm_wpcfto_nonces) ---- */
var stm_wpcfto_nonces = {"wpcfto_save_settings": "12bfe0b729", "get_image_url": "1c4081c7fc", "wpcfto_upload_file": "371d1d75c4", "wpcfto_search_posts": "c0afd4031f", "wpcfto_regenerate_fonts": "b193a51c75", "wpcfto_create_term": "b37e082fce"};

/* ---- inline <script> (setREVStartSize) ---- */
function setREVStartSize(e) {
    //window.requestAnimationFrame(function() {
    window.RSIW = window.RSIW === undefined ? window.innerWidth : window.RSIW;
    window.RSIH = window.RSIH === undefined ? window.innerHeight : window.RSIH;
    try {
        var pw = document.getElementById(e.c).parentNode.offsetWidth,
            newh;
        pw = pw === 0 || isNaN(pw) || (e.l == "fullwidth" || e.layout == "fullwidth") ? window.RSIW : pw;
        e.tabw = e.tabw === undefined ? 0 : parseInt(e.tabw);
        e.thumbw = e.thumbw === undefined ? 0 : parseInt(e.thumbw);
        e.tabh = e.tabh === undefined ? 0 : parseInt(e.tabh);
        e.thumbh = e.thumbh === undefined ? 0 : parseInt(e.thumbh);
        e.tabhide = e.tabhide === undefined ? 0 : parseInt(e.tabhide);
        e.thumbhide = e.thumbhide === undefined ? 0 : parseInt(e.thumbhide);
        e.mh = e.mh === undefined || e.mh == "" || e.mh === "auto" ? 0 : parseInt(e.mh, 0);
        if (e.layout === "fullscreen" || e.l === "fullscreen")
            newh = Math.max(e.mh, window.RSIH);
        else {
            e.gw = Array.isArray(e.gw) ? e.gw : [e.gw];
            for (var i in e.rl) if (e.gw[i] === undefined || e.gw[i] === 0) e.gw[i] = e.gw[i - 1];
            e.gh = e.el === undefined || e.el === "" || (Array.isArray(e.el) && e.el.length == 0) ? e.gh : e.el;
            e.gh = Array.isArray(e.gh) ? e.gh : [e.gh];
            for (var i in e.rl) if (e.gh[i] === undefined || e.gh[i] === 0) e.gh[i] = e.gh[i - 1];

            var nl = new Array(e.rl.length),
                ix = 0,
                sl;
            e.tabw = e.tabhide >= pw ? 0 : e.tabw;
            e.thumbw = e.thumbhide >= pw ? 0 : e.thumbw;
            e.tabh = e.tabhide >= pw ? 0 : e.tabh;
            e.thumbh = e.thumbhide >= pw ? 0 : e.thumbh;
            for (var i in e.rl) nl[i] = e.rl[i] < window.RSIW ? 0 : e.rl[i];
            sl = nl[0];
            for (var i in nl) if (sl > nl[i] && nl[i] > 0) { sl = nl[i]; ix = i; }
            var m = pw > (e.gw[ix] + e.tabw + e.thumbw) ? 1 : (pw - (e.tabw + e.thumbw)) / (e.gw[ix]);
            newh = (e.gh[ix] * m) + (e.tabh + e.thumbh);
        }
        var el = document.getElementById(e.c);
        if (el !== null && el) el.style.height = newh + "px";
        el = document.getElementById(e.c + "_wrapper");
        if (el !== null && el) {
            el.style.height = newh + "px";
            el.style.display = "block";
        }
    } catch (e) {
        console.log("Failure at Presize of Slider:" + e)
    }
    //});
};

/* ---- inline <script> (rev slider init call) ---- */
setREVStartSize({c: 'rev_slider_4_1', rl: [1240, 1024, 768, 480], el: [757, 757, 757, 757], gw: [1240, 1024, 778, 480], gh: [757, 757, 757, 757], type: 'standard', justify: '', layout: 'fullwidth', mh: "757"});
if (window.RS_MODULES !== undefined && window.RS_MODULES.modules !== undefined && window.RS_MODULES.modules["revslider41"] !== undefined) {
    window.RS_MODULES.modules["revslider41"].once = false;
    window.revapi4 = undefined;
    if (window.RS_MODULES.checkMinimal !== undefined) window.RS_MODULES.checkMinimal();
}

/* ---- inline <script> (filter selects / select2) ---- */
(function ($) {
    "use strict";

    $(document).ready(function () {
        var stmTaxRelations = Array;

        $('.stm_mc-filter-selects select:not(.hide)').select2().on('change', function () {

            var stmCurVal = $(this).val();
            var stmCurSelect = $(this).attr('name');

            if (stmTaxRelations[stmCurSelect]) {
                var key = stmTaxRelations[stmCurSelect]['dependency'];
                $('select[name="' + key + '"]').val('');
                if (stmCurVal === '') {
                    $('select[name="' + key + '"] > option').each(function () {
                        $(this).removeAttr('disabled');
                    });
                } else {
                    var allowedTerms = stmTaxRelations[stmCurSelect][stmCurVal];

                    if (typeof (allowedTerms) == 'object') {
                        $('select[name="' + key + '"] > option').removeAttr('disabled');

                        $('select[name="' + key + '"] > option').each(function () {
                            var optVal = $(this).val();
                            if (optVal != '' && $.inArray(optVal, allowedTerms) == -1) {
                                $(this).attr('disabled', '1');
                            }
                        });
                    } else {
                        $('select[name="' + key + '"]').val(allowedTerms);
                    }

                    if (typeof (stmTaxRelations[stmCurSelect][stmCurVal]) == 'undefined') {
                        $('select[name="' + key + '"] > option').each(function () {
                            $(this).removeAttr('disabled');
                        });
                    }
                }

                $('.stm_mc-filter-selects select[name="' + key + '"]').select2("destroy");
                $('.stm_mc-filter-selects select[name="' + key + '"]').select2();
            }
        });
    });

})(jQuery);

/* ---- inline lightGallery photo handlers (listing cards - all occurrences) ---- */
jQuery(document).ready(function () {
    // Photos: 4239-44869
    jQuery(".stm-car-photos-4239-44869").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-1.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-1.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4239-71067
    jQuery(".stm-car-videos-4239-71067").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4238-47824
    jQuery(".stm-car-photos-4238-47824").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-4.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-4.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4238-62534
    jQuery(".stm-car-videos-4238-62534").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4255-60776
    jQuery(".stm-car-photos-4255-60776").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-2.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-1.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-1.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-6.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-6.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4255-51534
    jQuery(".stm-car-videos-4255-51534").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4252-56836
    jQuery(".stm-car-photos-4252-56836").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv0-1.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv0-1.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv6.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv6.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport2.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_4.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_4.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4252-87481
    jQuery(".stm-car-videos-4252-87481").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4251-52561
    jQuery(".stm-car-photos-4251-52561").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-8.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-8.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4251-40681
    jQuery(".stm-car-videos-4251-40681").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4250-94053
    jQuery(".stm-car-photos-4250-94053").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-2.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-7.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4250-48565
    jQuery(".stm-car-videos-4250-48565").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4238-69680 (recent tab)
    jQuery(".stm-car-photos-4238-69680").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-4.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-4.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4238-65244
    jQuery(".stm-car-videos-4238-65244").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4255-73705
    jQuery(".stm-car-photos-4255-73705").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-2.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-1.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-1.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-6.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-6.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4255-50656
    jQuery(".stm-car-videos-4255-50656").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4252-39089
    jQuery(".stm-car-photos-4252-39089").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv0-1.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv0-1.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv6.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv6.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport2.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_sport.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_4.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/atv_4.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4252-77118
    jQuery(".stm-car-videos-4252-77118").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4251-8047
    jQuery(".stm-car-photos-4251-8047").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-8.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-8.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-5.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-7.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/motorcycle-2.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4251-55259
    jQuery(".stm-car-videos-4251-55259").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4250-13584
    jQuery(".stm-car-photos-4250-13584").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-2.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-2.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-7.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/watercraft-7.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4250-92265
    jQuery(".stm-car-videos-4250-92265").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
    // Photos: 4248-36802
    jQuery(".stm-car-photos-4248-36802").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, dynamicEl: [{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-4.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-4.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-1.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-1.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-3.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-3.jpg"},{src:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-6.jpg",thumb:"https://motors.stylemixthemes.com/motorcycles/wp-content/uploads/sites/5/2016/08/scooter-6.jpg"}], download: false, mode: 'lg-fade' });
    });
    // Videos: 4248-751
    jQuery(".stm-car-videos-4248-751").on('click', function (e) {
        e.preventDefault();
        jQuery(this).lightGallery({ dynamic: true, iframe: true, dynamicEl: [{src:"https://www.youtube.com/embed/QeKFc3BNtU4"}], download: false, mode: 'lg-fade' });
    });
});

/* ---- inline <script> (iframe video poster player) ---- */
(function ($) {
    "use strict";

    $(document).ready(function ($) {
        stmPlayIframeVideo();
    });

    function stmPlayIframeVideo() {
        $('.stm-boats-video-poster').on('click', function (e) {
            e.preventDefault();
            var url = $(this).attr('data-url');
            jQuery(this).lightGallery({
                dynamic: true,
                dynamicEl: [{ src: url }],
                download: false,
                mode: 'lg-fade',
            });
        });
    }

})(jQuery);

/* ---- inline <script> (mailchimp mc4wp) ---- */
(function () {
    window.mc4wp = window.mc4wp || {
        listeners: [],
        forms: {
            on: function (evt, cb) {
                window.mc4wp.listeners.push({ event: evt, callback: cb });
            }
        }
    };
})();

/* ---- inline <script> (RS_MODULES init) ---- */
window.RS_MODULES = window.RS_MODULES || {};
window.RS_MODULES.modules = window.RS_MODULES.modules || {};
window.RS_MODULES.waiting = window.RS_MODULES.waiting || [];
window.RS_MODULES.defered = false;
window.RS_MODULES.moduleWaiting = window.RS_MODULES.moduleWaiting || {};
window.RS_MODULES.type = 'compiled';

/* ---- inline <script type="text/javascript"> (Matomo analytics) ---- */
var _paq = _paq || [];
_paq.push(["setDocumentTitle", "5/" + document.title]);
_paq.push(['trackPageView']);
(function () {
    var u = "https://analytics2.wpmudev.com/";
    _paq.push(['setTrackerUrl', u + 'track/']);
    _paq.push(['setSiteId', '13741']);
    var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
    g.type = 'text/javascript';
    g.async = true;
    g.defer = true;
    g.src = '../../analytics.wpmucdn.com/matomo.js';
    s.parentNode.insertBefore(g, s);
})();

/* ---- inline <script> (stm security nonces + globals) ---- */
var stm_security_nonce = 'bbfb380a4f';
var stm_media_security_nonce = '8ea3864ecc';
var stm_listings_user_data_nonce = '5c8894d901';
var stm_car_price_nonce = '9a86e10155';
var stm_compare_list_nonce = 'abbf15c69e';
var stm_custom_register_nonce = '1913af34a2';
var stm_custom_login_nonce = 'b650bc44a8';
var stm_add_test_drive_nonce = '75d2f5beb2';
var stm_logout_user_nonce = 'e9dd63a53b';
var cc_prefix = 'stm5_compare_';
var compare_init_object = {"listings": []};
var allowDealerAddCategory = '';
var noFoundSelect2 = 'No results found';

/* ---- inline <script> (trade-in modal hash check) ---- */
(function ($) {
    "use strict";

    $(document).ready(function () {
        if (window.location.hash === '#error-fields') {
            $('#trade-in').modal('show');
            history.pushState("", document.title, window.location.pathname + window.location.search);
        }
    });

})(jQuery);

/* ---- inline <script> (statistics modal / chart.js) ---- */
(function ($) {
    $(document).ready(function () {

        let stat_listing_id = '';

        $('.listing_stats_wrap > div').on('click touchend', function () {
            var type = $(this).data('type');
            $('ul.statistics-type li').addClass('off');

            if (type == 'phone') {
                $('ul.statistics-type li.phone_reveals').removeClass('off');
            } else {
                $('ul.statistics-type li.listing_views').removeClass('off');
            }

            $('#statistics-modal').modal('show');

            if (typeof $(this).data('id') !== 'undefined') {
                stat_listing_id = $(this).data('id');
                build_stat_chart(stat_listing_id);
            }

            $('#statistics-listing-title').html($(this).data('title'));
        });

        $('ul.statistics-type li').on('click', function () {
            if ($(this).hasClass('off')) {
                $(this).removeClass('off');
            } else {
                $(this).addClass('off');
            }
            if (stat_listing_id > 0) build_stat_chart(stat_listing_id);
        });

        $('#statistics-period').on('change', function () {
            if (stat_listing_id > 0) build_stat_chart(stat_listing_id);
        });

        var config = {
            type: 'line',
            data: {
                labels: [],
                tooltipDates: [],
                datasets: [
                    {
                        label: 'Listing views',
                        backgroundColor: 'rgba(18, 128, 223,0.7)',
                        borderColor: 'rgba(18, 128, 223,0.7)',
                        data: [],
                        pointBackgroundColor: 'rgba(18, 128, 223,0.7)',
                    },
                    {
                        label: 'Phone views',
                        backgroundColor: 'rgb(22, 203, 100, 0.4)',
                        borderColor: 'rgb(22, 203, 100)',
                        data: [],
                        pointBackgroundColor: 'rgb(22, 203, 100)'
                    }
                ]
            },
            options: {
                'plugins': {
                    'legend': { 'display': false },
                    'tooltip': {
                        titleAlign: 'center',
                        yAlign: 'bottom',
                        titleFont: { weight: 'normal' },
                        callbacks: {
                            title: function (tooltipItem) {
                                return tooltipItem[0].chart.config.data.tooltipDates[tooltipItem[0].dataIndex];
                            }
                        },
                        titleColor: '#fff',
                        usePointStyle: true,
                        displayColors: true,
                        boxWidth: 8,
                        boxHeight: 8,
                    }
                },
                'elements': {
                    'line': { 'fill': true, 'borderWidth': 1.5 },
                    'point': { 'borderWidth': 4 }
                },
                scale: { ticks: { precision: 0 } },
                scales: { y: { min: 0 } }
            }
        };

        listingsChart = new Chart(document.getElementById('listingsChart'), config);

        var stat_data = [];

        function build_stat_chart(id) {
            if (typeof stat_data === 'undefined' || stat_data.length == 0) return;

            listingsChart.data.labels = [];
            listingsChart.data.tooltipDates = [];
            listingsChart.data.datasets[0].data = [];
            listingsChart.data.datasets[1].data = [];

            if ($('#statistics-period').val() == 'week') {
                var seven_days = Object.keys(stat_data[id]).slice(-7).map(key => ({ [key]: stat_data[id][key] }));
                for (var key in seven_days) {
                    for (var date in seven_days[key]) {
                        listingsChart.data.labels.push(seven_days[key][date].day_name);
                        listingsChart.data.tooltipDates.push(seven_days[key][date].tooltip_date);
                        if (!$('#view-toggler').hasClass('off')) listingsChart.data.datasets[0].data.push(seven_days[key][date].view);
                        if (!$('#phone-toggler').hasClass('off')) listingsChart.data.datasets[1].data.push(seven_days[key][date].phone);
                    }
                }
            } else {
                Object.values(stat_data[id]).forEach(val => {
                    listingsChart.data.labels.push(val.day_number);
                    listingsChart.data.tooltipDates.push(val.tooltip_date);
                    if (!$('#view-toggler').hasClass('off')) listingsChart.data.datasets[0].data.push(val.view);
                    if (!$('#phone-toggler').hasClass('off')) listingsChart.data.datasets[1].data.push(val.phone);
                });
            }
            listingsChart.update();
        }
    });
})(jQuery);

/* ---- inline <script> (mmAjaxUrl) ---- */
var mmAjaxUrl = 'wp-admin/admin-ajax.html';

/* ---- inline <script> (mc4wp url prefix) ---- */
(function () {
    function maybePrefixUrlField() {
        const value = this.value.trim();
        if (value !== '' && value.indexOf('http') !== 0) {
            this.value = 'http://' + value;
        }
    }
    const urlFields = document.querySelectorAll('.mc4wp-form input[type="url"]');
    for (let j = 0; j < urlFields.length; j++) {
        urlFields[j].addEventListener('blur', maybePrefixUrlField);
    }
})();

/* ---- inline <script type="text/javascript"> (Freemius / demos panel) ---- */
jQuery('document').ready(function () {
    var $ = jQuery;
    var api_url = 'https://stylemixthemes.scdn2.secure.raxcdn.com/api/prices.json';

    $.ajax({
        url: api_url,
        dataType: 'json',
        context: this,
        complete: function (data) {
            var r = data.responseJSON;
            $('.stm_price_api').text(r.themes.motors.price);
        }
    });

    var freemiusCheckoutConfig = {"product_id": "32800", "public_key": "pk_e2778e7c3157e2a166c1e2769094d", "name": "Motors", "licenses": 1, "billing_cycle": "lifetime"};
    var freemiusCheckout = null;

    if (window.FS && FS.Checkout) {
        freemiusCheckout = new FS.Checkout({
            product_id: freemiusCheckoutConfig.product_id,
            public_key: freemiusCheckoutConfig.public_key
        });
    }

    $('#crypterio-buy').on('click', function (event) {
        event.preventDefault();
        if (!freemiusCheckout) return;
        freemiusCheckout.open({
            name: freemiusCheckoutConfig.name,
            licenses: freemiusCheckoutConfig.licenses,
            billing_cycle: freemiusCheckoutConfig.billing_cycle
        });
    });

    $('#crypterio-demos').on('click', function () {
        $('#demo_popups').addClass('opened');
        $('body').addClass('fancy-lock');
    });

    $('.stm_theme_demos__close').on('click', function () {
        $('.stm_theme_demos__popup').removeClass('opened');
        $('body').removeClass('fancy-lock');
    });

    $('#qr-codes').on('click', function () {
        $('#qr_popup').addClass('opened');
        $('body').addClass('fancy-lock');
    });

    $('.stm_theme_demos__icon--toggle').on('click', function () {
        $(this).closest('.stm_theme_demos').toggleClass('expand');
    });

    $('body').on('click', function () {
        if (!$('.stm_theme_demos:hover').length) {
            $('.stm_theme_demos').removeClass('expand');
        }
    });
});

/* ---- inline <script> (stm motors globals #2) ---- */
var stm_security_nonce = 'bbfb380a4f';
var stm_motors_current_ajax_url = 'index.html';
var resetAllTxt = 'Reset All';
var prevText = 'Previous';
var nextText = 'Next';
var is_rental = 'false';
var file_type = 'file type noimg';
var file_size = 'file size big';
var max_img_quant = 'max imgs 3';
var currentLocale = 'en';
var noFoundSelect2 = 'No results found';
var stm_login_to_see_plans = 'Please, log in to view your available plans';
var compare_init_object = {"listings": []};
var ajax_url = 'wp-admin/admin-ajax.html';
var cc_prefix = 'stm5_compare_';

/* ---- inline <script type="text/javascript"> (loan calculator) ---- */
(function ($) {
    "use strict";

    $(document).ready(function () {
        var vehicle_price;
        var interest_rate;
        var down_payment;
        var period_month;

        var stmCurrency = "$";
        var stmPriceDel = " ";
        var stmCurrencyPos = "left";

        $('.calculate_loan_payment').on('click', function (e) {
            e.preventDefault();

            var current_calculator = $(this).closest('.stm_auto_loan_calculator');
            var calculator_alert = current_calculator.find('.calculator-alert');
            calculator_alert.removeClass('visible-alert');

            vehicle_price = parseFloat(current_calculator.find('input.vehicle_price').val());
            interest_rate = parseFloat(current_calculator.find('input.interest_rate').val());
            interest_rate = interest_rate / 1200;
            down_payment = parseFloat(current_calculator.find('input.down_payment').val());
            period_month = parseFloat(current_calculator.find('input.period_month').val());

            var validation_errors = true;
            var monthly_payment = 0;
            var total_interest_payment = 0;
            var total_amount_to_pay = 0;

            if (isNaN(vehicle_price)) {
                calculator_alert.text("Please fill Vehicle Price field");
                calculator_alert.addClass('visible-alert');
                current_calculator.find('input.vehicle_price').closest('.form-group').addClass('has-error');
                validation_errors = true;
            } else if (isNaN(interest_rate)) {
                calculator_alert.text("Please fill Interest Rate field");
                calculator_alert.addClass('visible-alert');
                current_calculator.find('input.interest_rate').closest('.form-group').addClass('has-error');
                validation_errors = true;
            } else if (isNaN(period_month)) {
                calculator_alert.text("Please fill Period field");
                calculator_alert.addClass('visible-alert');
                current_calculator.find('input.period_month').closest('.form-group').addClass('has-error');
                validation_errors = true;
            } else if (isNaN(down_payment)) {
                calculator_alert.text("Please fill Down Payment field");
                calculator_alert.addClass('visible-alert');
                current_calculator.find('input.down_payment').closest('.form-group').addClass('has-error');
                validation_errors = true;
            } else if (down_payment > vehicle_price) {
                calculator_alert.text("Down payment can not be more than vehicle price");
                calculator_alert.addClass('visible-alert');
                current_calculator.find('input.down_payment').closest('.form-group').addClass('has-error');
                validation_errors = true;
            } else {
                validation_errors = false;
            }

            if (!validation_errors) {
                var interest_rate_unused = interest_rate;
                var mathPow = Math.pow(1 + interest_rate, period_month);

                if (interest_rate == 0) interest_rate_unused = 1;

                monthly_payment = (interest_rate_unused != 1)
                    ? (vehicle_price - down_payment) * interest_rate_unused * mathPow
                    : (vehicle_price - down_payment) / period_month;

                var monthly_payment_div = (mathPow - 1);
                if (monthly_payment_div == 0) monthly_payment_div = 1;

                monthly_payment = (monthly_payment / monthly_payment_div).toFixed(2);

                total_amount_to_pay = down_payment + (monthly_payment * period_month);
                total_amount_to_pay = (interest_rate == 0) ? vehicle_price : total_amount_to_pay.toFixed(2);

                total_interest_payment = total_amount_to_pay - vehicle_price;
                total_interest_payment = (interest_rate == 0) ? 0 : total_interest_payment.toFixed(2);

                current_calculator.find('.stm_calculator_results').slideDown();
                current_calculator.find('.monthly_payment').text(monthly_payment);
                current_calculator.find('.total_interest_payment').text(total_interest_payment);
                current_calculator.find('.total_amount_to_pay').text(total_amount_to_pay);
            } else {
                current_calculator.find('.stm_calculator_results').slideUp();
                current_calculator.find('.monthly_payment').text('');
                current_calculator.find('.total_interest_payment').text('');
                current_calculator.find('.total_amount_to_pay').text('');
            }
        });

        $(".numbersOnly").on("keypress keyup blur", function (event) {
            $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
            if ($(this).val() != '') {
                $(this).closest('.form-group').removeClass('has-error');
            }
        });
    });

})(jQuery);

/* ---- inline <script type="text/javascript"> (sbiajaxurl) ---- */
var sbiajaxurl = "wp-admin/admin-ajax.html";

/* ---- inline <script type="text/javascript"> (preloader timer) ---- */
var preloader_timer = "300";

/* ---- inline <script type="text/javascript"> (woocommerce-js class swap) ---- */
(function () {
    var c = document.body.className;
    c = c.replace(/woocommerce-no-js/, 'woocommerce-js');
    document.body.className = c;
})();

/* ---- inline <script> (revslider double jQuery guard) ---- */
if (typeof revslider_showDoubleJqueryError === "undefined") {
    function revslider_showDoubleJqueryError(sliderID) {
        console.log("You have some jquery.js library include that comes after the Slider Revolution files js inclusion.");
        console.log("To fix this, you can:");
        console.log("1. Set 'Module General Options' -> 'Advanced' -> 'jQuery & OutPut Filters' -> 'Put JS to Body' to on");
        console.log("2. Find the double jQuery.js inclusion and remove it");
        return "Double Included jQuery Library";
    }
}

/* ---- inline <script id="contact-form-7-js-extra"> ---- */
var wpcf7 = {"api": {"root": "https://motors.stylemixthemes.com/motorcycles/wp-json/", "namespace": "contact-form-7/v1"}, "cached": "1"};
//# sourceURL=contact-form-7-js-extra

/* ---- inline <script id="listings-init-js-extra"> ---- */
var stm_i18n = {"stm_label_add": "Add to compare", "stm_label_remove": "Remove from compare", "stm_label_remove_list": "Remove from list", "stm_label_in_compare": "In compare list", "add_to_compare": "Add to compare", "remove_from_compare": "Remove from compare", "stm_already_added_to_compare_text": "You have already added 3 cars", "remove_from_favorites": "Remove from favorites", "add_to_favorites": "Add to favorites", "motors_vl_config": {"enable_friendly_urls": true}, "required_fields": "Please enter required fields", "image_upload_required": "Please upload a photo to create a listing", "seller_notes_required": "Please leave a seller\u2019s note to create a listing", "features_required": "Please choose at least one feature to create a listing", "video_required": "Please share a video URL to create a listing", "car_price_required": "Please add item price", "mvl_current_page_url": "https://motors.stylemixthemes.com/motorcycles/", "mvl_search_placeholder": "Search", "mvl_password_validation": "Password must contain at least 8 characters."};
//# sourceURL=listings-init-js-extra

/* ---- inline <script id="app-ajax-js-after"> ---- */
var onloadRecaptchaCallback = function () {
    var submitButton = document.querySelector('.stm-login-register-form .stm-register-form form input[type="submit"]');
    if (submitButton) {
        submitButton.setAttribute('disabled', '1');
    }
};
//# sourceURL=app-ajax-js-after

/* ---- inline <script id="wc-order-attribution-js-extra"> ---- */
var wc_order_attribution = {"params": {"lifetime": 1.0e-5, "session": 30, "ajaxurl": "https://motors.stylemixthemes.com/motorcycles/wp-admin/admin-ajax.php", "prefix": "wc_order_attribution_", "allowTracking": true}, "fields": {"source_type": "current.typ", "referrer": "current_add.rf", "utm_campaign": "current.cmp", "utm_source": "current.src", "utm_medium": "current.mdm", "utm_content": "current.cnt", "utm_id": "current.id", "utm_term": "current.trm", "session_entry": "current_add.ep", "session_start_time": "current_add.fd", "session_pages": "session.pgs", "session_count": "udata.vst", "user_agent": "udata.uag"}};
//# sourceURL=wc-order-attribution-js-extra

/* ---- inline <script id="stm-theme-scripts-js-extra"> ---- */
var motors_i18n = [];
var stm_theme_config = {"enable_friendly_urls": "1"};
//# sourceURL=stm-theme-scripts-js-extra

/* ---- inline <script id="stm-theme-scripts-js-after"> ---- */
var stm_cats_conf = {"condition": false, "body": false, "category_type": false, "make": false, "serie": false, "mileage": true, "ca-year": false, "price": true};
//# sourceURL=stm-theme-scripts-js-after

/* ---- inline <script id="rs-initialisation-scripts"> (Revolution Slider init) ---- */
var tpj = jQuery;
var revapi4;

if (window.RS_MODULES === undefined) window.RS_MODULES = {};
if (RS_MODULES.modules === undefined) RS_MODULES.modules = {};
RS_MODULES.modules["revslider41"] = {
    once: RS_MODULES.modules["revslider41"] !== undefined ? RS_MODULES.modules["revslider41"].once : undefined,
    init: function () {
        window.revapi4 = window.revapi4 === undefined || window.revapi4 === null || window.revapi4.length === 0
            ? document.getElementById("rev_slider_4_1")
            : window.revapi4;
        if (window.revapi4 === null || window.revapi4 === undefined || window.revapi4.length == 0) {
            window.revapi4initTry = window.revapi4initTry === undefined ? 0 : window.revapi4initTry + 1;
            if (window.revapi4initTry < 20) requestAnimationFrame(function () { RS_MODULES.modules["revslider41"].init(); });
            return;
        }
        window.revapi4 = jQuery(window.revapi4);
        if (window.revapi4.revolution == undefined) { revslider_showDoubleJqueryError("rev_slider_4_1"); return; }
        revapi4.revolutionInit({
            revapi: "revapi4",
            sliderLayout: "fullwidth",
            duration: 3000,
            visibilityLevels: "1240,1024,768,480",
            gridwidth: "1240,1024,778,480",
            gridheight: "757,757,757,757",
            minHeight: 757,
            lazyType: "smart",
            spinner: "spinner2",
            perspectiveType: "local",
            editorheight: "757,757,757,757",
            responsiveLevels: "1240,1024,768,480",
            progressBar: { disableProgressBar: true },
            navigation: { onHoverStop: false },
            parallax: {
                levels: [5, 10, 15, 20, 25, 30, 35, 40, 45, 46, 47, 48, 49, 50, 51, 55],
                type: "mousescroll",
                disable_onmobile: true
            },
            viewPort: {
                global: true,
                globalDist: "-200px",
                enable: false,
                visible_area: "20%"
            },
            fallbacks: { allowHTML5AutoPlayOnAndroid: true },
        });
    }
};
if (window.RS_MODULES.checkMinimal !== undefined) { window.RS_MODULES.checkMinimal(); }

/* ---- inline <script> (Cloudflare challenge iframe) ---- */
(function () {
    function c() {
        var b = a.contentDocument || (a.contentWindow && a.contentWindow.document);
        if (b) {
            var d = b.createElement('script');
            d.innerHTML = "window.__CF$cv$params={r:'a4858e3b0b2fe21c',t:'MTc5MTYzNDU3OQ=='};var a=document.createElement('script');a.src='../cdn-cgi/challenge-platform/h/g/scripts/jsd/4df4a60fa397/maind41d.js';document.getElementsByTagName('head')[0].appendChild(a);";
            b.getElementsByTagName('head')[0].appendChild(d);
        }
    }
    if (document.body) {
        var a = document.createElement('iframe');
        a.height = 1; a.width = 1;
        a.style.position = 'absolute'; a.style.top = 0; a.style.left = 0;
        a.style.border = 'none'; a.style.visibility = 'hidden';
        document.body.appendChild(a);
        if ('loading' !== document.readyState) c();
        else if (window.addEventListener) document.addEventListener('DOMContentLoaded', c);
        else {
            var e = document.onreadystatechange || function () { };
            document.onreadystatechange = function (b) {
                e(b);
                'loading' !== document.readyState && (document.onreadystatechange = e, c());
            };
        }
    }
})();
