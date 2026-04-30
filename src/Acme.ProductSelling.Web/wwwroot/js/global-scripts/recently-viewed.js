/* recently-viewed.js
 * Manages the "Recently Viewed Products" widget.
 * Works with ABP's dynamic service proxy (acme.productSelling.products.services.recentlyViewedProduct).
 * Safe to load at any point in the page lifecycle — handles both "proxy ready" and "proxy not yet ready" cases.
 */
const RecentlyViewedManager = (function () {
    'use strict';

    const STORAGE_KEY = 'Acme_RecentlyViewed_Ids';
    const MAX_GUEST_ITEMS = 10;
    const SYNC_COOLDOWN_MS = 5000;

    let isSyncing = false;
    let lastSyncTime = 0;
    let pendingRequest = null;

    // ─── Local Storage Helpers ────────────────────────────────────────────────

    function getGuestIds() {
        try {
            const json = localStorage.getItem(STORAGE_KEY);
            if (!json) return [];
            const ids = JSON.parse(json);
            if (!Array.isArray(ids)) {
                localStorage.removeItem(STORAGE_KEY);
                return [];
            }
            return ids.filter(function (id) {
                return id && typeof id === 'string' && id.trim().length > 0;
            }).slice(0, MAX_GUEST_ITEMS);
        } catch (e) {
            try { localStorage.removeItem(STORAGE_KEY); } catch (_) { }
            return [];
        }
    }

    function saveGuestIds(ids) {
        if (!Array.isArray(ids)) return;
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(ids.slice(0, MAX_GUEST_ITEMS)));
        } catch (e) { }
    }

    function clearGuestIds() {
        try { localStorage.removeItem(STORAGE_KEY); } catch (e) { }
    }

    // ─── ABP Helpers ──────────────────────────────────────────────────────────

    function isAuthenticated() {
        try {
            return !!(abp && abp.currentUser && abp.currentUser.isAuthenticated);
        } catch (e) {
            return false;
        }
    }

    function getApiProxy() {
        try {
            return acme?.productSelling?.products?.services?.recentlyViewedProduct ?? null;
        } catch (e) {
            return null;
        }
    }

    // ─── Rendering ────────────────────────────────────────────────────────────

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
        if (!product || !product.productId) return '';

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

        const stockHtml = product.isAvailableForPurchase
            ? ''
            : '<div class="rv-stock-badge">Hết hàng</div>';

        const safeImgSrc = product.imageUrl || '/images/placeholder.png';
        const urlSlug = product.urlSlug || '#';

        return `
        <div class="col">
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

    // ─── Public API ───────────────────────────────────────────────────────────

    return {

        /**
         * Record that the user viewed a product.
         * Always updates localStorage; also notifies the server for authenticated users.
         */
        trackView: function (productId) {
            if (!productId || typeof productId !== 'string') return;

            try {
                let ids = getGuestIds();
                ids = ids.filter(function (id) { return id !== productId; });
                ids.unshift(productId);
                saveGuestIds(ids);

                if (isAuthenticated()) {
                    const api = getApiProxy();
                    if (api && api.trackProductView) {
                        api.trackProductView(productId).catch(function (err) {
                            console.warn('[RecentlyViewed] trackView server call failed:', err);
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
         */
        syncWithServer: function () {
            const now = Date.now();
            if (isSyncing) return Promise.resolve();
            if (now - lastSyncTime < SYNC_COOLDOWN_MS) return Promise.resolve();

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
                    isSyncing = false;
                })
                .catch(function (err) {
                    console.warn('[RecentlyViewed] Sync failed:', err);
                    isSyncing = false;
                });
        },

        /**
         * Load recently-viewed products and inject them into a container.
         *
         * @param {string}   containerId       DOM id of the product grid element
         * @param {number}   [maxCount=6]
         * @param {string}   [excludeProductId]
         * @param {Function} [callback]        Called with true (has products) or false
         */
        loadWidget: function (containerId, maxCount, excludeProductId, callback) {
            maxCount = Math.min(parseInt(maxCount, 10) || 6, 20);

            if (!containerId || typeof containerId !== 'string') {
                if (callback) callback(false);
                return;
            }

            const $container = $('#' + containerId);
            if ($container.length === 0) {
                if (callback) callback(false);
                return;
            }

            // Cancel any stale in-flight request token
            pendingRequest = null;

            let guestIds = getGuestIds();
            if (excludeProductId) {
                guestIds = guestIds.filter(function (id) { return id !== excludeProductId; });
            }

            // For unauthenticated users with no local history, nothing to show
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

            const requestToken = {};
            pendingRequest = requestToken;

            const requestPayload = { maxCount: maxCount + 1 }; // +1 allows filtering excludeProductId server-side

            const slicedIds = guestIds.slice(0, 20);
            if (slicedIds.length > 0) {
                requestPayload.guestProductIds = slicedIds;
            }

            console.log('[RecentlyViewed] Fetching widget, maxCount=%d, guestIds=%d', maxCount, slicedIds.length);

            api.getList(requestPayload)
                .then(function (products) {
                    if (pendingRequest !== requestToken) return; // Superseded
                    pendingRequest = null;

                    console.log('[RecentlyViewed] Server returned %d products', (products || []).length);

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

                    $container.html(filtered.map(renderProductCard).join(''));
                    if (callback) callback(true);
                })
                .catch(function (err) {
                    console.warn('[RecentlyViewed] Fetch error:', err);
                    pendingRequest = null;
                    if (callback) callback(false);
                });
        },

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

        /** Exposed for debugging */
        getGuestIds: getGuestIds,
    };

}());

window.RecentlyViewedManager = RecentlyViewedManager;

function initRecentlyViewedWidget() {
    const $wrapper = $('#recently-viewed-section');
    if (!$wrapper || $wrapper.length === 0) return; // Not on a page that has the widget

    const maxCount = parseInt($wrapper.data('max-count'), 10) || 6;
    const excludeId = $wrapper.data('exclude-product-id') || null;

    RecentlyViewedManager.loadWidget(
        'recently-viewed-products',
        (maxCount >= 1 && maxCount <= 20) ? maxCount : 6,
        excludeId,
        function (hasProducts) {
            // Always hide the skeleton first
            $('#recently-viewed-skeleton').hide();

            if (hasProducts) {

                $('#recently-viewed-products').removeAttr('style');

                $wrapper.css('display', 'block');
            } else {
                $wrapper.hide();
            }
        }
    );

    $('#btn-clear-recently-viewed').off('click.rv').on('click.rv', function (e) {
        e.preventDefault();

        abp.message.confirm(
            abp.localization.getResource('ProductSelling')('UI:ConfirmClearRecentlyViewed'),
            abp.localization.getResource('ProductSelling')('UI:AreYouSure'),
            function (confirmed) {
                if (!confirmed) return;

                RecentlyViewedManager.clear()
                    .then(function () {
                        $wrapper.fadeOut(300);
                        if (typeof showNotification === 'function') {
                            showNotification('Đã xóa lịch sử xem sản phẩm', '', 'success');
                        }
                    })
                    .catch(function (err) {
                        console.error('[RecentlyViewed] Clear failed:', err);
                        if (typeof showNotification === 'function') {
                            showNotification('Không thể xóa lịch sử', '', 'error');
                        }
                    });
            }
        );
    });
}

(function () {
    'use strict';

    var _widgetRan = false;

    function runOnce() {
        if (_widgetRan) return;
        _widgetRan = true;
        initRecentlyViewedWidget();
    }

    function bootstrap() {
        if (typeof abp === 'undefined' || !abp.event) {
            // Case A: No ABP — just run on DOMContentLoaded
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', runOnce);
            } else {
                runOnce();
            }
            return;
        }

        // Cases B and C: ABP available. Subscribe to setupComplete first so we
        // don't miss it, then check if it already fired.
        abp.event.on('abp.setupComplete', function () {
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', runOnce);
            } else {
                runOnce();
            }
        });

        // If applicationConfiguration is already loaded, abp.appInfo will be set.
        // That's a reliable signal that setupComplete has already fired.
        if (abp.appInfo || (abp.currentUser !== undefined)) {
            // setupComplete already fired — run now (or after DOM is ready)
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', runOnce);
            } else {
                runOnce();
            }
        }
    }

    // Run bootstrap immediately since the script is at bottom of body
    bootstrap();
}());