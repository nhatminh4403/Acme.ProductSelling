/* =============================================================
   cart.actions.js
   Purpose: "Add to Cart" button listeners and App Initialization
   Depends on: cart.utils.js, cart.widget.js
   ============================================================= */

$(function () {
    //Initialization Logic (Waits for ABP)
    var initAttempts = 0;

    function waitForABP() {
        var cartService = window.CartUtils.getCartService();

        if (cartService && window.updateCartWidgetCount) {
            // Everything is ready
            init();
        } else {
            // Still loading
            initAttempts++;
            if (initAttempts < 50) {
                setTimeout(waitForABP, 100);
            } else {
                console.warn('ABP Timeout. Initializing partial features.');
                init();
            }
        }
    }

    // Start waiting when DOM is ready
    waitForABP();

    function init() {
        console.log("Cart System Initialized");

        // Update the widget on page load
        if (window.updateCartWidgetCount) {
            window.updateCartWidgetCount();
        }

        // Attach event listeners
        attachClickListeners();
    }


    //Button Event Listeners
    function attachClickListeners() {
        $('body').off('click.cart').on('click.cart', '.add-to-cart-button', function (e) {
            e.preventDefault();
            var $button = $(this);

            // Auth Check
            if (!window.CartUtils.isUserAuthenticated()) {
                var returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                var basePath = (typeof abp !== 'undefined' && abp.appPath) ? abp.appPath : '/';
                window.location.href = basePath + 'Account/Login?ReturnUrl=' + returnUrl;
                return;
            }

            // Get Data
            var productId = $button.data('product-id');
            var cartService = window.CartUtils.getCartService();

            if (!productId || !cartService) return;

            // Get Quantity
            var $quantityInput = $('#quantity-for-' + productId);
            if ($quantityInput.length === 0) {
                $quantityInput = $button.closest('.product-item, .product-detail').find('input[type="number"]');
            }
            var quantity = parseInt($quantityInput.val() || '1');

            // UI Loading
            $button.prop('disabled', true).addClass('disabled');
            var originalHtml = $button.html();
            var loadingText = window.CartUtils.getLocalizedString('Cart:AddingToCart', 'Adding...');
            $button.html('<i class="fas fa-spinner fa-spin me-1"></i> ' + loadingText);

            // API Call
            cartService.addItemToCart({
                ProductId: productId,
                Quantity: quantity
            }).then(function () {
                var successMsg = window.CartUtils.getLocalizedString('Cart:ItemAddedToCart', 'Added to cart');
                window.CartUtils.notify('success', successMsg);

                // Refresh Badge
                if (window.updateCartWidgetCount) window.updateCartWidgetCount();

                // Reset Button
                $button.prop('disabled', false).removeClass('disabled').html(originalHtml);
            }).catch(function (err) {
                window.CartUtils.notify('error', 'Failed to add item');
                $button.prop('disabled', false).removeClass('disabled').html(originalHtml);
            });
        });
    }
});