function onSubmitPageRegister(token) {
	let $ = jQuery
	var form = $('#page-register-form');
	$.ajax({
		type: "POST",
		url: ajaxurl,
		dataType: 'json',
		context: this,
		data: form.serialize() + '&action=stm_custom_register',
		beforeSend: function () {
			form.find('input').removeClass('form-error');
			form.find('.stm-listing-loader').addClass('visible');
			$('.stm-validation-message').empty();
		},
		success: function (data) {
			if (data.user_html) {
				$('#stm_user_info').append(data.user_html);

				$('.stm-not-disabled, .stm-not-enabled').slideUp('fast', function () {
					$('#stm_user_info').slideDown('fast');
				});
				$("html, body").animate({ scrollTop: $('.stm-form-checking-user').offset().top }, "slow");

				$('.stm-form-checking-user button[type="submit"]').removeClass('disabled').addClass('enabled');
			}

			if (data.restricted && data.restricted) {
				$('.btn-add-edit').remove();
			}

			form.find('.stm-listing-loader').removeClass('visible');
			for (var err in data.errors) {
				form.find('input[name=' + err + ']').addClass('form-error');
			}

			// insert plans select
			if (data.plans_select && $('#user_plans_select_wrap').length > 0) {
				$('#user_plans_select_wrap').html(data.plans_select);
				$('#user_plans_select_wrap select').select2();
			}

			if (data.redirect_url) {
				window.location = data.redirect_url;
			}

			if (data.message) {
				var message = $('<div class="stm-message-ajax-validation heading-font">' + data.message + '</div>').hide();

				form.find('.stm-validation-message').append(message);
				message.slideDown('fast');
			}
		}
	});
}

jQuery(document).ready(function ($) {

	$('.stm-show-password .fas').mousedown(function () {
		$(this).closest('.stm-show-password').find('input').attr('type', 'text');
		$(this).addClass('fa-eye');
		$(this).removeClass('fa-eye-slash');
	});

	$(document).mouseup(function () {
		$('.stm-show-password').find('input').attr('type', 'password');
		$('.stm-show-password .fas').addClass('fa-eye-slash');
		$('.stm-show-password .fas').removeClass('fa-eye');
	});

	$("body").on('touchstart', '.stm-show-password .fas', function () {
		$(this).closest('.stm-show-password').find('input').attr('type', 'text');
		$(this).addClass('fa-eye');
		$(this).removeClass('fa-eye-slash');
	});

	$('.stm-forgot-password a').on(
		'click',
		function (e) {
			e.preventDefault();
			var login_popup = $(this).parents('.stm-login-form-unregistered, .stm-login-form-mobile-unregistered');
			if (login_popup.length) {
				$('.stm-login-form-unregistered form, .stm-login-form-mobile-unregistered form').slideToggle();
				$('.stm-login-form-unregistered .stm_forgot_password_send input[type=text], .stm-login-form-mobile-unregistered .stm_forgot_password_send input[type=text]').trigger('focus');
			} else {
				$('.stm-login-form .stm_forgot_password_send').slideToggle();
				$('.stm-login-form .stm_forgot_password_send input[type=text]').trigger('focus');
			}
			$(this).toggleClass('active');
		}
	);
	$('.stm-forgot-password-back').on(
		'click',
		function (e) {
			e.preventDefault();
			$('.stm-login-form-unregistered form, .stm-login-form-mobile-unregistered form').slideToggle();
		}
	);
	$(".stm-login-form-mobile-unregistered form,.stm-login-form form:not(.stm_password_recovery), .stm-login-form-unregistered form").on(
		'submit',
		function (e) {
			e.preventDefault();
			if (!$(this).hasClass('stm_forgot_password_send')) {
				$.ajax(
					{
						type: "POST",
						url: ajaxurl,
						dataType: 'json',
						context: this,
						data: $(this).serialize() + '&action=stm_custom_login',
						beforeSend: function () {
							$(this).find('input').removeClass('form-error');
							$(this).find('.stm-listing-loader').addClass('visible');
							$('.stm-validation-message').empty();

							if ($(this).parent('.lOffer-account-unit').length > 0) {
								$('.stm-login-form-unregistered').addClass('working');
							}
						},
						success: function (data) {
							if ($(this).parent('.lOffer-account-unit').length > 0) {
								$('.stm-login-form-unregistered').addClass('working');
							}
							if (data.user_html) {
								$('.add-car-btns-wrap').remove();
								var $user_html = $(data.user_html).appendTo('#stm_user_info');
								$('.stm-not-disabled, .stm-not-enabled').slideUp(
									'fast',
									function () {
										$('#stm_user_info').slideDown('fast');
									}
								);

								$("html, body").animate({ scrollTop: $('.stm-form-checking-user').offset().top }, "slow");
								$('.stm-add-a-car-login-overlay,.stm-add-a-car-login').toggleClass('visiblity');

								$('.stm-form-checking-user button[type="submit"]').removeClass('disabled').addClass('enabled');
							}

							if (data.restricted && data.restricted) {
								$('.btn-add-edit').remove();
							}

							// insert plans select
							if (data.plans_select && $('#user_plans_select_wrap').length > 0) {
								$('#user_plans_select_wrap').html(data.plans_select);
								$('#user_plans_select_wrap select').select2(
									{
										dropdownParent: $('body'),
									}
								);
							}

							$(this).find('.stm-listing-loader').removeClass('visible');
							for (var err in data.errors) {
								$(this).find('input[name=' + err + ']').addClass('form-error');
							}

							if (data.message) {
								var message = $('<div class="stm-message-ajax-validation heading-font">' + data.message + '</div>').hide();

								$(this).find('.stm-validation-message').append(message);
								message.slideDown('fast');
							}

							if (typeof (data.redirect_url) !== 'undefined') {
								window.location = data.redirect_url;
							}
						}
					}
				);
			} else {
				/*Send passs*/
				$.ajax(
					{
						type: "POST",
						url: ajaxurl,
						dataType: 'json',
						context: this,
						data: $(this).serialize() + '&action=mvl_restore_password&security=' + stm_security_nonce,
						beforeSend: function () {
							$(this).find('input').removeClass('form-error');
							$(this).find('.stm-listing-loader').addClass('visible');
							$('.stm-validation-message').empty();
						},
						success: function (data) {
							$(this).find('.stm-listing-loader').removeClass('visible');
							if (data.message) {
								var message = $('<div class="stm-message-ajax-validation heading-font">' + data.message + '</div>').hide();

								$(this).find('.stm-validation-message').append(message);
								message.slideDown('fast');
							}
							for (var err in data.errors) {
								$(this).find('input[name=' + err + ']').addClass('form-error');
							}
						}
					}
				);
			}
		}
	);

	$('.user_validated_field').on(
		'hover',
		function () {
			$(this).removeClass('form-error');
		}
	);

	$('input[name="stm_accept_terms"]').on(
		'click',
		function () {
			if ($(this).is(':checked')) {
				$('.stm-login-register-form .stm-register-form form input[type="submit"]').removeAttr('disabled');
			} else {
				$('.stm-login-register-form .stm-register-form form input[type="submit"]').attr('disabled', '1');
			}
		}
	);
});

