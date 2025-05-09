// Write your Javascript code.

$(function () {
    // Wait for ABP to be fully initialized
    $(document).on('abp.configurationInitialized', function () {
        initializeCartFunctionality();
    });

    // Fallback initialization if the event doesn't fire within 1 second
    setTimeout(function () {
        if (typeof acme !== 'undefined' && acme.productSelling && acme.productSelling.carts) {
            initializeCartFunctionality();
        }
    }, 1000);

    function initializeCartFunctionality() {
        // Only initialize once
        if (window.cartInitialized) {
            return;
        }
        window.cartInitialized = true;


        var cartService = acme.productSelling.carts.cart;
        var localizationResource = abp.localization.getResource('ProductSelling');

        // Debug information

        // Check authentication status directly from cookies or other sources
        var isUserLoggedIn = false;

        // Method 1: Check ABP
        if (abp && abp.currentUser) {
            isUserLoggedIn = !!abp.currentUser.isAuthenticated;
        }

        // Method 2: Check if user section exists in DOM
        if ($('a[href="/Account/Logout"]').length > 0) {
            isUserLoggedIn = true;
        }

        // Method 3: Check for authentication cookie presence (simplified)
        if (document.cookie.indexOf('.AspNetCore.') >= 0) {
            isUserLoggedIn = true;
        }


        // Initialize cart widget based on determined authentication status
        if (isUserLoggedIn) {
            updateCartWidgetCount();
        } else {
            console.log("User not authenticated");
        }

        // Add to cart button functionality User.Identities.Any(i => i.IsAuthenticated)
        $('body').on('click', '.add-to-cart-button', function (e) {
            e.preventDefault();
            var $button = $(this);

            // Force check authentication status again
            var currentAuthStatus = isUserLoggedIn;
            if (abp && abp.currentUser) {
                currentAuthStatus = !!abp.currentUser.isAuthenticated;
            }


            if (!currentAuthStatus) {
                var returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                window.location.href = (abp && abp.appPath ? abp.appPath : '/') + 'Account/Login?ReturnUrl=' + returnUrl;
                return;
            }

            var productId = $button.data('product-id');

            var quantityInputId = '#quantity-for-' + productId;
            var $quantityInput = $(quantityInputId);

            if ($quantityInput.length === 0) {
                $quantityInput = $button.closest('.product-item, .product-detail').find('input[type="number"]');
            }

            var quantity = 1;
            if ($quantityInput.length > 0) {
                quantity = parseInt($quantityInput.val() || '1');
            }

            if (!productId || quantity <= 0) {
                abp.notify.warn(localizationResource('InvalidProductOrQuantity') || 'Invalid product or quantity');
                return;
            }

            $button.prop('disabled', true).addClass('disabled');
            var originalButtonHtml = $button.html();
            $button.html('<i class="fas fa-spinner fa-spin me-1"></i>' + (localizationResource('AddingToCart') || 'Adding...'));

            try {
                if (!cartService || typeof cartService.addItem !== 'function') {
                    abp.notify.error("Cart service not available");
                    $button.prop('disabled', false).removeClass('disabled');
                    $button.html(originalButtonHtml);
                    return;
                }

                cartService.addItem({
                    productId: productId,
                    quantity: quantity
                }, {
                    success: function () {
                        abp.notify.success(localizationResource('ItemAddedToCart') || 'Item added to cart');
                        updateCartWidgetCount();
                    },
                    error: function (error) {
                        abp.notify.error(error.message || localizationResource('CouldNotAddItemToCart') || 'Could not add item to cart');
                        console.error("Add to cart error:", error);
                    },
                    complete: function () {
                        $button.prop('disabled', false).removeClass('disabled');
                        $button.html(originalButtonHtml);
                    }
                });
            } catch (error) {
                abp.notify.error("Error adding item to cart");
                $button.prop('disabled', false).removeClass('disabled');
                $button.html(originalButtonHtml);
            }
        });
    }

    function updateCartWidgetCount() {
        var cartService = acme.productSelling.carts.cart;
        var $cartButton = $('a[href="/cart"]');
        var $countElement = $cartButton.find('.badge');
        var logoutBtn = document.getElementById('logoutButton');
        if ($cartButton.length === 0) {
            return;
        }

        if ($countElement.length === 0) {
            $countElement = $('<span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">0</span>');
            $cartButton.append($countElement);
        }


        // Force check authentication again
        var isUserLoggedIn = false;
        if (abp && abp.currentUser) {
            isUserLoggedIn = !!abp.currentUser.isAuthenticated;
        }

        // Also check if user section exists in DOM as backup 
        if (logoutBtn) {
            isUserLoggedIn = true;
        }

        if (isUserLoggedIn) {
            try {
                if (!cartService || typeof cartService.getItemCount !== 'function') {
                    return;
                }

                cartService.getItemCount({
                    success: function (count) {
                        $countElement.text(count);
                        if (count > 0) {
                            $countElement.removeClass('d-none');
                        } else {
                            $countElement.addClass('d-none');
                        }
                    },
                    error: function (error) {
                        $countElement.addClass('d-none');
                    }
                });
            } catch (error) {
                $countElement.addClass('d-none');
            }
        } else {
            $countElement.text('0').addClass('d-none');
        }
    }
});