/* Your Global Scripts */
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

    menuContainer.addEventListener('mouseenter', showMainMenu);
    menuContainer.addEventListener('mouseleave', startHideMainMenu);

    // --- Submenu (Mega Menu) Logic ---
    if (mainDropdownMenu) {
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
    }

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
});
//logout
$(function () {
    abp.event.on('abp.setupComplete', function () {
        
        document.querySelectorAll('a[href^="/Account/Logout"]').forEach(function (link) {
            link.addEventListener('click', function (e) {
                e.preventDefault();

                

                let href = link.getAttribute('href');

                if (href.indexOf('returnUrl') === -1) {
                    href += (href.indexOf('?') === -1 ? '?' : '&') + 'returnUrl=/';
                }

                setTimeout(function () {

                    abp.ajax({
                        url: href,
                        type: 'GET',
                        dataType: 'json',
                        success: function () {
                            abp.notify.success(
                                'You are being logged out. Redirecting to home page...',
                                'Logging Out'
                            );
                            window.location.href = '/';
                        },
                        error: function () {
                            window.location.href = '/';
                        }
                    });
                }, 1000);
            });
        });
    });
});
/*
//login
$(function () {
    // Properly define the loginModal variable using jQuery
    var $loginModalElement = $('#loginModal');
    var loginModal = new bootstrap.Modal($loginModalElement[0], {
        keyboard: false,
        backdrop: 'static'
    });
    var loginForm = $('#loginForm');

    // Only initialize if the element exists
    function safeInitModal(element) {
        if (element && typeof bootstrap !== 'undefined' && bootstrap.Modal) {
            return new bootstrap.Modal(element);
        }
        return null;
    }

    // Ensure abp object and its methods exist
    window.abp = window.abp || {};
    abp.notify = abp.notify || {};
    abp.message = abp.message || {};

    // Implement missing notify methods
    abp.notify.success = abp.notify.success || function (message) {
        console.log("Success:", message);
        alert("Success: " + message);
    };

    abp.notify.error = abp.notify.error || function (message) {
        console.error("Error:", message);
        alert("Error: " + message);
    };

    abp.message.error = abp.message.error || function (message) {
        console.error("Message Error:", message);
        alert("Error: " + message);
    };

    // Handle form submission
    loginForm.on('submit', function (e) {
        e.preventDefault();

        // Validate form if jQuery validate is available
        if ($.fn.validate && !loginForm.valid()) {
            return;
        }

        var loginButton = $(this).find('button[type="submit"]');
        var originalButtonText = loginButton.html();
        loginButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Logging in...');

        var loginData = {
            userNameOrEmailAddress: $('#loginEmail').val(),
            password: $('#loginPassword').val(),
            rememberMe: $('#rememberMeCheck').is(':checked')
        };
        console.log("Raw login data before sending:", loginData); 
        // Log the data being sent (for debugging - remove in production)
        

        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(loginData),
            contentType: 'application/json',
            dataType: 'json',
            success: function (result) {
                handleLoginSuccess(result);
            },
            error: function (xhr, status, error) {
                console.log("Status:", xhr.status);
                console.log("Status text:", xhr.statusText);
                console.log("ResponseText:", xhr.responseText);

                try {
                    var errorData = JSON.parse(xhr.responseText);
                    handleLoginError(errorData);
                } catch (e) {
                    handleLoginError({
                        error: {
                            message: "Could not process login. Server returned: " + xhr.status + " " + xhr.statusText
                        }
                    });
                }
            },
            complete: function () {
                // Reset button regardless of success/error
                loginButton.prop('disabled', false).html(originalButtonText);
            }
        });
    });

    // Handle successful login
    function handleLoginSuccess(result) {
        console.log("Login successful:", result);

        if ($loginModalElement.length > 0) {
            try {
                var bsModal = bootstrap.Modal.getInstance($loginModalElement[0]);
                if (bsModal) {
                    bsModal.hide();
                }
            } catch (e) {
                console.warn("Could not hide modal:", e);
            }
        }

        abp.notify.success('Login successful!');

        var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
        if (returnUrl) {
            window.location.href = decodeURIComponent(returnUrl);
        } else {
            window.location.reload();
        }
    }

    // Handle login errors
    function handleLoginError(error) {
        console.error("Processing login error:", error);

        var errorMessage;
        if (error && error.error && error.error.message) {
            errorMessage = error.error.message;
        } else if (error && error.message) {
            errorMessage = error.message;
        } else {
            errorMessage = 'Login failed. Please check your credentials and try again.';
        }

        abp.notify.error(errorMessage);
    }

    // Optional: Reset form when modal is closed
    $loginModalElement.on('hidden.bs.modal', function (event) {
        loginForm.find('.text-danger').remove();
        loginForm[0].reset();
        var loginButton = loginForm.find('button[type="submit"]');
        loginButton.prop('disabled', false).html('Login');
    });
});
//register
$(function () {
    var registerModalElement = document.getElementById('registerModal');
    var registerForm = $('#registerForm');
    var registerButton; // Di chuyển ra ngoài để có thể truy cập từ các hàm khác nếu cần
    var originalButtonText; // Di chuyển ra ngoài

    registerForm.on('submit', function (e) {
        e.preventDefault();

        if ($.fn.validate && !registerForm.valid()) { // jQuery Validate
            return;
        }

        // Kiểm tra mật khẩu khớp nhau
        var password = $('#registerPassword').val();
        var confirmPassword = $('#registerConfirmPassword').val();
        if (password !== confirmPassword) {
            abp.notify.error('Passwords do not match.');
            return;
        }

        registerButton = $(this).find('button[type="submit"]'); // Gán ở đây
        originalButtonText = registerButton.html(); // Gán ở đây
        registerButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Registering...');

        // Thu thập dữ liệu đăng ký một lần và truyền nó đi
        var currentRegisterData = {
            appName: 'ProductSelling',
            emailAddress: $('#registerEmail').val(),
            userName: $('#registerName').val(),
            password: password,
            extraProperties: {}
        };

        // Check if abp proxy is available and fallback otherwise
        if (typeof abp !== 'undefined' && abp.auth && typeof abp.auth.register === 'function') {
            abp.auth.register(currentRegisterData)
                .then(function (result) { handleRegisterSuccess(result, currentRegisterData); }) // Truyền currentRegisterData
                .catch(handleRegisterError);
        } else if (typeof abp !== 'undefined' && typeof abp.ajax === 'function') {
            // setTimeout không cần thiết ở đây trừ khi có lý do cụ thể
            abp.ajax({
                url: '/api/account/register',
                type: 'POST',
                data: JSON.stringify(currentRegisterData),
                contentType: 'application/json'
            })
                .then(function (result) { handleRegisterSuccess(result, currentRegisterData); }) // Truyền currentRegisterData
                .catch(handleRegisterError);
        } else {
            $.ajax({
                url: '/api/account/register',
                type: 'POST',
                data: JSON.stringify(currentRegisterData),
                contentType: 'application/json',
                success: function (result) { handleRegisterSuccess(result, currentRegisterData); }, // Truyền currentRegisterData
                error: function (xhr) {
                    // Giả sử handleLoginError có thể xử lý lỗi từ register
                    handleRegisterError(xhr.responseJSON || { error: { message: 'Registration failed due to an unknown error.' } });
                }
            });
        }
    });

    function handleRegisterSuccess(result, registeredUserData) {
        // registeredUserData chứa emailAddress và password đã dùng để đăng ký
        if (typeof abp !== 'undefined' && abp.notify) {
            abp.notify.success('Registration successful! Attempting to log you in...');
        } else {
            alert('Registration successful! Attempting to log you in...');
        }

        // Ẩn form đăng ký hoặc đóng modal
        if (registerModalElement) {
            var modalInstance = bootstrap.Modal.getInstance(registerModalElement);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
        // registerForm.hide(); // Nếu không phải modal

        // Tiến hành tự động đăng nhập
        attemptAutoLogin(registeredUserData.emailAddress, registeredUserData.password);
    }

    function attemptAutoLogin(userNameOrEmail, password) {
        var loginData = {
            userNameOrEmailAddress: userNameOrEmail,
            password: password,
            rememberMe: false // Hoặc true, tùy theo logic bạn muốn
        };

        var loginPromise;

        if (typeof abp !== 'undefined' && abp.auth && typeof abp.auth.login === 'function') {
            loginPromise = abp.auth.login(loginData);
        } else if (typeof abp !== 'undefined' && typeof abp.ajax === 'function') {
            loginPromise = abp.ajax({
                url: '/api/account/login', // Đảm bảo URL này đúng
                type: 'POST',
                data: JSON.stringify(loginData),
                contentType: 'application/json'
            });
        } else {
            loginPromise = $.ajax({
                url: '/api/account/login',
                type: 'POST',
                data: JSON.stringify(loginData),
                contentType: 'application/json'
            });
        }

        loginPromise
            .then(function (loginResult) {
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.success('Login successful!');
                } else {
                    alert('Login successful!');
                }

                var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
                if (returnUrl) {
                    window.location.href = decodeURIComponent(returnUrl);
                } else {
                    window.location.reload();
                }
            })
            .catch(function (error) {
                var errorMessage = error?.responseJSON?.error?.message || error?.error?.message || error?.message || 'Registration was successful, but auto-login failed. Please log in manually.';
                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.error(errorMessage);
                } else {
                    alert('Error: ' + errorMessage);
                }
                console.error("Auto-login error:", error);
                if (registerButton) {
                    registerButton.prop('disabled', false).html(originalButtonText || 'Register');
                }
            });
    }


    function handleRegisterError(error) {
        var errorMessage = error?.responseJSON?.error?.message || error?.error?.message || error?.message || 'Registration failed. Please check your input and try again.';

        if (typeof abp !== 'undefined' && abp.notify) {
            abp.notify.error(errorMessage);
        } else {
            alert('Error: ' + errorMessage);
        }

        console.error("Registration error:", error);
        if (registerButton) { // Đảm bảo registerButton đã được khởi tạo
            registerButton.prop('disabled', false).html(originalButtonText || 'Register');
        }
    }

    if (registerModalElement) {
        registerModalElement.addEventListener('hidden.bs.modal', function (event) {
            if (registerForm && registerForm[0]) {
                registerForm.find('.text-danger').remove(); // Xóa thông báo lỗi validation (nếu có)
                registerForm[0].reset(); // Reset các trường của form
            }
            if (registerButton) { // Reset nút nếu modal bị đóng bởi người dùng
                registerButton.prop('disabled', false).html(originalButtonText || 'Register');
            }
        });
    }
});
*/


$(function () {

    // --- COMMON HELPER FUNCTION ---
    /**
     * Shows a temporary ABP notification and then executes an action after a delay.
     * @param {string} message The message for the notification.
     * @param {string} title The title for the notification.
     * @param {number} delayMilliseconds The delay in milliseconds before executing the action.
     * @param {function} actionCallback The function to call after the delay.
     * @param {object} [notificationOptions={}] Optional options for abp.notify.info.
    */

    const L = function (key, resourceName) {
        // Default resource name if not provided, adjust if your project has a different default
        const defaultResourceName = 'ProductSelling'; // <<<< THAY ĐỔI TÊN RESOURCE NÀY
        resourceName = resourceName || defaultResourceName;

        if (typeof abp === 'undefined' || !abp.localization || !abp.localization.localize) {
            console.warn("abp.localization.localize is not available. Falling back to key: " + key);
            return key; // Fallback if localization is not available
        }

        return abp.localization.localize(key, resourceName);
    };

    const L_Button_Processing = L('Button:Processing');
    function showTemporaryNotificationAndProceed(message, title, delayMilliseconds, actionCallback, notificationOptions = {}) {
        if (typeof abp !== 'undefined' && abp.notify && abp.notify.info) {
            const mergedOptions = { timeOut: delayMilliseconds, ...notificationOptions };
            abp.notify.info(message, title, mergedOptions);
        } else {
            console.log((title ? title + ": " : "") + message); // Fallback
        }

        setTimeout(function () {
            actionCallback();
        }, delayMilliseconds);
    }

    // --- GLOBAL ABP NOTIFY FALLBACKS (if not already implemented elsewhere) ---
    // Ensure abp object and its methods exist (minimal polyfill if needed)
    window.abp = window.abp || {};
    abp.notify = abp.notify || {};
    abp.message = abp.message || {};

    if (typeof abp.notify.success !== 'function') {
        abp.notify.success = function (message, title) {
            console.log("Success (Fallback):", (title ? title + ": " : "") + message);
            alert("Success: " + message);
        };
    }
    if (typeof abp.notify.error !== 'function') {
        abp.notify.error = function (message, title) {
            console.error("Error (Fallback):", (title ? title + ": " : "") + message);
            alert("Error: " + message);
        };
    }
    if (typeof abp.notify.info !== 'function') {
        abp.notify.info = function (message, title) {
            console.info("Info (Fallback):", (title ? title + ": " : "") + message);
            alert("Info: " + message);
        };
    }
    if (typeof abp.notify.warn !== 'function') {
        abp.notify.warn = function (message, title) {
            console.warn("Warning (Fallback):", (title ? title + ": " : "") + message);
            alert("Warning: " + message);
        };
    }


    // --- LOGIN LOGIC ---
    var $loginModalElement = $('#loginModal');
    var loginForm = $('#loginForm');
    var loginButton, originalLoginButtonText;

    if (loginForm.length) { // Only bind if the form exists
        loginForm.on('submit', function (e) {
            e.preventDefault();

            if (typeof $.fn.validate === 'function' && !loginForm.valid()) {
                return;
            }

            loginButton = $(this).find('button[type="submit"]');
            originalLoginButtonText = loginButton.html();
            loginButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Processing...');

            var loginData = {
                userNameOrEmailAddress: $('#loginEmail').val(),
                password: $('#loginPassword').val(),
                rememberMe: $('#rememberMeCheck').is(':checked')
            };

            function performLogin() {
                var loginPromise;
                if (typeof abp !== 'undefined' && abp.auth && typeof abp.auth.login === 'function') {
                    loginPromise = abp.auth.login(loginData);
                } else if (typeof abp !== 'undefined' && abp.ajax && typeof abp.ajax === 'function') {
                    loginPromise = abp.ajax({
                        url: '/api/account/login', type: 'POST',
                        data: JSON.stringify(loginData), contentType: 'application/json'
                    });
                } else {
                    loginPromise = $.ajax({
                        url: '/api/account/login', type: 'POST',
                        data: JSON.stringify(loginData), contentType: 'application/json'
                    });
                }

                loginPromise
                    .then(handleLoginSuccess)
                    .catch(handleLoginError);
            }

            showTemporaryNotificationAndProceed(
                L('Login:Processing'),
                L('Login:Initiated'),
                1500, // 1.5 seconds delay
                performLogin
            );
        });
    }


    function handleLoginSuccess(result) {
        abp.notify.success(L('Login:Success'));

        var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
        if (returnUrl) {
            window.location.href = decodeURIComponent(returnUrl);
        } else {
            window.location.reload();
        }
    }

    function handleLoginError(error) {
        var errorMessage = error?.responseJSON?.error?.message || error?.error?.message || error?.message || L('Login:ErrorDefault');
        abp.notify.error(errorMessage, L('Login:ErrorTitle', 'AbpUi'));
        abp.notify.error(errorMessage, 'Login Error');
        console.error("Login error:", error);
        if (loginButton) {
            loginButton.prop('disabled', false).html(originalLoginButtonText || L('Login:ButtonDefault'));
        }
    }

    if ($loginModalElement.length) {
        $loginModalElement.on('hidden.bs.modal', function (event) {
            if (loginForm.length && loginForm[0]) {
                if (typeof loginForm.validate === 'function') { // Reset jQuery Validate errors
                    loginForm.validate().resetForm();
                }
                loginForm.find('.is-invalid').removeClass('is-invalid'); // Remove Bootstrap validation classes
                loginForm.find('.invalid-feedback').remove(); // Remove Bootstrap error messages
                loginForm[0].reset();
            }
            if (loginButton) {
                loginButton.prop('disabled', false).html(originalLoginButtonText || L('Login:ButtonDefault'));
            }
        });
    }

    // --- REGISTRATION LOGIC ---
    var registerModalElement = document.getElementById('registerModal'); // Use getElementById if you have it
    var registerForm = $('#registerForm');
    var registerButton, originalRegisterButtonText;

    if (registerForm.length) { // Only bind if the form exists
        registerForm.on('submit', function (e) {
            e.preventDefault();

            if (typeof $.fn.validate === 'function' && !registerForm.valid()) {
                return;
            }

            var password = $('#registerPassword').val();
            var confirmPassword = $('#registerConfirmPassword').val();
            if (password !== confirmPassword) {
                abp.notify.error(L('Register:PasswordsDoNotMatch'), L('Validation:ErrorTitle', 'AbpUi'));
                return;
            }

            registerButton = $(this).find('button[type="submit"]');
            originalRegisterButtonText = registerButton.html();
            registerButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Processing...');

            var currentRegisterData = {
                appName: 'ProductSelling', // Or get dynamically if needed
                emailAddress: $('#registerEmail').val(),
                userName: $('#registerName').val(),
                password: password,
                extraProperties: {}
            };

            function performRegistration() {
                var registerPromise;
                if (typeof abp !== 'undefined' && abp.auth && typeof abp.auth.register === 'function') {
                    registerPromise = abp.auth.register(currentRegisterData);
                } else if (typeof abp !== 'undefined' && abp.ajax && typeof abp.ajax === 'function') {
                    registerPromise = abp.ajax({
                        url: '/api/account/register', type: 'POST',
                        data: JSON.stringify(currentRegisterData), contentType: 'application/json'
                    });
                } else {
                    registerPromise = $.ajax({
                        url: '/api/account/register', type: 'POST',
                        data: JSON.stringify(currentRegisterData), contentType: 'application/json'
                    });
                }

                registerPromise
                    .then(function (result) { handleRegisterSuccess(result, currentRegisterData); })
                    .catch(handleRegisterError);
            }

            showTemporaryNotificationAndProceed(
                L('Register:Processing'),
                L('Register:Initiated'),
                1500, // 1.5 seconds delay
                performRegistration
            );
        });
    }

    function handleRegisterSuccess(result, registeredUserData) {
        abp.notify.success(L('Register:SuccessAndLoggingIn'));

        if (registerModalElement) {
            var modalInstance = bootstrap.Modal.getInstance(registerModalElement);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
        attemptAutoLogin(registeredUserData.emailAddress, registeredUserData.password);
    }

    function handleRegisterError(error) {
        var errorMessage = error?.responseJSON?.error?.message || error?.error?.message || error?.message || L('Register:ErrorDefault');
        abp.notify.error(errorMessage, 'Registration Error');
        console.error("Registration error:", error);
        if (registerButton) {
            registerButton.prop('disabled', false).html(originalRegisterButtonText || L('Register:ButtonDefault'));
        }
    }

    function attemptAutoLogin(userNameOrEmail, password) {
        var loginData = {
            userNameOrEmailAddress: userNameOrEmail,
            password: password,
            rememberMe: false // Or true, depending on your preference for auto-login
        };
        var loginPromise;

        if (typeof abp !== 'undefined' && abp.auth && typeof abp.auth.login === 'function') {
            loginPromise = abp.auth.login(loginData);
        } else if (typeof abp !== 'undefined' && abp.ajax && typeof abp.ajax === 'function') {
            loginPromise = abp.ajax({
                url: '/api/account/login', type: 'POST',
                data: JSON.stringify(loginData), contentType: 'application/json'
            });
        } else {
            loginPromise = $.ajax({
                url: '/api/account/login', type: 'POST',
                data: JSON.stringify(loginData), contentType: 'application/json'
            });
        }

        loginPromise
            .then(function (loginResult) {
                abp.notify.success(L('Login:Success'));
                var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
                if (returnUrl) {
                    window.location.href = decodeURIComponent(returnUrl);
                } else {
                    window.location.reload();
                }
            })
            .catch(function (error) {
                var errorMessage = error?.responseJSON?.error?.message || error?.error?.message || error?.message || L('AutoLogin:ErrorDefault');
                abp.notify.warn(errorMessage, L('AutoLogin:WarnTitle'));
                console.error("Auto-login error:", error);

                if (registerButton) {
                    registerButton.prop('disabled', false).html(originalRegisterButtonText || L('Register:ButtonDefault'));;
                }

            });
    }

    if (registerModalElement) {
        registerModalElement.addEventListener('hidden.bs.modal', function (event) {
            if (registerForm.length && registerForm[0]) {
                if (typeof registerForm.validate === 'function') { // Reset jQuery Validate errors
                    registerForm.validate().resetForm();
                }
                registerForm.find('.is-invalid').removeClass('is-invalid'); // Remove Bootstrap validation classes
                registerForm.find('.invalid-feedback').remove(); // Remove Bootstrap error messages
                registerForm[0].reset();
            }
            if (registerButton) {
                registerButton.prop('disabled', false).html(originalRegisterButtonText || L('Register:ButtonDefault'));
            }
        });
    }

});