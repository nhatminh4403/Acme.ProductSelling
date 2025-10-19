$(function () {

    window.cartInitialized = false;

    $(document).on('abp.configurationInitialized', function () {
        tryInitializeCart();
    });

    $(document).ready(function () {
        waitForABP();
    });

    var initAttempts = 0;
    var maxAttempts = 20;

    function waitForABP() {
        if (typeof abp !== 'undefined' &&
            abp.localization &&
            typeof acme !== 'undefined' &&
            acme.productSelling &&
            acme.productSelling.carts) {
            tryInitializeCart();
            return;
        }

        initAttempts++;
        if (initAttempts < maxAttempts) {
            setTimeout(waitForABP, 500);
        } else {
            console.warn('ABP initialization timeout - some features may not work');
            tryInitializeCart();
        }
    }

    function tryInitializeCart() {
        if (window.cartInitialized) {
            return;
        }

        if (typeof $ === 'undefined') {
            console.error('jQuery is not loaded');
            return;
        }

        initializeCartFunctionality();
    }

    function initializeCartFunctionality() {
        if (window.cartInitialized) {
            return;
        }
        window.cartInitialized = true;

        var cartService = null;
        var localizationResource = null;

        try {
            if (typeof acme !== 'undefined' &&
                acme.productSelling &&
                acme.productSelling.carts &&
                acme.productSelling.carts.cart) {
                cartService = acme.productSelling.carts.cart;
            }

            if (typeof abp !== 'undefined' && abp.localization) {
                localizationResource = abp.localization.getResource('ProductSelling');
            } else {
                localizationResource = function (key) {
                    var fallbacks = {
                        'Cart:InvalidProduct': 'Invalid product',
                        'Cart:AddingToCart': 'Adding...',
                        'Cart:CartServiceNotAvailable': 'Cart service not available',
                        'Cart:ItemAddedToCart': 'Item added to cart',
                        'Cart:CouldNotAddItemToCart': 'Could not add item to cart'
                    };
                    return fallbacks[key] || key;
                };
            }
        } catch (error) {
            console.error('Error accessing services:', error);
        }

        function isUserAuthenticated() {
            try {
                if (typeof abp !== 'undefined' &&
                    abp.currentUser &&
                    abp.currentUser.isAuthenticated) {
                    return true;
                }

                if ($('a[href="/Account/Logout"]').length > 0) {
                    return true;
                }

                if ($('#logoutButton').length > 0) {
                    return true;
                }

                if (document.cookie.indexOf('.AspNetCore.') >= 0) {
                    return true;
                }
            } catch (error) {
                console.error('Error checking authentication:', error);
            }

            return false;
        }

        if (isUserAuthenticated()) {
            updateCartWidgetCount();
        }

        $('body').off('click.cart').on('click.cart', '.add-to-cart-button', function (e) {
            e.preventDefault();
            var $button = $(this);

            if (!isUserAuthenticated()) {
                var returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                var basePath = '';

                try {
                    basePath = (typeof abp !== 'undefined' && abp.appPath) ? abp.appPath : '/';
                } catch (error) {
                    basePath = '/';
                }

                var loginUrl = basePath + 'Account/Login?ReturnUrl=' + returnUrl;
                window.location.href = loginUrl;
                return;
            }

            var productId = $button.data('product-id');
            if (!productId) {
                showNotification('warn', localizationResource('Cart:InvalidProduct'));
                return;
            }

            var quantityInputId = '#quantity-for-' + productId;
            var $quantityInput = $(quantityInputId);

            if ($quantityInput.length === 0) {
                $quantityInput = $button.closest('.product-item, .product-detail').find('input[type="number"]');
            }

            var quantity = 1;
            if ($quantityInput.length > 0) {
                quantity = parseInt($quantityInput.val() || '1');
                if (isNaN(quantity) || quantity <= 0) {
                    quantity = 1;
                }
            }

            $button.prop('disabled', true).addClass('disabled');
            var originalButtonHtml = $button.html();
            $button.html('<i class="fas fa-spinner fa-spin me-1"></i>' + localizationResource('Cart:AddingToCart'));

            function resetButton() {
                $button.prop('disabled', false).removeClass('disabled');
                $button.html(originalButtonHtml);
            }

            if (!cartService || typeof cartService.addItem !== 'function') {
                showNotification('error', localizationResource('Cart:CartServiceNotAvailable'));
                resetButton();
                return;
            }

            cartService.addItem({
                ProductId: productId,
                Quantity: parseInt(quantity)
            }).then(function (result) {
                showNotification('success', localizationResource('Cart:ItemAddedToCart'));
                updateCartWidgetCount();
                resetButton();
            }).catch(function (error) {
                var errorMessage = localizationResource('Cart:CouldNotAddItemToCart');

                if (error && error.message) {
                    errorMessage = error.message;
                } else if (error && error.error && error.error.message) {
                    errorMessage = error.error.message;
                } else if (error.response && error.response.data && error.response.data.error) {
                    errorMessage = error.response.data.error.message || errorMessage;
                }

                showNotification('error', errorMessage);
                resetButton();
            });
        });

        function showNotification(type, message) {
            try {
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify[type](message);
                } else {
                    if (type === 'error') {
                        alert('Error: ' + message);
                    } else if (type === 'warn') {
                        alert('Warning: ' + message);
                    } else {
                        alert(message);
                    }
                }
            } catch (error) {
                console.error('Error showing notification:', error);
            }
        }
    }

    function updateCartWidgetCount() {
        try {
            var cartService = null;

            if (typeof acme !== 'undefined' &&
                acme.productSelling &&
                acme.productSelling.carts &&
                acme.productSelling.carts.cart) {
                cartService = acme.productSelling.carts.cart;
            }

            // FIXED: Target both desktop and mobile cart buttons
            var $cartButtons = $('#shopping-cart-widget, #shopping-cart-widget-mobile');

            if ($cartButtons.length === 0) {
                console.warn('Cart buttons not found');
                return;
            }

            console.log('Found', $cartButtons.length, 'cart button(s)');

            function isUserAuthenticated() {
                try {
                    if (typeof abp !== 'undefined' &&
                        abp.currentUser &&
                        abp.currentUser.isAuthenticated) {
                        return true;
                    }

                    if ($('#logoutButton').length > 0 || $('a[href="/Account/Logout"]').length > 0) {
                        return true;
                    }
                } catch (error) {
                    console.error('Error checking authentication in updateCartWidgetCount:', error);
                }

                return false;
            }

            if (!isUserAuthenticated()) {
                console.log('User not authenticated, hiding badges');
                $cartButtons.each(function () {
                    var $badge = $(this).find('.cart-item-count');
                    if ($badge.length > 0) {
                        $badge.text('0').addClass('d-none');
                    }
                });
                return;
            }

            if (!cartService || typeof cartService.getItemCount !== 'function') {
                console.warn('Cart service not available');
                $cartButtons.each(function () {
                    var $badge = $(this).find('.cart-item-count');
                    if ($badge.length > 0) {
                        $badge.addClass('d-none');
                    }
                });
                return;
            }

            cartService.getItemCount().then(function (count) {
                console.log('Cart count retrieved:', count);

                // Update both desktop and mobile badges
                $cartButtons.each(function () {
                    var $cartButton = $(this);
                    var $badge = $cartButton.find('.cart-item-count');

                    // If badge doesn't exist, create it
                    if ($badge.length === 0) {
                        console.log('Creating badge for', $cartButton.attr('id'));
                        $badge = $('<span class="cart-item-count position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">0</span>');
                        $cartButton.append($badge);
                    }

                    // Update badge
                    $badge.text(count || 0);
                    if (count && count > 0) {
                        $badge.removeClass('d-none');
                    } else {
                        $badge.addClass('d-none');
                    }
                });
            }).catch(function (error) {
                console.error("Get cart count error:", error);
                $cartButtons.each(function () {
                    var $badge = $(this).find('.cart-item-count');
                    if ($badge.length > 0) {
                        $badge.addClass('d-none');
                    }
                });
            });
        } catch (error) {
            console.error('Error in updateCartWidgetCount:', error);
        }
    }

    window.updateCartWidgetCount = updateCartWidgetCount;
});