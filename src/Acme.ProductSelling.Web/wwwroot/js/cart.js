$(function () {
    // Global flag to prevent multiple initializations
    window.cartInitialized = false;

    // Multiple initialization strategies to ensure script works

    // Strategy 1: Listen for ABP configuration initialized event
    $(document).on('abp.configurationInitialized', function () {
        console.log('ABP configuration initialized - attempting cart initialization');
        tryInitializeCart();
    });

    // Strategy 2: Listen for document ready and ABP ready
    $(document).ready(function () {
        console.log('Document ready - checking for ABP');
        waitForABP();
    });

    // Strategy 3: Fallback timer-based initialization
    var initAttempts = 0;
    var maxAttempts = 20; // Try for up to 10 seconds (20 * 500ms)

    function waitForABP() {
        if (typeof abp !== 'undefined' &&
            abp.localization &&
            typeof acme !== 'undefined' &&
            acme.productSelling &&
            acme.productSelling.carts) {

            console.log('ABP and services are ready');
            tryInitializeCart();
            return;
        }

        initAttempts++;
        if (initAttempts < maxAttempts) {
            console.log(`Waiting for ABP... attempt ${initAttempts}/${maxAttempts}`);
            setTimeout(waitForABP, 500);
        } else {
            console.warn('ABP initialization timeout - some features may not work');
            // Still try to initialize with basic functionality
            tryInitializeCart();
        }
    }

    function tryInitializeCart() {
        if (window.cartInitialized) {
            console.log('Cart already initialized, skipping');
            return;
        }

        // Check if minimum requirements are met
        if (typeof $ === 'undefined') {
            console.error('jQuery is not loaded');
            return;
        }

        console.log('Initializing cart functionality');
        initializeCartFunctionality();
    }

    function initializeCartFunctionality() {
        // Prevent multiple initialization
        if (window.cartInitialized) {
            return;
        }
        window.cartInitialized = true;

        console.log('Cart functionality initialized');

        // Safe service access with fallbacks
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
                // Fallback localization function
                localizationResource = function (key) {
                    var fallbacks = {
                        'InvalidProduct': 'Invalid product',
                        'AddingToCart': 'Adding...',
                        'CartServiceNotAvailable': 'Cart service not available',
                        'ItemAddedToCart': 'Item added to cart',
                        'CouldNotAddItemToCart': 'Could not add item to cart'
                    };
                    return fallbacks[key] || key;
                };
            }
        } catch (error) {
            console.error('Error accessing services:', error);
        }

        function isUserAuthenticated() {
            try {
                // Primary check: ABP current user
                if (typeof abp !== 'undefined' &&
                    abp.currentUser &&
                    abp.currentUser.isAuthenticated) {
                    return true;
                }

                // Fallback checks
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

        // Initialize cart count if user is authenticated
        if (isUserAuthenticated()) {
            updateCartWidgetCount();
        }

        // Event handler for add to cart buttons
        $('body').off('click.cart').on('click.cart', '.add-to-cart-button', function (e) {
            e.preventDefault();
            var $button = $(this);

            console.log('Add to cart button clicked');

            // Check authentication status
            if (!isUserAuthenticated()) {
                console.log('User not authenticated, redirecting to login');
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
                console.warn('No product ID found');
                showNotification('warn', localizationResource('InvalidProduct'));
                return;
            }

            // Get quantity
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

            // Disable button and show loading state
            $button.prop('disabled', true).addClass('disabled');
            var originalButtonHtml = $button.html();
            $button.html('<i class="fas fa-spinner fa-spin me-1"></i>' + localizationResource('AddingToCart'));

            function resetButton() {
                $button.prop('disabled', false).removeClass('disabled');
                $button.html(originalButtonHtml);
            }

            // Validate cart service
            if (!cartService || typeof cartService.addItem !== 'function') {
                console.error('Cart service not available');
                showNotification('error', localizationResource('CartServiceNotAvailable'));
                resetButton();
                return;
            }

            console.log('Adding item to cart:', { ProductId: productId, Quantity: quantity });

            cartService.addItem({
                ProductId: productId,
                Quantity: parseInt(quantity)
            }).then(function (result) {
                console.log('Item added to cart successfully:', result);
                showNotification('success', localizationResource('ItemAddedToCart'));
                updateCartWidgetCount();
                resetButton();
            }).catch(function (error) {
                console.error("Add to cart error:", error);

                var errorMessage = localizationResource('CouldNotAddItemToCart');

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

        // Helper function for notifications
        function showNotification(type, message) {
            try {
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify[type](message);
                } else {
                    // Fallback to alert or console
                    if (type === 'error') {
                        alert('Error: ' + message);
                    } else if (type === 'warn') {
                        alert('Warning: ' + message);
                    } else {
                        alert(message);
                    }
                    console.log(type.toUpperCase() + ':', message);
                }
            } catch (error) {
                console.error('Error showing notification:', error);
                console.log(type.toUpperCase() + ':', message);
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

            var $cartButton = $('a[href="/cart"]');
            var $countElement = $cartButton.find('.badge');

            if ($cartButton.length === 0) {
                console.log('Cart button not found');
                return;
            }

            if ($countElement.length === 0) {
                $countElement = $('<span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">0</span>');
                $cartButton.append($countElement);
            }

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
                $countElement.text('0').addClass('d-none');
                return;
            }

            if (!cartService || typeof cartService.getItemCount !== 'function') {
                console.log('Cart service or getItemCount not available');
                $countElement.addClass('d-none');
                return;
            }

            cartService.getItemCount().then(function (count) {
                console.log('Cart count updated:', count);
                $countElement.text(count || 0);
                if (count && count > 0) {
                    $countElement.removeClass('d-none');
                } else {
                    $countElement.addClass('d-none');
                }
            }).catch(function (error) {
                console.error("Get cart count error:", error);
                $countElement.addClass('d-none');
            });
        } catch (error) {
            console.error('Error in updateCartWidgetCount:', error);
        }
    }

    // Expose updateCartWidgetCount globally for other scripts to use
    window.updateCartWidgetCount = updateCartWidgetCount;
});