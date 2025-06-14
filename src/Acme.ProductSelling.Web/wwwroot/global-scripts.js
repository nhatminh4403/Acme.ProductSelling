/* Your Global Scripts - Fixed for ABP API */
function safeInitModal(modalElementId) {
    const modalElement = document.getElementById(modalElementId);
    if (!modalElement) return null;

    let modalInstance = null;

    // Try Bootstrap 5 approach
    if (typeof bootstrap !== 'undefined') {
        try {
            modalInstance = new bootstrap.Modal(modalElement);
            return modalInstance;
        } catch (e) {
            console.log(`Could not initialize ${modalElementId} with Bootstrap 5:`, e);
        }
    }

    console.error(`Failed to initialize modal ${modalElementId}`);
    return null;
}

document.addEventListener('DOMContentLoaded', function () {
    const menuContainer = document.querySelector('.category-dropdown-container');
    if (!menuContainer) return;

    const mainToggleButton = menuContainer.querySelector('#navbarDropdownCategories');
    const mainDropdownMenu = menuContainer.querySelector('.category-hover-menu');

    let mainHideTimeout;
    const HIDE_DELAY = 500; // Milliseconds

    // --- Main Menu Logic ---
    const showMainMenu = () => {
        clearTimeout(mainHideTimeout);
        if (mainDropdownMenu && !mainDropdownMenu.classList.contains('show')) {
            mainDropdownMenu.classList.add('show');
            mainToggleButton.setAttribute('aria-expanded', 'true');
        }
    };

    const startHideMainMenu = () => {
        mainHideTimeout = setTimeout(() => {
            if (mainDropdownMenu && mainDropdownMenu.classList.contains('show')) {
                // Check if the mouse is currently over any submenu before hiding main
                let isHoveringSubmenu = false;
                mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => {
                    if (sm.matches(':hover')) {
                        isHoveringSubmenu = true;
                    }
                });
                // Also check if hovering the main menu itself
                if (mainDropdownMenu.matches(':hover')) {
                    isHoveringSubmenu = true;
                }

                if (!isHoveringSubmenu) { // Only hide if not hovering a related menu
                    mainDropdownMenu.classList.remove('show');
                    mainToggleButton.setAttribute('aria-expanded', 'false');
                    mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => sm.classList.remove('show'));
                }
            }
        }, HIDE_DELAY);
    };

    if (mainToggleButton && mainDropdownMenu) {
        menuContainer.addEventListener('mouseenter', showMainMenu);
        menuContainer.addEventListener('mouseleave', startHideMainMenu);

        // --- Submenu (Mega Menu) Logic ---
        const submenuParentItems = mainDropdownMenu.querySelectorAll('.has-submenu');

        submenuParentItems.forEach(item => {
            const submenu = item.querySelector('.submenu'); // Finds the .mega-menu
            let submenuHideTimeout;

            // Function to show this specific submenu and hide others
            const showSubmenu = () => {
                clearTimeout(mainHideTimeout); // Keep main menu open
                clearTimeout(submenuHideTimeout);

                // Hide other currently open sibling submenus
                mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(openSubmenu => {
                    if (openSubmenu !== submenu) {
                        openSubmenu.classList.remove('show');
                    }
                });

                if (submenu && !submenu.classList.contains('show')) {
                    submenu.classList.add('show');
                }
            }

            // Function to start hiding this submenu
            const startHideSubmenu = () => {
                if (submenu) {
                    submenuHideTimeout = setTimeout(() => {
                        // Double check: Only hide if mouse isn't back over the trigger item or the submenu itself
                        if (!item.matches(':hover') && !submenu.matches(':hover')) {
                            submenu.classList.remove('show');
                        }
                    }, HIDE_DELAY);
                }
                // Also trigger the main menu hide check in case mouse moved completely out
                startHideMainMenu();
            }

            item.addEventListener('mouseenter', showSubmenu);
            item.addEventListener('mouseleave', startHideSubmenu);

            if (submenu) {
                submenu.addEventListener('mouseenter', () => {
                    clearTimeout(mainHideTimeout);
                    clearTimeout(submenuHideTimeout); // Clear hide timer when entering the submenu
                });

                submenu.addEventListener('mouseleave', startHideSubmenu); // Start hide timer when leaving submenu
            }
        });

        // Click listener for main button (touch fallback)
        mainToggleButton.addEventListener('click', function (event) {
            const isShown = mainDropdownMenu.classList.contains('show');
            if (isShown) {
                mainDropdownMenu.classList.remove('show');
                mainToggleButton.setAttribute('aria-expanded', 'false');
                mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => sm.classList.remove('show'));
            } else {
                mainDropdownMenu.classList.add('show');
                mainToggleButton.setAttribute('aria-expanded', 'true');
            }
        });

        // Click outside to close main menu
        document.addEventListener('click', function (event) {
            if (mainDropdownMenu && !menuContainer.contains(event.target) && mainDropdownMenu.classList.contains('show')) {
                mainDropdownMenu.classList.remove('show');
                mainToggleButton.setAttribute('aria-expanded', 'false');
                mainDropdownMenu.querySelectorAll('.has-submenu .submenu.show').forEach(sm => sm.classList.remove('show'));
            }
        });
    }
});

//logout - Enhanced with better error handling
$(function () {
    // Wait for ABP to be ready
    if (typeof abp !== 'undefined' && abp.event) {
        abp.event.on('abp.setupComplete', function () {
            setupLogoutHandlers();
        });
    } else {
        // Fallback if ABP is not available
        $(document).ready(function () {
            setupLogoutHandlers();
        });
    }

    function setupLogoutHandlers() {
        document.querySelectorAll('a[href^="/Account/Logout"]').forEach(function (link) {
            link.addEventListener('click', function (e) {
                e.preventDefault();

                let href = link.getAttribute('href');

                if (href.indexOf('returnUrl') === -1) {
                    href += (href.indexOf('?') === -1 ? '?' : '&') + 'returnUrl=/';
                }

                // Show immediate feedback
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.info('Logging out...', 'Please wait');
                }

                setTimeout(function () {
                    if (typeof abp !== 'undefined' && abp.ajax) {
                        abp.ajax({
                            url: href,
                            type: 'GET',
                            dataType: 'json',
                            success: function () {
                                if (abp.notify) {
                                    abp.notify.success('You have been logged out successfully.', 'Logout Complete');
                                }
                                window.location.href = '/';
                            },
                            error: function (xhr, status, error) {
                                console.error('Logout error:', error);
                                // Still redirect even if there's an error
                                window.location.href = '/';
                            }
                        });
                    } else {
                        // Fallback using standard fetch
                        fetch(href, {
                            method: 'GET',
                            credentials: 'same-origin'
                        })
                            .then(() => {
                                window.location.href = '/';
                            })
                            .catch((error) => {
                                console.error('Logout error:', error);
                                window.location.href = '/';
                            });
                    }
                }, 500);
            });
        });
    }
});

$(function () {
    // Function to get CSRF token
    function getCSRFToken() {
        var token = $('input[name="__RequestVerificationToken"]').val() ||
            $('meta[name="__RequestVerificationToken"]').attr('content') ||
            document.querySelector('input[name="__RequestVerificationToken"]')?.value ||
            (typeof abp !== 'undefined' && abp.security ? abp.security.antiForgery.getToken() : null);
        return token;
    }

    // Function to get proper headers for ABP API
    function getABPHeaders() {
        var headers = {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        };

        var csrfToken = getCSRFToken();
        if (csrfToken) {
            headers['RequestVerificationToken'] = csrfToken;
            headers['X-CSRF-TOKEN'] = csrfToken;
        }

        if (typeof abp !== 'undefined' && abp.multiTenancy && abp.multiTenancy.getTenantIdCookie) {
            var tenantId = abp.multiTenancy.getTenantIdCookie();
            if (tenantId) {
                headers['__tenant'] = tenantId;
            }
        }

        return headers;
    }

    // Localization helper function
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
            'Login:Processing': 'Đang xử lý đăng nhập...',
            'Login:Initiated': 'Bắt đầu đăng nhập',
            'Login:Success': 'Đăng nhập thành công!',
            'Login:ErrorDefault': 'Đăng nhập thất bại. Vui lòng kiểm tra thông tin và thử lại.',
            'Login:ErrorTitle': 'Lỗi đăng nhập',
            'Login:ButtonDefault': 'Đăng nhập',
            'Register:Processing': 'Đang xử lý đăng ký...',
            'Register:Initiated': 'Bắt đầu đăng ký',
            'Register:SuccessAndLoggingIn': 'Đăng ký thành công! Đang đăng nhập...',
            'Register:ErrorDefault': 'Đăng ký thất bại. Vui lòng kiểm tra thông tin và thử lại.',
            'Register:ButtonDefault': 'Đăng ký',
            'Register:PasswordsDoNotMatch': 'Mật khẩu không khớp.',
            'Validation:ErrorTitle': 'Lỗi xác thực',
            'AutoLogin:ErrorDefault': 'Đăng ký thành công nhưng tự động đăng nhập thất bại. Vui lòng đăng nhập thủ công.',
            'AutoLogin:WarnTitle': 'Cảnh báo tự động đăng nhập'
        };

        return fallbackTranslations[key] || key;
    };

    // FIXED: Enhanced notification system - prevent duplicates
    var activeNotifications = new Set();

    function showUniqueNotification(message, title, type = 'info', options = {}) {
        // Create unique key for notification
        const notificationKey = `${type}-${title}-${message}`;

        // Prevent duplicate notifications
        if (activeNotifications.has(notificationKey)) {
            return;
        }

        activeNotifications.add(notificationKey);

        // Remove from active set after delay
        setTimeout(() => {
            activeNotifications.delete(notificationKey);
        }, options.timeOut || 5000);

        if (typeof abp !== 'undefined' && abp.notify && abp.notify[type]) {
            abp.notify[type](message, title, options);
        } else {
            console.log(`${type.toUpperCase()}: ${title ? title + ": " : ""}${message}`);
        }
    }

    // FIXED: Simplified notification function without auto-proceed
    function showNotification(message, title, type = 'info', options = {}) {
        showUniqueNotification(message, title, type, options);
    }

    // Initialize ABP notify system with fallbacks and duplicate prevention
    window.abp = window.abp || {};
    abp.notify = abp.notify || {};
    abp.message = abp.message || {};

    // Override ABP notify functions to prevent duplicates
    const originalNotify = {
        success: abp.notify.success,
        error: abp.notify.error,
        info: abp.notify.info,
        warn: abp.notify.warn
    };

    if (typeof abp.notify.success !== 'function') {
        abp.notify.success = function (message, title, options) {
            showUniqueNotification(message, title, 'success', options);
        };
    } else {
        abp.notify.success = function (message, title, options) {
            const notificationKey = `success-${title}-${message}`;
            if (!activeNotifications.has(notificationKey)) {
                activeNotifications.add(notificationKey);
                setTimeout(() => activeNotifications.delete(notificationKey), 5000);
                originalNotify.success.call(this, message, title, options);
            }
        };
    }

    if (typeof abp.notify.error !== 'function') {
        abp.notify.error = function (message, title, options) {
            showUniqueNotification(message, title, 'error', options);
        };
    } else {
        abp.notify.error = function (message, title, options) {
            const notificationKey = `error-${title}-${message}`;
            if (!activeNotifications.has(notificationKey)) {
                activeNotifications.add(notificationKey);
                setTimeout(() => activeNotifications.delete(notificationKey), 5000);
                originalNotify.error.call(this, message, title, options);
            }
        };
    }

    if (typeof abp.notify.info !== 'function') {
        abp.notify.info = function (message, title, options) {
            showUniqueNotification(message, title, 'info', options);
        };
    } else {
        abp.notify.info = function (message, title, options) {
            const notificationKey = `info-${title}-${message}`;
            if (!activeNotifications.has(notificationKey)) {
                activeNotifications.add(notificationKey);
                setTimeout(() => activeNotifications.delete(notificationKey), 5000);
                originalNotify.info.call(this, message, title, options);
            }
        };
    }

    if (typeof abp.notify.warn !== 'function') {
        abp.notify.warn = function (message, title, options) {
            showUniqueNotification(message, title, 'warn', options);
        };
    } else {
        abp.notify.warn = function (message, title, options) {
            const notificationKey = `warn-${title}-${message}`;
            if (!activeNotifications.has(notificationKey)) {
                activeNotifications.add(notificationKey);
                setTimeout(() => activeNotifications.delete(notificationKey), 5000);
                originalNotify.warn.call(this, message, title, options);
            }
        };
    }

    // --- ENHANCED LOGIN LOGIC ---
    var $loginModalElement = $('#loginModal');
    var loginForm = $('#loginForm');
    var loginButton, originalLoginButtonText;
    var isLoginInProgress = false; // Prevent multiple submissions

    if (loginForm.length) {
        loginForm.on('submit', function (e) {
            e.preventDefault();

            // Prevent multiple submissions
            if (isLoginInProgress) {
                return;
            }

            // Enhanced validation
            if (typeof $.fn.validate === 'function' && !loginForm.valid()) {
                return;
            }

            // Basic client-side validation
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

            var loginData = {
                userNameOrEmailAddress: email,
                password: password,
                rememberMe: $('#rememberMeCheck').is(':checked')
            };

            console.log("Attempting login with data:", {
                userNameOrEmailAddress: email,
                rememberMe: loginData.rememberMe
            });

            // Show processing notification
            showNotification(L('Login:Processing'), L('Login:Initiated'), 'info', { timeOut: 1000 });

            // Perform login after short delay
            setTimeout(() => {
                $.ajax({
                    url: '/api/account/login',
                    type: 'POST',
                    data: JSON.stringify(loginData),
                    headers: getABPHeaders(),
                    dataType: 'json',
                    timeout: 15000,
                    success: function (result) {
                        console.log("Login successful:", result);
                        handleLoginSuccess(result);
                    },
                    error: function (xhr, status, error) {
                        console.error("Login failed:", {
                            status: xhr.status,
                            statusText: xhr.statusText,
                            responseText: xhr.responseText,
                            error: error
                        });

                        let errorData;
                        try {
                            errorData = JSON.parse(xhr.responseText);
                        } catch (e) {
                            errorData = {
                                error: {
                                    message: `Lỗi server (${xhr.status}): ${xhr.statusText || error}`,
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
    }

    function handleLoginSuccess(result) {
        console.log("Processing login success:", result);

        // Hide modal if it exists
        if ($loginModalElement.length > 0) {
            try {
                var bsModal = bootstrap.Modal.getInstance($loginModalElement[0]);
                if (bsModal) {
                    bsModal.hide();
                }
            } catch (e) {
                console.warn("Could not hide login modal:", e);
            }
        }
        abp.notify.success(L('Login:Success'));
        showNotification(L('Login:Success'), '', 'success', { timeOut: 2000 });

        // Enhanced redirect logic
        setTimeout(() => {
            var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
            if (returnUrl) {
                window.location.href = decodeURIComponent(returnUrl);
            } else {
                window.location.reload();
            }
        }, 1000);
    }

    function handleLoginError(error) {
        console.error("Processing login error:", error);

        var errorMessage;

        if (error && error.error) {
            if (error.error.message) {
                errorMessage = error.error.message;
            } else if (error.error.details) {
                errorMessage = error.error.details;
            } else if (typeof error.error === 'string') {
                errorMessage = error.error;
            }
        } else if (error && error.message) {
            errorMessage = error.message;
        } else if (typeof error === 'string') {
            errorMessage = error;
        }

        if (error && error.error && error.error.validationErrors) {
            var validationMessages = [];
            for (var key in error.error.validationErrors) {
                if (error.error.validationErrors[key]) {
                    validationMessages.push(error.error.validationErrors[key].join(', '));
                }
            }
            if (validationMessages.length > 0) {
                errorMessage = validationMessages.join('; ');
            }
        }

        if (!errorMessage) {
            errorMessage = L('Login:ErrorDefault');
        }

        showNotification(errorMessage, L('Login:ErrorTitle'), 'error');
    }

    // Enhanced modal reset
    if ($loginModalElement.length) {
        $loginModalElement.on('hidden.bs.modal', function (event) {
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

    // --- ENHANCED REGISTER LOGIC ---
    var registerModalElement = document.getElementById('registerModal');
    var registerForm = $('#registerForm');
    var registerButton, originalRegisterButtonText;
    var isRegisterInProgress = false;

    if (registerForm.length) {
        registerForm.on('submit', function (e) {
            e.preventDefault();

            if (isRegisterInProgress) {
                return;
            }

            if (typeof $.fn.validate === 'function' && !registerForm.valid()) {
                return;
            }

            const name = $('#registerName').val().trim();
            const email = $('#registerEmail').val().trim();
            const password = $('#registerPassword').val();
            const confirmPassword = $('#registerConfirmPassword').val();

            if (!name || !email || !password || !confirmPassword) {
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

            var currentRegisterData = {
                userName: name,
                emailAddress: email,
                password: password,
                appName: "ProductSelling"
            };

            console.log("Attempting registration with data:", {
                userName: name,
                emailAddress: email,
                appName: currentRegisterData.appName
            });

            showNotification(L('Register:Processing'), L('Register:Initiated'), 'info', { timeOut: 1000 });

            setTimeout(() => {
                $.ajax({
                    url: '/api/account/register',
                    type: 'POST',
                    data: JSON.stringify(currentRegisterData),
                    headers: getABPHeaders(),
                    dataType: 'json',
                    timeout: 15000,
                    success: function (result) {
                        console.log("Registration successful:", result);
                        handleRegisterSuccess(result, currentRegisterData);
                    },
                    error: function (xhr, status, error) {
                        console.error("Registration failed:", {
                            status: xhr.status,
                            statusText: xhr.statusText,
                            responseText: xhr.responseText,
                            error: error
                        });

                        let errorData;
                        try {
                            errorData = JSON.parse(xhr.responseText);
                        } catch (e) {
                            errorData = {
                                error: {
                                    message: `Đăng ký thất bại (${xhr.status}): ${xhr.statusText || error}`,
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
    }

    function handleRegisterSuccess(result, registeredUserData) {
        console.log("Processing registration success:", result);

        showNotification(L('Register:SuccessAndLoggingIn'), '', 'success');

        if (registerModalElement) {
            var modalInstance = bootstrap.Modal.getInstance(registerModalElement);
            if (modalInstance) {
                modalInstance.hide();
            }
        }

        setTimeout(() => {
            attemptAutoLogin(registeredUserData.emailAddress, registeredUserData.password);
        }, 1000);
    }

    function handleRegisterError(error) {
        console.error("Processing registration error:", error);

        var errorMessage;

        if (error && error.error) {
            if (error.error.message) {
                errorMessage = error.error.message;
            } else if (error.error.details) {
                errorMessage = error.error.details;
            } else if (typeof error.error === 'string') {
                errorMessage = error.error;
            }
        } else if (error && error.message) {
            errorMessage = error.message;
        } else if (typeof error === 'string') {
            errorMessage = error;
        }

        if (error && error.error && error.error.validationErrors) {
            var validationMessages = [];
            for (var key in error.error.validationErrors) {
                if (error.error.validationErrors[key]) {
                    validationMessages.push(error.error.validationErrors[key].join(', '));
                }
            }
            if (validationMessages.length > 0) {
                errorMessage = validationMessages.join('; ');
            }
        }

        if (!errorMessage) {
            errorMessage = L('Register:ErrorDefault');
        }

        showNotification(errorMessage, 'Lỗi đăng ký', 'error');
    }

    function attemptAutoLogin(userNameOrEmail, password) {
        console.log("Attempting auto-login for:", userNameOrEmail);

        var loginData = {
            userNameOrEmailAddress: userNameOrEmail,
            password: password,
            rememberMe: false
        };

        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(loginData),
            headers: getABPHeaders(),
            dataType: 'json',
            timeout: 10000,
            success: function (loginResult) {
                console.log("Auto-login successful:", loginResult);
                showNotification(L('Login:Success'), '', 'success');

                setTimeout(() => {
                    var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
                    if (returnUrl) {
                        window.location.href = decodeURIComponent(returnUrl);
                    } else {
                        window.location.reload();
                    }
                }, 1000);
            },
            error: function (xhr, status, error) {
                console.error("Auto-login failed:", {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    responseText: xhr.responseText,
                    error: error
                });

                var errorMessage = L('AutoLogin:ErrorDefault');
                try {
                    var errorData = JSON.parse(xhr.responseText);
                    if (errorData && errorData.error && errorData.error.message) {
                        errorMessage = errorData.error.message;
                    }
                } catch (e) {
                    // Use default message
                }

                showNotification(errorMessage, L('AutoLogin:WarnTitle'), 'warn');
            }
        });
    }

    // Enhanced register modal reset
    if (registerModalElement) {
        registerModalElement.addEventListener('hidden.bs.modal', function (event) {
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