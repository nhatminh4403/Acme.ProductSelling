/* Global Scripts */

// =============================================================================
// UTILITY FUNCTIONS
// =============================================================================

/**
 * RECENTLY VIEWED PRODUCT MANAGER
 * Handles LocalStorage for guests and syncs with Server upon Login
 */

const RecentlyViewedManager = (function () {
    const STORAGE_KEY = 'Acme_RecentlyViewed_Ids';
    const MAX_GUEST_ITEMS = 10;

    // =========================================================================
    // PRIVATE: LocalStorage Helpers
    // =========================================================================

    function getGuestIds() {
        try {
            const json = localStorage.getItem(STORAGE_KEY);
            return json ? JSON.parse(json) : [];
        } catch (e) {
            console.warn('Failed to read recently viewed:', e);
            return [];
        }
    }

    function saveGuestIds(ids) {
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
        } catch (e) {
            console.warn('Failed to save recently viewed:', e);
        }
    }

    function clearGuestIds() {
        try {
            localStorage.removeItem(STORAGE_KEY);
        } catch (e) {
            console.warn('Failed to clear recently viewed:', e);
        }
    }

    function isAuthenticated() {
        return !!(abp?.currentUser?.isAuthenticated);
    }

    function getApiProxy() {
        return acme?.productSelling?.products?.recentlyViewedProduct || null;
    }

    function formatCurrency(value) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
            maximumFractionDigits: 0
        }).format(value);
    }

    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function renderProductCard(product) {
        let priceHtml;
        if (product.discountedPrice && product.discountedPrice < product.originalPrice) {
            priceHtml = `
            <div class="rv-price-group">
                <span class="rv-price-old">${formatCurrency(product.originalPrice)}</span>
                <span class="rv-price-current">${formatCurrency(product.discountedPrice)}</span>
                <span class="rv-discount-badge">-${product.discountPercent}%</span>
            </div>`;
        } else {
            priceHtml = `        
            <div class="rv-price-group">
                <span class="rv-price-current text-primary">${formatCurrency(product.originalPrice)}</span>
            </div>`;
        }
        const stockHtml = product.isAvailableForPurchase
            ? ''
            : '<div class="rv-stock-badge">Hết hàng</div>';

        // SAFETY FIX: Prevent Infinite Image Loop
        // We use a safe one-time handler.
        // Also assuming you have a default '/images/no-image.png' or similar on your server.
        const safeImgSrc = product.imageUrl || '/images/placeholder.png';

        return `
        <div class="col-12 col-md-6 col-lg-3">
            <a href="/products/${product.urlSlug}" class="rv-card" title="${escapeHtml(product.productName)}">
                <div class="rv-img-wrapper">
                    ${stockHtml}
                    <img src="${safeImgSrc}" 
                         alt="${escapeHtml(product.productName)}"
                         loading="lazy"
                         onerror="this.onerror=null; this.src='/images/placeholder.png';"> 
                </div>
                <div class="rv-content">
                    <div class="rv-title">
                        ${escapeHtml(product.productName)}
                    </div>
                    ${priceHtml}
                </div>
            </a>
        </div>`;
    }

    // =========================================================================
    // PUBLIC API
    // =========================================================================

    return {
        /**
         * Track a product view
         */
        trackView: function (productId) {
            if (!productId) return;

            // Always update localStorage
            let ids = getGuestIds();
            ids = ids.filter(id => id !== productId);
            ids.unshift(productId);
            if (ids.length > MAX_GUEST_ITEMS) ids.pop();
            saveGuestIds(ids);

            // If authenticated, also track on server
            if (isAuthenticated()) {
                const api = getApiProxy();
                if (api) {
                    api.trackProductView(productId).catch(err => {
                        console.warn('Failed to track view:', err);
                    });
                }
            }
        },

        /**
         * Sync guest history to server after login
         */
        syncWithServer: function () {
            const guestIds = getGuestIds();
            if (guestIds.length === 0 || !isAuthenticated()) {
                return Promise.resolve();
            }

            const api = getApiProxy();
            if (!api) return Promise.resolve();

            return api.syncGuestHistory({ productIds: guestIds })
                .then(function () {
                    clearGuestIds();
                    console.log('Recently viewed synced');
                })
                .catch(function (err) {
                    console.warn('Sync failed:', err);
                });
        },

        /**
         * Load and render widget
         * @param {string} containerId - Container element ID
         * @param {number} maxCount - Max products to show
         * @param {string|null} excludeProductId - Product ID to exclude (current product)
         * @param {function} callback - Callback with (hasProducts) parameter
         */
        loadWidget: function (containerId, maxCount, excludeProductId, callback) {
            maxCount = maxCount || 6;
            const $container = $('#' + containerId);

            if ($container.length === 0) {
                if (callback) callback(false);
                return;
            }

            let guestIds = getGuestIds();

            // Exclude current product if specified
            if (excludeProductId) {
                guestIds = guestIds.filter(id => id !== excludeProductId);
            }

            // If no guest IDs and not authenticated, nothing to show
            if (guestIds.length === 0 && !isAuthenticated()) {
                if (callback) callback(false);
                return;
            }

            const api = getApiProxy();
            if (!api) {
                if (callback) callback(false);
                return;
            }

            api.getList({
                maxCount: maxCount + 1, // Fetch one extra in case we need to exclude
                guestProductIds: guestIds
            })
                .then(function (products) {
                    if (!products || products.length === 0) {
                        if (callback) callback(false);
                        return;
                    }

                    // Filter out excluded product
                    let filtered = products;
                    if (excludeProductId) {
                        filtered = products.filter(p => p.productId !== excludeProductId);
                    }

                    // Take only what we need
                    filtered = filtered.slice(0, maxCount);

                    if (filtered.length === 0) {
                        if (callback) callback(false);
                        return;
                    }

                    // Render
                    const html = filtered.map(renderProductCard).join('');
                    $container.html(html);

                    if (callback) callback(true);
                })
                .catch(function (err) {
                    console.warn('Failed to load recently viewed:', err);
                    if (callback) callback(false);
                });
        },

        /**
         * Clear all recently viewed
         */
        clear: function () {
            clearGuestIds();

            if (isAuthenticated()) {
                const api = getApiProxy();
                if (api?.clear) {
                    return api.clear().catch(console.warn);
                }
            }

            return Promise.resolve();
        },

        getGuestIds: getGuestIds
    };
})();

document.addEventListener('DOMContentLoaded', function () {
    const $wrapper = $('#recently-viewed-section');

    // 1. Check if the ViewComponent exists on this page
    if ($wrapper.length === 0) return;

    // 2. Check if dependencies are loaded
    if (typeof RecentlyViewedManager === 'undefined') {
        console.error('RecentlyViewedManager script is missing');
        return;
    }

    // 3. Read the Data Attributes we set in the CSHTML
    const config = {
        maxCount: $wrapper.data('max-count'),
        excludeProductId: $wrapper.data('exclude-product-id'),
        isAuthenticated: $wrapper.data('is-authenticated')
    };

    // 4. Execute Logic
    RecentlyViewedManager.loadWidget(
        'recently-viewed-products', // Container ID inside the wrapper
        config.maxCount,
        config.excludeProductId || null,
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

    // 5. Attach Click Event (Delegation or direct)
    $('#btn-clear-recently-viewed').on('click', function (e) {
        e.preventDefault();
        if (confirm('Bạn có chắc muốn xóa lịch sử sản phẩm đã xem?')) {
            RecentlyViewedManager.clear().then(function () {
                $wrapper.fadeOut(300);
                if (typeof abp !== 'undefined') abp.notify.success('Đã xóa lịch sử xem sản phẩm');
            });
        }
    });
});
window.RecentlyViewedManager = RecentlyViewedManager;
/**
 * Safely initialize a Bootstrap modal
 */
function safeInitModal(modalElementId) {
    const modalElement = document.getElementById(modalElementId);
    if (!modalElement) return null;

    if (typeof bootstrap !== 'undefined') {
        try {
            return new bootstrap.Modal(modalElement);
        } catch (e) {
            console.log(`Could not initialize ${modalElementId} with Bootstrap 5:`, e);
        }
    }

    console.error(`Failed to initialize modal ${modalElementId}`);
    return null;
}

/**
 * Get CSRF token from various sources
 */
function getCSRFToken() {
    return $('input[name="__RequestVerificationToken"]').val() ||
        $('meta[name="__RequestVerificationToken"]').attr('content') ||
        document.querySelector('input[name="__RequestVerificationToken"]')?.value ||
        (typeof abp !== 'undefined' && abp.security ? abp.security.antiForgery.getToken() : null);
}

/**
 * Get proper headers for ABP API calls
 */
function getABPHeaders() {
    const headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    };

    const csrfToken = getCSRFToken();
    if (csrfToken) {
        headers['RequestVerificationToken'] = csrfToken;
        headers['X-CSRF-TOKEN'] = csrfToken;
    }

    if (typeof abp !== 'undefined' && abp.multiTenancy && abp.multiTenancy.getTenantIdCookie) {
        const tenantId = abp.multiTenancy.getTenantIdCookie();
        if (tenantId) {
            headers['__tenant'] = tenantId;
        }
    }

    return headers;
}

/**
 * Localization helper function
 */
const L = function (key, resourceName) {
    const defaultResourceName = 'ProductSelling';
    resourceName = resourceName || defaultResourceName;

    if (typeof abp !== 'undefined' && abp.localization && abp.localization.localize) {
        try {
            return abp.localization.localize(key, resourceName);
        } catch (e) {
            console.warn("Error with localization for key: " + key, e);
        }
    }

    const fallbackTranslations = {
        // Login
        'Login:Processing': 'Đang xử lý đăng nhập...',
        'Login:Initiated': 'Bắt đầu đăng nhập',
        'Login:Success': 'Đăng nhập thành công!',
        'Login:ErrorDefault': 'Đăng nhập thất bại. Vui lòng kiểm tra thông tin và thử lại.',
        'Login:ErrorTitle': 'Lỗi đăng nhập',
        'Login:ButtonDefault': 'Đăng nhập',
        // Register
        'Register:Processing': 'Đang xử lý đăng ký...',
        'Register:Initiated': 'Bắt đầu đăng ký',
        'Register:SuccessAndLoggingIn': 'Đăng ký thành công! Đang đăng nhập...',
        'Register:ErrorDefault': 'Đăng ký thất bại. Vui lòng kiểm tra thông tin và thử lại.',
        'Register:ButtonDefault': 'Đăng ký',
        'Register:PasswordsDoNotMatch': 'Mật khẩu không khớp.',
        // Logout
        'Logout:Processing': 'Đang đăng xuất...',
        'Logout:PleaseWait': 'Vui lòng đợi',
        'Logout:Success': 'Bạn đã đăng xuất thành công!',
        'Logout:Complete': 'Đăng xuất hoàn tất',
        // Validation
        'Validation:ErrorTitle': 'Lỗi xác thực',
        // Auto Login
        'AutoLogin:ErrorDefault': 'Đăng ký thành công nhưng tự động đăng nhập thất bại. Vui lòng đăng nhập thủ công.',
        'AutoLogin:WarnTitle': 'Cảnh báo tự động đăng nhập'
    };

    return fallbackTranslations[key] || key;
};

// =============================================================================
// NOTIFICATION SYSTEM
// =============================================================================

$(function () {
    const activeNotifications = new Set();

    function showUniqueNotification(message, title, type = 'info', options = {}) {
        const notificationKey = `${type}-${title}-${message}`;

        if (activeNotifications.has(notificationKey)) {
            return;
        }

        activeNotifications.add(notificationKey);

        setTimeout(() => {
            activeNotifications.delete(notificationKey);
        }, options.timeOut || 5000);

        if (typeof abp !== 'undefined' && abp.notify && abp.notify[type]) {
            abp.notify[type](message, title, options);
        } else {
            console.log(`${type.toUpperCase()}: ${title ? title + ": " : ""}${message}`);
        }
    }

    function showNotification(message, title, type = 'info', options = {}) {
        showUniqueNotification(message, title, type, options);
    }

    // Initialize ABP notify system with duplicate prevention
    window.abp = window.abp || {};
    abp.notify = abp.notify || {};
    abp.message = abp.message || {};

    const originalNotify = {
        success: abp.notify.success,
        error: abp.notify.error,
        info: abp.notify.info,
        warn: abp.notify.warn
    };

    ['success', 'error', 'info', 'warn'].forEach(type => {
        if (typeof abp.notify[type] !== 'function') {
            abp.notify[type] = function (message, title, options) {
                showUniqueNotification(message, title, type, options);
            };
        } else {
            const originalMethod = originalNotify[type];
            abp.notify[type] = function (message, title, options) {
                const notificationKey = `${type}-${title}-${message}`;
                if (!activeNotifications.has(notificationKey)) {
                    activeNotifications.add(notificationKey);
                    setTimeout(() => activeNotifications.delete(notificationKey), 5000);
                    originalMethod.call(this, message, title, options);
                }
            };
        }
    });

    // Make showNotification globally available
    window.showNotification = showNotification;
});

// =============================================================================
// LOGOUT HANDLER
// =============================================================================

$(function () {
    function setupLogoutHandlers() {
        document.querySelectorAll('a[href^="/Account/Logout"], form[action*="/Account/Logout"] button[type="submit"]').forEach(function (element) {
            element.addEventListener('click', function (e) {
                e.preventDefault();

                let href;
                if (element.tagName === 'A') {
                    href = element.getAttribute('href');
                } else if (element.tagName === 'BUTTON') {
                    const form = element.closest('form');
                    href = form.getAttribute('action') || '/Account/Logout';
                    const returnUrlInput = form.querySelector('input[name="returnUrl"]');
                    if (returnUrlInput) {
                        href += '?returnUrl=' + encodeURIComponent(returnUrlInput.value);
                    }
                }

                if (href.indexOf('returnUrl') === -1) {
                    href += (href.indexOf('?') === -1 ? '?' : '&') + 'returnUrl=/';
                }

                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.info(L('Logout:Processing'), L('Logout:PleaseWait'));
                }

                try {
                    sessionStorage.setItem('justLoggedOut', 'true');
                } catch (e) {
                    console.warn('Could not set sessionStorage:', e);
                }

                const form = document.createElement('form');
                form.method = 'POST';
                form.action = href;

                const token = getCSRFToken();
                if (token) {
                    const tokenInput = document.createElement('input');
                    tokenInput.type = 'hidden';
                    tokenInput.name = '__RequestVerificationToken';
                    tokenInput.value = token;
                    form.appendChild(tokenInput);
                }

                document.body.appendChild(form);
                form.submit();
            });
        });
    }

    // Setup logout handlers after ABP is ready
    if (typeof abp !== 'undefined' && abp.event) {
        abp.event.on('abp.setupComplete', setupLogoutHandlers);
    } else {
        $(document).ready(setupLogoutHandlers);
    }

    // Show logout success message
    try {
        if (sessionStorage.getItem('justLoggedOut') === 'true') {
            sessionStorage.removeItem('justLoggedOut');

            setTimeout(function () {
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.success(L('Logout:Success'), L('Logout:Complete'), {
                        timeOut: 3000
                    });
                }
            }, 500);
        }
    } catch (e) {
        console.warn('Could not check logout status:', e);
    }
});

// =============================================================================
// LOGIN HANDLER
// =============================================================================
// =============================================================================
// SESSION NOTIFICATION HANDLER (Login/Logout/Register messages)
// =============================================================================

$(function () {
    // 1. Check for Logout
    try {
        if (sessionStorage.getItem('justLoggedOut') === 'true') {
            sessionStorage.removeItem('justLoggedOut');
            // Small delay to ensure UI is ready
            setTimeout(function () {
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.success(L('Logout:Success'), L('Logout:Complete'));
                }
            }, 500);
        }
    } catch (e) { console.warn(e) };

    // 2. Check for Login Success
    try {
        if (sessionStorage.getItem('justLoggedIn') === 'true') {
            sessionStorage.removeItem('justLoggedIn');
            setTimeout(function () {
                showNotification(L('Login:Success'), '', 'success');
            }, 500);
        }
    } catch (e) { console.warn(e) };

    // 3. Check for Register Success
    try {
        if (sessionStorage.getItem('justRegistered') === 'true') {
            sessionStorage.removeItem('justRegistered');
            setTimeout(function () {
                showNotification('Chào mừng bạn gia nhập!', 'Đăng ký thành công', 'success');
            }, 500);
        }
    } catch (e) { console.warn(e) };
});
$(function () {
    const $loginModalElement = $('#loginModal');
    const loginForm = $('#loginForm');
    let loginButton, originalLoginButtonText;
    let isLoginInProgress = false;

    if (!loginForm.length) return;

    loginForm.on('submit', function (e) {
        e.preventDefault();

        if (isLoginInProgress) return;

        if (typeof $.fn.validate === 'function' && !loginForm.valid()) {
            return;
        }

        const email = $('#loginEmail').val().trim();
        const password = $('#loginPassword').val();

        if (!email || !password) {
            showNotification('Vui lòng điền đầy đủ thông tin bắt buộc.', 'Lỗi xác thực', 'error');
            return;
        }

        isLoginInProgress = true;
        loginButton = $(this).find('button[type="submit"]');
        originalLoginButtonText = loginButton.html();
        loginButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Đang xử lý...');

        const loginData = {
            userNameOrEmailAddress: email,
            password: password,
            rememberMe: $('#rememberMeCheck').is(':checked')
        };

        showNotification(L('Login:Processing'), L('Login:Initiated'), 'info', { timeOut: 1000 });

        setTimeout(() => {
            $.ajax({
                url: '/api/account/login',
                type: 'POST',
                data: JSON.stringify(loginData),
                headers: getABPHeaders(),
                dataType: 'json',
                timeout: 15000,
                success: handleLoginSuccess,
                error: function (xhr) {
                    let errorData;
                    try {
                        errorData = JSON.parse(xhr.responseText);
                    } catch (e) {
                        errorData = {
                            error: {
                                message: `Lỗi server (${xhr.status}): ${xhr.statusText}`,
                                details: xhr.responseText
                            }
                        };
                    }
                    handleLoginError(errorData);
                },
                complete: function () {
                    isLoginInProgress = false;
                    if (loginButton) {
                        loginButton.prop('disabled', false).html(originalLoginButtonText || 'Đăng nhập');
                    }
                }
            });
        }, 800);
    });

    function handleLoginSuccess() {
        if ($loginModalElement.length > 0) {
            try {
                const bsModal = bootstrap.Modal.getInstance($loginModalElement[0]);
                if (bsModal) bsModal.hide();
            } catch (e) {
                console.warn("Could not hide login modal:", e);
            }
        }
        sessionStorage.setItem('justLoggedIn', 'true');

        showNotification(L('Login:Success'), '', 'success', { timeOut: 2000 });


        const executeRedirect = () => {
            const returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
            if (returnUrl) {
                window.location.href = decodeURIComponent(returnUrl);
                return;
            }
            $.ajax({
                url: '/api/app/account/role-prefix',
                type: 'GET',
                headers: getABPHeaders(),
                success: function (result) {
                    if (result && result.hasAdminAccess) {
                        // Redirect to admin panel with role-based prefix
                        window.location.href = '/' + result.prefix;
                    } else {
                        // Regular user - stay on current page or go home
                        window.location.href = '/';
                    }
                },
                error: function () {
                    // If the API fails, just reload current page
                    window.location.reload();
                }
            });
        };

        const redirectDelay = 1500; // 1.5 seconds to see the toast

        if (typeof RecentlyViewedManager !== 'undefined') {
            if (loginButton) loginButton.html('<i class="fas fa-sync fa-spin"></i> Đang đồng bộ...');

            RecentlyViewedManager.syncWithServer()
                .finally(() => {
                    // Only call executeRedirect ONCE in finally
                    setTimeout(executeRedirect, redirectDelay);
                });
        } else {
            // Fixed: removed () to pass function reference, not invoke it
            setTimeout(executeRedirect, redirectDelay);
        }

    }

    function handleLoginError(error) {
        let errorMessage;

        if (error?.error?.message) {
            errorMessage = error.error.message;
        } else if (error?.error?.details) {
            errorMessage = error.error.details;
        } else if (typeof error?.error === 'string') {
            errorMessage = error.error;
        } else if (error?.message) {
            errorMessage = error.message;
        } else if (typeof error === 'string') {
            errorMessage = error;
        }

        if (error?.error?.validationErrors) {
            const validationMessages = [];
            for (const key in error.error.validationErrors) {
                if (error.error.validationErrors[key]) {
                    validationMessages.push(error.error.validationErrors[key].join(', '));
                }
            }
            if (validationMessages.length > 0) {
                errorMessage = validationMessages.join('; ');
            }
        }

        errorMessage = errorMessage || L('Login:ErrorDefault');
        showNotification(errorMessage, L('Login:ErrorTitle'), 'error');
    }

    // Reset modal on close
    if ($loginModalElement.length) {
        $loginModalElement.on('hidden.bs.modal', function () {
            isLoginInProgress = false;
            if (loginForm.length && loginForm[0]) {
                if (typeof loginForm.validate === 'function') {
                    try {
                        loginForm.validate().resetForm();
                    } catch (e) {
                        console.warn("Could not reset validation:", e);
                    }
                }
                loginForm.find('.is-invalid').removeClass('is-invalid');
                loginForm.find('.invalid-feedback').remove();
                loginForm[0].reset();
            }
            if (loginButton) {
                loginButton.prop('disabled', false).html(originalLoginButtonText || L('Login:ButtonDefault'));
            }
        });
    }
});

// =============================================================================
// REGISTER HANDLER
// =============================================================================

$(function () {
    const registerModalElement = document.getElementById('registerModal');
    const registerForm = $('#registerForm');
    let registerButton, originalRegisterButtonText;
    let isRegisterInProgress = false;

    if (!registerForm.length) return;

    registerForm.on('submit', function (e) {
        e.preventDefault();

        if (isRegisterInProgress) return;

        if (typeof $.fn.validate === 'function' && !registerForm.valid()) {
            return;
        }

        const name = $('#registerName').val().trim();
        const surname = $('#registerSurname').val().trim();
        const email = $('#registerEmail').val().trim();
        const password = $('#registerPassword').val();
        const confirmPassword = $('#registerConfirmPassword').val();

        if (!name || !surname || !email || !password || !confirmPassword) {
            showNotification('Vui lòng điền đầy đủ thông tin bắt buộc.', 'Lỗi xác thực', 'error');
            return;
        }

        if (password !== confirmPassword) {
            showNotification(L('Register:PasswordsDoNotMatch'), L('Validation:ErrorTitle'), 'error');
            return;
        }

        if (password.length < 6) {
            showNotification('Mật khẩu phải có ít nhất 6 ký tự.', 'Lỗi xác thực', 'error');
            return;
        }

        isRegisterInProgress = true;
        registerButton = $(this).find('button[type="submit"]');
        originalRegisterButtonText = registerButton.html();
        registerButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Đang xử lý...');

        const registerData = {
            userName: email,
            emailAddress: email,
            password: password,
            surname: surname,
            name: name,
            appName: "ProductSelling"
        };

        showNotification(L('Register:Processing'), L('Register:Initiated'), 'info', { timeOut: 1000 });

        setTimeout(() => {
            $.ajax({
                url: '/api/app/account/register',
                type: 'POST',
                data: JSON.stringify(registerData),
                headers: getABPHeaders(),
                dataType: 'json',
                timeout: 15000,
                success: function (result) {
                    handleRegisterSuccess(result, registerData);
                },
                error: function (xhr) {
                    let errorData;
                    try {
                        errorData = JSON.parse(xhr.responseText);
                    } catch (e) {
                        errorData = {
                            error: {
                                message: `Đăng ký thất bại (${xhr.status}): ${xhr.statusText}`,
                                details: xhr.responseText
                            }
                        };
                    }
                    handleRegisterError(errorData);
                },
                complete: function () {
                    isRegisterInProgress = false;
                    if (registerButton) {
                        registerButton.prop('disabled', false).html(originalRegisterButtonText || 'Đăng ký');
                    }
                }
            });
        }, 800);
    });

    function handleRegisterSuccess(result, registeredUserData) {
        showNotification(L('Register:SuccessAndLoggingIn'), '', 'success');

        if (registerModalElement) {
            const modalInstance = bootstrap.Modal.getInstance(registerModalElement);
            if (modalInstance) modalInstance.hide();
        }

        setTimeout(() => {
            attemptAutoLogin(registeredUserData.emailAddress, registeredUserData.password);
        }, 1000);
    }

    function handleRegisterError(error) {
        let errorMessage;

        if (error?.error?.message) {
            errorMessage = error.error.message;
        } else if (error?.error?.details) {
            errorMessage = error.error.details;
        } else if (typeof error?.error === 'string') {
            errorMessage = error.error;
        } else if (error?.message) {
            errorMessage = error.message;
        } else if (typeof error === 'string') {
            errorMessage = error;
        }

        if (error?.error?.validationErrors) {
            const validationMessages = [];
            for (const key in error.error.validationErrors) {
                if (error.error.validationErrors[key]) {
                    validationMessages.push(error.error.validationErrors[key].join(', '));
                }
            }
            if (validationMessages.length > 0) {
                errorMessage = validationMessages.join('; ');
            }
        }

        errorMessage = errorMessage || L('Register:ErrorDefault');
        showNotification(errorMessage, 'Lỗi đăng ký', 'error');
    }

    function attemptAutoLogin(userNameOrEmail, password) {
        const loginData = {
            userNameOrEmailAddress: userNameOrEmail,
            password: password,
            rememberMe: false
        };
        showNotification('Đang tự động đăng nhập...', 'Vui lòng đợi', 'info');

        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(loginData),
            headers: getABPHeaders(),
            dataType: 'json',
            timeout: 10000,
            success: function () {
                sessionStorage.setItem('justRegistered', 'true');

                showNotification(L('Login:Success'), '', 'success');


                const executeRedirect = () => {
                    const returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');

                    if (returnUrl) {
                        window.location.href = decodeURIComponent(returnUrl);
                        return;
                    }

                    // Fetch role prefix for new user
                    $.ajax({
                        url: '/api/app/account/role-prefix',
                        type: 'GET',
                        headers: getABPHeaders(),
                        success: function (result) {
                            if (result && result.hasAdminAccess) {
                                window.location.href = '/' + result.prefix;
                            } else {
                                window.location.href = '/';
                            }
                        },
                        error: function () {
                            window.location.reload();
                        }
                    });
                };
                const redirectDelay = 1500;

                if (typeof RecentlyViewedManager !== 'undefined') {
                    RecentlyViewedManager.syncWithServer()
                        .finally(() => {
                            setTimeout(executeRedirect, redirectDelay);
                        });
                } else {
                    setTimeout(executeRedirect, redirectDelay);
                }
            },
            error: function (xhr) {
                let errorMessage = L('AutoLogin:ErrorDefault');
                try {
                    const errorData = JSON.parse(xhr.responseText);
                    if (errorData?.error?.message) {
                        errorMessage = errorData.error.message;
                    }
                } catch (e) {
                    // Use default message
                }

                showNotification(errorMessage, L('AutoLogin:WarnTitle'), 'warn');
            }
        });
    }

    // Reset modal on close
    if (registerModalElement) {
        registerModalElement.addEventListener('hidden.bs.modal', function () {
            isRegisterInProgress = false;
            if (registerForm.length && registerForm[0]) {
                if (typeof registerForm.validate === 'function') {
                    try {
                        registerForm.validate().resetForm();
                    } catch (e) {
                        console.warn("Could not reset register validation:", e);
                    }
                }
                registerForm.find('.is-invalid').removeClass('is-invalid');
                registerForm.find('.invalid-feedback').remove();
                registerForm[0].reset();
            }
            if (registerButton) {
                registerButton.prop('disabled', false).html(originalRegisterButtonText || L('Register:ButtonDefault'));
            }
        });
    }
});