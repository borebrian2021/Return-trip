(function($) {
    $(document).on('click', '#buy-car-online', function(e) {
        e.preventDefault();

        var thisBtn = $(this);

        var carId = $(this).data('id');

        $.ajax({
            url: ajaxurl,
            type: "POST",
            dataType: 'json',
            context: this,
            data: 'car_id=' + carId + '&action=stm_ajax_buy_car_online&security=' + stm_security_nonce,
            beforeSend: function () {
                thisBtn.addClass('buy-online-load');
            },
            success: function (data) {

                thisBtn.removeClass('buy-online-load');

                if (data.status == 'success') {
                    window.location = data.redirect_url;
                }
            }
        });
    });

	function sortByListingType() {
		$( 'body' ).on(
			'change',
			'.select-listing-type select',
			function () {
				let $select       = $( this );
				let listingType   = $select.val();
				let viewType      = $( '#stm_user_dealer_view_type' ).val();
				let listingsView  = $( '#stm-dealer-view-type' ).val();
				let userId        = $select.data( 'user' );
				let popular       = listingsView === 'popular' ? 'yes' : 'no';
				let userPublic    = $select.data( 'user-public' );
				let userPrivate   = $select.data( 'user-private' );
				let userFavourite = $select.data( 'user-favourite' );
				let postsPerPage  = parseInt( $select.data( 'posts-per-page' ), 10 );
				let offset        = parseInt( $select.data( 'offset' ), 10 );
				if ( ! postsPerPage ) {
					postsPerPage = parseInt( $( '#sort_by_select' ).data( 'posts-per-page' ), 10 ) || -1;
				}
				if ( userPrivate ) {
					viewType = 'list';
				} else if ( ! viewType ) {
					viewType = 'grid';
				}
				if ( offset === 0 ) {
					offset = postsPerPage;
				}

				$.ajax(
					{
						url: ajaxurl,
						data: {
							action: 'mvl_ajax_dealer_load_listings_by_type',
							listing_type: listingType,
							user_id: userId,
							popular: popular,
							view_type: viewType,
							user_public: userPublic,
							user_private: userPrivate,
							user_favourite: userFavourite,
							posts_per_page: postsPerPage,
							offset: offset,
							security: stm_security_nonce
						},
						method: 'POST',
						dataType: 'json',
						beforeSend: function () {
							$( '.select-listing-type select' ).prop( "disabled", true );
						},
						success: function (data) {
							$( '.select-listing-type select' ).prop( "disabled", false );
							if (data) {
								const $target = userPrivate || userFavourite ? $( '.archive-listing-page .car-listing-row' ) :
								userPublic ? $( '.archive-listing-page' ) :
								$( '#' + listingsView ).find( '.car-listing-row' );
								$target.html( data.html );
							}

							const $loadMoreButton = $( '#' + listingsView ).find( '.stm-load-more-dealer-cars' );
							$loadMoreButton.find('a').attr('data-listing-type', listingType);
							$loadMoreButton.find('a').attr('data-offset', offset);

							if (data.button === 'show') {
								$loadMoreButton.show();
							} else {
								$loadMoreButton.hide();
							}

							$( 'img.lazy' ).lazyload(
								{
									effect: "fadeIn",
									failure_limit: Math.max( $( 'img' ).length - 1, 0 )
								}
							);

						}
					}
				);
			}
		);
	}


    function loadMoreDealerCars() {
		$('body').on('click', '.stm-load-more-dealer-cars a', function (e) {
			e.preventDefault()

			if (
				$(this)
					.closest('.stm-load-more-dealer-cars')
					.hasClass('not-clickable')
			) {
				return false
			}

			var offset = $(this).attr('data-offset')
			var user_id = $(this).data('user')
			var popular = $(this).data('popular')
			var profile_page = $(this).data('profile');
			var view_type = $('#stm_user_dealer_view_type').val()
			var listing_type = $(this).attr('data-listing-type');

			$.ajax({
				url: ajaxurl,
				data: {
					action: 'mvl_ajax_dealer_load_cars',
					offset: offset,
					user_id: user_id,
					popular: popular,
					view_type: view_type,
					profile_page: profile_page,
					security: stm_security_nonce,
					listing_type: listing_type,
				},
				method: 'POST',
				dataType: 'json',
				context: this,
				beforeSend: function () {
					$(this)
						.closest('.stm-load-more-dealer-cars')
						.addClass('not-clickable')
				},
				success: function (data) {
					$(this)
						.closest('.stm-load-more-dealer-cars')
						.removeClass('not-clickable')
					if (data.html) {
						$(this)
							.closest('.tab-pane')
							.find('.car-listing-row')
							.append(data.html)
						$(this)
							.closest('.stm-user-public-listing')
							.find('.car-listing-row')
							.append(data.html)
						$(this)
							.closest('.stm-user-private-main')
							.find('.car-listing-row')
							.append(data.html)
					}
					if (data.new_offset) {
						$(this).attr('data-offset', data.new_offset)
					}
					if (data.button == 'hide') {
						$(this).closest('.stm-load-more-dealer-cars').slideUp()
						$(this)
							.closest('.tab-pane')
							.find('.row-no-border-last')
							.removeClass('row-no-border-last')
					}
				},
			})
		})
	}
	loadMoreDealerCars();
	sortByListingType();
    if (window.location.search.includes('add-to-cart')) {
        var newUrl = window.location.origin + window.location.pathname;
        window.history.replaceState({}, document.title, newUrl);
    }

	$(document).on('change', '#sort_by_select', function () {
		var sortBy = $(this).val()
		var user_id = $(this).data('user')
		var posts_per_page = $(this).data('posts-per-page')
		var listing_type = $('.select-listing-type select').val() || 'all'
		var data = {
			action: 'stm_sort_listings',
			sort_by: sortBy,
			user_id: user_id,
			security: stm_security_nonce,
			page: 1,
			posts_per_page: posts_per_page,
			listing_type: listing_type,
		}
		$.ajax({
			url: ajaxurl,
			type: 'POST',
			data: data,
			success: function (response) {
				if (response.success) {
					$('.car-listing-row').html(response.data.listings_html)
					$('.pagination-container').html(response.data.pagination_html)
					$('#sort_by_select').data('page', 1)

					if (response.data.show_more_button) {
						$('.stm-load-more-dealer-cars').show()
					} else {
						$('.stm-load-more-dealer-cars').hide()
					}
				}
			},
		})
	})

	$(document).on('click', '.pagination-container a', function (e) {
		e.preventDefault()
		var page = $(this).attr('href').split('page=')[1]
		var sortBy = $('#sort_by_select').val()
		var user_id = $('#sort_by_select').data('user')
		var posts_per_page = $('#sort_by_select').data('posts-per-page')
		var listing_type = $('.select-listing-type select').val() || 'all'
		var data = {
			action: 'stm_sort_listings',
			sort_by: sortBy,
			user_id: user_id,
			page: page,
			posts_per_page: posts_per_page,
			listing_type: listing_type,
			security: stm_security_nonce,
		}
		$.ajax({
			url: ajaxurl,
			type: 'POST',
			data: data,
			success: function (response) {
				if (response.success) {
					$('.car-listing-row').html(response.data.listings_html)
					$('.pagination-container').html(response.data.pagination_html)
					$('#sort_by_select').data('page', page)
				}
			},
		})
	})

    $(document).on('submit', '#stm_new_password', function() {
        var password = $(this).val();
        var messageDiv = $(this).closest('.form-group').find('.stm-validation-message');
        
        $.ajax({
            url: ajaxurl,
            type: 'POST',
            data: {
                action: 'stm_validate_password',
                password: password,
                security: stm_security_nonce
            },
            success: function(response) {
                if (!response.valid) {
                    messageDiv.text(response.message).show();
                } else {
                    messageDiv.hide();
                }
            }
        });
    });

    $(document).on('submit', '.stm_password_recovery', function(e) {
        var password = $('#stm_new_password').val();
		var messageDiv = $(this).find('.stm-validation-message');
        
        if (password.length < 8) {
            e.preventDefault();
            messageDiv.text(stm_i18n.mvl_password_validation).show();
            return false;
        }
        
        return true;
    });

})(jQuery)
