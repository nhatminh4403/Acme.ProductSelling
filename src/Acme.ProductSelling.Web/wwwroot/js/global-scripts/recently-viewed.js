//Depends on: localization.js, notification.js
const RecentlyViewedManager = (function () {
    'use strict';

    // -------------------------------------------------------------------------
    // Constants
    // -------------------------------------------------------------------------
    const STORAGE_KEY = 'Acme_RecentlyViewed_Ids';
    const MAX_GUEST_ITEMS = 10;
    const REQUEST_COOLDOWN_MS = 1000;   // minimum gap between server requests
    const SYNC_COOLDOWN_MS = 5000;   // minimum gap between sync attempts

    // -------------------------------------------------------------------------
    // Private state
    // -------------------------------------------------------------------------
    let lastRequestTime = 0;
    let isSyncing = false;
    let lastSyncTime = 0;
    let pendingRequest = null;         // tracks in-flight loadWidget promise

    // -------------------------------------------------------------------------
    // localStorage helpers
    // -------------------------------------------------------------------------
    function getGuestIds() {
        try {
            const json = localStorage.getItem(STORAGE_KEY);
            if (!json) return [];

            const ids = JSON.parse(json);
            if (!Array.isArray(ids)) {
                console.warn('[RecentlyViewed] Invalid data structure, resetting');
                localStorage.removeItem(STORAGE_KEY);
                return [];
            }

            return ids.slice(0, MAX_GUEST_ITEMS);
        } catch (e) {
            console.warn('[RecentlyViewed] Failed to read storage:', e);
            try { localStorage.removeItem(STORAGE_KEY); } catch (_) { /* ignore */ }
            return [];
        }
    }

    function saveGuestIds(ids) {
        if (!Array.isArray(ids)) {
            console.error('[RecentlyViewed] saveGuestIds expects an array');
            return;
        }
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(ids.slice(0, MAX_GUEST_ITEMS)));
        } catch (e) {
            console.warn('[RecentlyViewed] Failed to save storage:', e);
        }
    }

    function clearGuestIds() {
        try { localStorage.removeItem(STORAGE_KEY); } catch (e) {
            console.warn('[RecentlyViewed] Failed to clear storage:', e);
        }
    }

    // -------------------------------------------------------------------------
    // ABP helpers
    // -------------------------------------------------------------------------
    function isAuthenticated() {
        return !!(abp && abp.currentUser && abp.currentUser.isAuthenticated);
    }

    /**
     * Returns the ABP dynamic proxy for the RecentlyViewedProduct app service.
     * The proxy is generated automatically by ABP from the server-side interface.
     *
     * @returns {object|null}
     */
    function getApiProxy() {
        return (acme &&
            acme.productSelling &&
            acme.productSelling.products &&
            acme.productSelling.products.recentlyViewedProduct)
            ? acme.productSelling.products.recentlyViewedProduct
            : null;
    }

    function canMakeRequest() {
        const now = Date.now();
        if (now - lastRequestTime < REQUEST_COOLDOWN_MS) return false;
        lastRequestTime = now;
        return true;
    }

    // -------------------------------------------------------------------------
    // Rendering helpers
    // -------------------------------------------------------------------------
    function formatCurrency(value) {
        if (typeof value !== 'number' || isNaN(value)) return '0 ₫';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
            maximumFractionDigits: 0,
        }).format(value);
    }

    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function renderProductCard(product) {
        if (!product || !product.productId) {
            console.warn('[RecentlyViewed] Invalid product data, skipping card');
            return '';
        }

        const hasDiscount = product.discountedPrice && product.discountedPrice < product.originalPrice;

        const priceHtml = hasDiscount
            ? `<div class="rv-price-group">
                   <span class="rv-price-old">${formatCurrency(product.originalPrice)}</span>
                   <span class="rv-price-current">${formatCurrency(product.discountedPrice)}</span>
                   <span class="rv-discount-badge">-${product.discountPercent || 0}%</span>
               </div>`
            : `<div class="rv-price-group">
                   <span class="rv-price-current text-primary">${formatCurrency(product.originalPrice)}</span>
               </div>`;

        const stockHtml = product.isAvailableForPurchase ? '' : '<div class="rv-stock-badge">Hết hàng</div>';
        const safeImgSrc = product.imageUrl || '/images/placeholder.png';
        const urlSlug = product.urlSlug || '#';

        return `
        <div class="col-12 col-md-6 col-lg-3">
            <a href="/products/${urlSlug}" class="rv-card" title="${escapeHtml(product.productName)}">
                <div class="rv-img-wrapper">
                    ${stockHtml}
                    <img src="${safeImgSrc}"
                         alt="${escapeHtml(product.productName)}"
                         loading="lazy"
                         onerror="if(this.src!=='/images/placeholder.png'){this.src='/images/placeholder.png';}">
                </div>
                <div class="rv-content">
                    <div class="rv-title">${escapeHtml(product.productName)}</div>
                    ${priceHtml}
                </div>
            </a>
        </div>`;
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------
    return {

        /**
         * Record that the user viewed a product.
         * Updates localStorage immediately; notifies the server for logged-in users.
         *
         * @param {string} productId
         */
        trackView: function (productId) {
            if (!productId || typeof productId !== 'string') {
                console.warn('[RecentlyViewed] trackView received invalid productId:', productId);
                return;
            }

            try {
                let ids = getGuestIds();
                ids = ids.filter(function (id) { return id !== productId; });
                ids.unshift(productId);
                saveGuestIds(ids);

                if (isAuthenticated() && canMakeRequest()) {
                    const api = getApiProxy();
                    if (api && api.trackProductView) {
                        api.trackProductView(productId).catch(function (err) {
                            console.warn('[RecentlyViewed] Server trackView failed:', err);
                        });
                    }
                }
            } catch (e) {
                console.error('[RecentlyViewed] Error in trackView:', e);
            }
        },

        /**
         * Upload guest history to the server after login.
         * Safe to call multiple times — guarded by isSyncing + cooldown.
         *
         * @returns {Promise}
         */
        syncWithServer: function () {
            const now = Date.now();

            if (isSyncing) {
                console.log('[RecentlyViewed] Sync already in progress, skipping');
                return Promise.resolve();
            }

            if (now - lastSyncTime < SYNC_COOLDOWN_MS) {
                console.log('[RecentlyViewed] Sync cooldown active, skipping');
                return Promise.resolve();
            }

            const guestIds = getGuestIds();
            if (guestIds.length === 0) return Promise.resolve();
            if (!isAuthenticated()) return Promise.resolve();

            const api = getApiProxy();
            if (!api || !api.syncGuestHistory) return Promise.resolve();

            isSyncing = true;
            lastSyncTime = now;

            return api.syncGuestHistory({ productIds: guestIds })
                .then(function () {
                    clearGuestIds();
                    console.log('[RecentlyViewed] Sync successful');
                    isSyncing = false;
                })
                .catch(function (err) {
                    console.warn('[RecentlyViewed] Sync failed:', err);
                    isSyncing = false;
                });
        },

        /**
         * Load recently-viewed products and render them into a container.
         *
         * @param {string}   containerId       DOM id of the target element
         * @param {number}   [maxCount=6]
         * @param {string}   [excludeProductId]
         * @param {Function} [callback]        Called with true (has products) or false
         */
        loadWidget: function (containerId, maxCount, excludeProductId, callback) {
            maxCount = Math.min(parseInt(maxCount, 10) || 6, 20);

            if (!containerId || typeof containerId !== 'string') {
                console.error('[RecentlyViewed] loadWidget: invalid containerId');
                if (callback) callback(false);
                return;
            }

            const $container = $('#' + containerId);
            if ($container.length === 0) {
                if (callback) callback(false);
                return;
            }

            // Abandon any in-flight request token
            pendingRequest = null;

            let guestIds = getGuestIds();
            if (excludeProductId) {
                guestIds = guestIds.filter(function (id) { return id !== excludeProductId; });
            }

            if (guestIds.length === 0 && !isAuthenticated()) {
                if (callback) callback(false);
                return;
            }

            const api = getApiProxy();
            if (!api || !api.getList) {
                console.warn('[RecentlyViewed] API proxy not available');
                if (callback) callback(false);
                return;
            }

            if (!canMakeRequest()) {
                console.log('[RecentlyViewed] Request throttled');
                if (callback) callback(false);
                return;
            }

            const requestToken = {};     // identity object used to detect cancellation
            pendingRequest = requestToken;

            api.getList({
                maxCount: maxCount + 1,            // fetch one extra so we can exclude + trim
                guestProductIds: guestIds.slice(0, 20),
            })
                .then(function (products) {
                    if (pendingRequest !== requestToken) return;  // stale — a newer call cancelled us
                    pendingRequest = null;

                    if (!products || !Array.isArray(products) || products.length === 0) {
                        if (callback) callback(false);
                        return;
                    }

                    let filtered = excludeProductId
                        ? products.filter(function (p) { return p.productId !== excludeProductId; })
                        : products;

                    filtered = filtered.slice(0, maxCount);

                    if (filtered.length === 0) {
                        if (callback) callback(false);
                        return;
                    }

                    // Batch-render to minimise reflows
                    $container.html(filtered.map(renderProductCard).join(''));
                    if (callback) callback(true);
                })
                .catch(function (err) {
                    console.warn('[RecentlyViewed] loadWidget failed:', err);
                    pendingRequest = null;
                    if (callback) callback(false);
                });
        },

        /**
         * Clear all recently-viewed data for the current user.
         *
         * @returns {Promise}
         */
        clear: function () {
            clearGuestIds();

            if (isAuthenticated()) {
                const api = getApiProxy();
                if (api && api.clear) {
                    return api.clear().catch(function (err) {
                        console.warn('[RecentlyViewed] Server clear failed:', err);
                    });
                }
            }

            return Promise.resolve();
        },

        /** Exposed for debugging / testing */
        getGuestIds: getGuestIds,
    };

}());

window.RecentlyViewedManager = RecentlyViewedManager;

// =============================================================================
// RECENTLY VIEWED WIDGET INIT
// Runs once on DOMContentLoaded; reads config from data-attributes.
// =============================================================================
document.addEventListener('DOMContentLoaded', function () {
    if (window._recentlyViewedInitialized) return;
    window._recentlyViewedInitialized = true;

    const $wrapper = $('#recently-viewed-section');
    if ($wrapper.length === 0) return;

    if (typeof RecentlyViewedManager === 'undefined') {
        console.error('[RecentlyViewed] RecentlyViewedManager not loaded');
        return;
    }

    const maxCount = parseInt($wrapper.data('max-count'), 10) || 6;

    RecentlyViewedManager.loadWidget(
        'recently-viewed-products',
        (maxCount >= 1 && maxCount <= 20) ? maxCount : 6,
        $wrapper.data('exclude-product-id') || null,
        function (hasProducts) {
            $('#recently-viewed-skeleton').hide();
            if (hasProducts) {
                $('#recently-viewed-products').show();
                $wrapper.fadeIn(300);
            } else {
                $wrapper.hide();
            }
        }
    );

    let clearInProgress = false;

    $('#btn-clear-recently-viewed').on('click', function (e) {
        e.preventDefault();
        if (clearInProgress) return;
        if (!confirm('Bạn có chắc muốn xóa lịch sử sản phẩm đã xem?')) return;

        clearInProgress = true;

        RecentlyViewedManager.clear()
            .then(function () {
                $wrapper.fadeOut(300);
                showNotification('Đã xóa lịch sử xem sản phẩm', '', 'success');
                clearInProgress = false;
            })
            .catch(function (err) {
                console.error('[RecentlyViewed] Clear failed:', err);
                showNotification('Không thể xóa lịch sử', '', 'error');
                clearInProgress = false;
            });
    });
});
