/* =============================================================
   cart.widget.js
   Purpose: Updates the header/menu cart badge counts
   Depends on: cart.utils.js
   ============================================================= */

(function ($) {

    function updateCartWidgetCount() {
        // Check Auth using Utils
        if (!window.CartUtils || !window.CartUtils.isUserAuthenticated()) {
            return;
        }

        var $cartBadges = $('#cartBadge, #cartBadgeMobile');
        var cartService = window.CartUtils.getCartService();

        if (!cartService) {
            console.warn('Cart widget: Service not available yet.');
            return;
        }

        // 2. Fetch Count
        cartService.getItemCount()
            .then(function (count) {
                var itemCount = count || 0;
                $cartBadges.text(itemCount);

                if (itemCount > 0) {
                    $cartBadges.removeClass('d-none');
                } else {
                    $cartBadges.addClass('d-none');
                }
            })
            .catch(function (error) {
                console.error("Widget Error: Could not get cart count", error);
                $cartBadges.addClass('d-none');
            });
    }

    // Expose this function globally so 'cart.actions.js' can call it after adding an item
    window.updateCartWidgetCount = updateCartWidgetCount;

})(jQuery);