/* =============================================================
   cart.utils.js
   Purpose: Shared helper functions (Auth check, Service getter)
   ============================================================= */

window.CartUtils = (function ($) {
    return {
        // Centralized Authentication Check
        isUserAuthenticated: function () {
            try {
                // 1. Check ABP global object
                if (typeof abp !== 'undefined' && abp.currentUser && abp.currentUser.isAuthenticated) {
                    return true;
                }
                // 2. Check for User Dropdown HTML ID
                if ($('#userDropdown').length > 0) {
                    return true;
                }
                // 3. Check for Logout link (Partial match)
                if ($('a[href*="/Account/Logout"]').length > 0) {
                    return true;
                }
                // 4. Cookie Fallback
                if (document.cookie.indexOf('.AspNetCore.Identity.Application') !== -1) {
                    return true;
                }
            } catch (error) {
                console.error('Error checking authentication:', error);
            }
            return false;
        },

        // Safe way to get the Proxy Service
        getCartService: function () {
            try {
                if (typeof acme !== 'undefined' &&
                    acme.productSelling &&
                    acme.productSelling.carts &&
                    acme.productSelling.carts.cart) {
                    return acme.productSelling.carts.cart;
                }
            } catch (e) {
                console.warn("Service namespace not found");
            }
            return null;
        },

        // Helper for notifications
        notify: function (type, message) {
            if (typeof abp !== 'undefined' && abp.notify) {
                abp.notify[type](message);
            } else {
                console.log(type + ': ' + message);
            }
        },

        // Helper to get localization strings
        getLocalizedString: function (key, fallback) {
            if (typeof abp !== 'undefined' && abp.localization) {
                return abp.localization.getResource('ProductSelling')(key);
            }
            return fallback || key;
        }
    };
})(jQuery);