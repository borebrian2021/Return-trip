; (function ($) {
	'use strict'

	$(window).on('load', function () {
		stm_scroll_user_sidebar()
		stm_private_user_height()
	})

	$(window).on('scroll', function () {
		stm_scroll_user_sidebar()
	})

	$(window).on('resize', function () {
		stm_private_user_height()
	})

	function stm_scroll_user_sidebar() {
		if ($(window).width() > 820) {
			var $stm_window = $(window)
			var $stm_sidebar = $('.stm-sticky-user-sidebar .stm-user-private-sidebar')
			var $stm_sidebar_holder = $('.stm-sticky-user-sidebar')

			if (
				$stm_sidebar.outerHeight() < $stm_window.outerHeight() &&
				$stm_sidebar.length
			) {
				var currentScrollPos = $(window).scrollTop()
				var sidebarPos = $stm_sidebar_holder.offset().top

				if (currentScrollPos > sidebarPos) {
					$stm_sidebar.addClass('side-fixed')
				} else {
					$stm_sidebar.removeClass('side-fixed')
				}
			}
		}
	}

	function stm_private_user_height() {
		var windowH = $(window).outerHeight()
		var topBarH = $('#top-bar').outerHeight() || 0
		var headerH = $('#header').outerHeight() || 0
		var minH = windowH - (topBarH + headerH)

		$('.stm-user-private-sidebar').css({
			'min-height': minH + 'px',
		})
	}
})(jQuery)
