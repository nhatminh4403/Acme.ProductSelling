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

// Enhanced Login and Register functionality - FIXED FOR ABP API
$(function () {
    // Function to get CSRF token
    function getCSRFToken() {
        // Try multiple ways to get the token
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

        // Add CSRF token if available
        var csrfToken = getCSRFToken();
        if (csrfToken) {
            headers['RequestVerificationToken'] = csrfToken;
            headers['X-CSRF-TOKEN'] = csrfToken;
        }

        // Add ABP tenant header if available
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

        // Fallback translations
        const fallbackTranslations = {
            'Login:Processing': 'Processing login...',
            'Login:Initiated': 'Login Started',
            'Login:Success': 'Login successful!',
            'Login:ErrorDefault': 'Login failed. Please check your credentials and try again.',
            'Login:ErrorTitle': 'Login Error',
            'Login:ButtonDefault': 'Login',
            'Register:Processing': 'Processing registration...',
            'Register:Initiated': 'Registration Started',
            'Register:SuccessAndLoggingIn': 'Registration successful! Logging you in...',
            'Register:ErrorDefault': 'Registration failed. Please check your input and try again.',
            'Register:ButtonDefault': 'Register',
            'Register:PasswordsDoNotMatch': 'Passwords do not match.',
            'Validation:ErrorTitle': 'Validation Error',
            'AutoLogin:ErrorDefault': 'Registration was successful, but auto-login failed. Please log in manually.',
            'AutoLogin:WarnTitle': 'Auto-login Warning'
        };

        return fallbackTranslations[key] || key;
    };

    // Enhanced notification system
    function showTemporaryNotificationAndProceed(message, title, delayMilliseconds, actionCallback, notificationOptions = {}) {
        if (typeof abp !== 'undefined' && abp.notify && abp.notify.info) {
            const mergedOptions = { timeOut: delayMilliseconds, ...notificationOptions };
            abp.notify.info(message, title, mergedOptions);
        } else {
            console.log((title ? title + ": " : "") + message);
        }

        setTimeout(function () {
            actionCallback();
        }, delayMilliseconds);
    }

    // Initialize ABP notify system with fallbacks
    window.abp = window.abp || {};
    abp.notify = abp.notify || {};
    abp.message = abp.message || {};

    if (typeof abp.notify.success !== 'function') {
        abp.notify.success = function (message, title) {
            console.log("Success:", (title ? title + ": " : "") + message);
            if (typeof toastr !== 'undefined') {
                toastr.success(message, title);
            } else {
                alert("Success: " + message);
            }
        };
    }
    if (typeof abp.notify.error !== 'function') {
        abp.notify.error = function (message, title) {
            console.error("Error:", (title ? title + ": " : "") + message);
            if (typeof toastr !== 'undefined') {
                toastr.error(message, title);
            } else {
                alert("Error: " + message);
            }
        };
    }
    if (typeof abp.notify.info !== 'function') {
        abp.notify.info = function (message, title, options) {
            console.info("Info:", (title ? title + ": " : "") + message);
            if (typeof toastr !== 'undefined') {
                toastr.info(message, title, options);
            }
        };
    }
    if (typeof abp.notify.warn !== 'function') {
        abp.notify.warn = function (message, title) {
            console.warn("Warning:", (title ? title + ": " : "") + message);
            if (typeof toastr !== 'undefined') {
                toastr.warning(message, title);
            } else {
                alert("Warning: " + message);
            }
        };
    }

    // --- ENHANCED LOGIN LOGIC - FIXED FOR ABP API ---
    var $loginModalElement = $('#loginModal');
    var loginForm = $('#loginForm');
    var loginButton, originalLoginButtonText;

    if (loginForm.length) {
        loginForm.on('submit', function (e) {
            e.preventDefault();

            // Enhanced validation
            if (typeof $.fn.validate === 'function' && !loginForm.valid()) {
                return;
            }

            // Basic client-side validation
            const email = $('#loginEmail').val().trim();
            const password = $('#loginPassword').val();

            if (!email || !password) {
                abp.notify.error('Please fill in all required fields.', 'Validation Error');
                return;
            }

            loginButton = $(this).find('button[type="submit"]');
            originalLoginButtonText = loginButton.html();
            loginButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Processing...');

            // FIXED: Correct ABP API login payload
            var loginData = {
                userNameOrEmailAddress: email,
                password: password,
                rememberMe: $('#rememberMeCheck').is(':checked')
            };

            console.log("Attempting login with data:", {
                userNameOrEmailAddress: email,
                rememberMe: loginData.rememberMe
            });

            function performLogin() {
                // FIXED: Use proper ABP API call with correct headers
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
                                    message: `Server error (${xhr.status}): ${xhr.statusText || error}`,
                                    details: xhr.responseText
                                }
                            };
                        }
                        handleLoginError(errorData);
                    },
                    complete: function () {
                        // Reset button state
                        if (loginButton) {
                            loginButton.prop('disabled', false).html(originalLoginButtonText || 'Login');
                        }
                    }
                });
            }

            showTemporaryNotificationAndProceed(
                L('Login:Processing'),
                L('Login:Initiated'),
                800,
                performLogin
            );
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

        // Handle ABP error format
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

        // Handle validation errors (common ABP pattern)
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

        abp.notify.error(errorMessage, L('Login:ErrorTitle'));
    }

    // Enhanced modal reset
    if ($loginModalElement.length) {
        $loginModalElement.on('hidden.bs.modal', function (event) {
            if (loginForm.length && loginForm[0]) {
                // Reset validation if jQuery Validate is present
                if (typeof loginForm.validate === 'function') {
                    try {
                        loginForm.validate().resetForm();
                    } catch (e) {
                        console.warn("Could not reset validation:", e);
                    }
                }
                // Remove Bootstrap validation classes
                loginForm.find('.is-invalid').removeClass('is-invalid');
                loginForm.find('.invalid-feedback').remove();
                // Reset form
                loginForm[0].reset();
            }
            // Reset button
            if (loginButton) {
                loginButton.prop('disabled', false).html(originalLoginButtonText || L('Login:ButtonDefault'));
            }
        });
    }

    // --- ENHANCED REGISTER LOGIC - FIXED FOR ABP API ---
    var registerModalElement = document.getElementById('registerModal');
    var registerForm = $('#registerForm');
    var registerButton, originalRegisterButtonText;

    if (registerForm.length) {
        registerForm.on('submit', function (e) {
            e.preventDefault();

            // Enhanced validation
            if (typeof $.fn.validate === 'function' && !registerForm.valid()) {
                return;
            }

            // Client-side validation
            const name = $('#registerName').val().trim();
            const email = $('#registerEmail').val().trim();
            const password = $('#registerPassword').val();
            const confirmPassword = $('#registerConfirmPassword').val();

            if (!name || !email || !password || !confirmPassword) {
                abp.notify.error('Please fill in all required fields.', 'Validation Error');
                return;
            }

            if (password !== confirmPassword) {
                abp.notify.error(L('Register:PasswordsDoNotMatch'), L('Validation:ErrorTitle'));
                return;
            }

            if (password.length < 6) {
                abp.notify.error('Password must be at least 6 characters long.', 'Validation Error');
                return;
            }

            registerButton = $(this).find('button[type="submit"]');
            originalRegisterButtonText = registerButton.html();
            registerButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Processing...');

            // FIXED: Correct ABP API register payload
            var currentRegisterData = {
                userName: name,
                emailAddress: email,
                password: password,
                appName: "ProductSelling" // Add your app name if required
            };

            console.log("Attempting registration with data:", {
                userName: name,
                emailAddress: email,
                appName: currentRegisterData.appName
            });

            function performRegistration() {
                // FIXED: Use proper ABP API call
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
                                    message: `Registration failed (${xhr.status}): ${xhr.statusText || error}`,
                                    details: xhr.responseText
                                }
                            };
                        }
                        handleRegisterError(errorData);
                    },
                    complete: function () {
                        if (registerButton) {
                            registerButton.prop('disabled', false).html(originalRegisterButtonText || 'Register');
                        }
                    }
                });
            }

            showTemporaryNotificationAndProceed(
                L('Register:Processing'),
                L('Register:Initiated'),
                800,
                performRegistration
            );
        });
    }

    function handleRegisterSuccess(result, registeredUserData) {
        console.log("Processing registration success:", result);

        abp.notify.success(L('Register:SuccessAndLoggingIn'));

        // Hide register modal
        if (registerModalElement) {
            var modalInstance = bootstrap.Modal.getInstance(registerModalElement);
            if (modalInstance) {
                modalInstance.hide();
            }
        }

        // Attempt auto-login
        setTimeout(() => {
            attemptAutoLogin(registeredUserData.emailAddress, registeredUserData.password);
        }, 1000);
    }

    function handleRegisterError(error) {
        console.error("Processing registration error:", error);

        var errorMessage;

        // Handle ABP error format
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

        // Handle validation errors
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

        abp.notify.error(errorMessage, 'Registration Error');
    }

    function attemptAutoLogin(userNameOrEmail, password) {
        console.log("Attempting auto-login for:", userNameOrEmail);

        var loginData = {
            userNameOrEmailAddress: userNameOrEmail,
            password: password,
            rememberMe: false
        };

        // Use same login logic as main login
        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(loginData),
            headers: getABPHeaders(),
            dataType: 'json',
            timeout: 10000,
            success: function (loginResult) {
                console.log("Auto-login successful:", loginResult);
                abp.notify.success(L('Login:Success'));

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

                abp.notify.warn(errorMessage, L('AutoLogin:WarnTitle'));
            }
        });
    }

    // Enhanced register modal reset
    if (registerModalElement) {
        registerModalElement.addEventListener('hidden.bs.modal', function (event) {
            if (registerForm.length && registerForm[0]) {
                // Reset validation
                if (typeof registerForm.validate === 'function') {
                    try {
                        registerForm.validate().resetForm();
                    } catch (e) {
                        console.warn("Could not reset register validation:", e);
                    }
                }
                // Remove Bootstrap validation classes
                registerForm.find('.is-invalid').removeClass('is-invalid');
                registerForm.find('.invalid-feedback').remove();
                // Reset form
                registerForm[0].reset();
            }
            // Reset button
            if (registerButton) {
                registerButton.prop('disabled', false).html(originalRegisterButtonText || L('Register:ButtonDefault'));
            }
        });
    }
});