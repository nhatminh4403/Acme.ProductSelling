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
$(function () {
    var loginModalElement = document.getElementById('loginModal');
    var loginForm = $('#loginForm');
    safeInitModal(loginModalElement);
    loginForm.on('submit', function (e) {
        e.preventDefault();

        // Validate form if jQuery validate is available
        if ($.validator && !loginForm.valid()) {
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

        // Implement custom error handling
        if (typeof abp === 'undefined') {
            // Define abp if it doesn't exist
            window.abp = {};
        }

        // Ensure error handlers are properly defined
        if (!abp.message) {
            abp.message = {};
        }

        if (!abp.message.error) {
            abp.message.error = function (message) {
                console.error("Error:", message);
                alert("Error: " + message);
            };
        }

        if (!abp.notify) {
            abp.notify = {};
        }

        if (!abp.notify.error) {
            abp.notify.error = function (message) {
                console.error("Notification error:", message);
                alert("Error: " + message);
            };
        }

        // Use standard AJAX for consistent behavior
        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(loginData),
            contentType: 'application/json',
            success: handleLoginSuccess,
            error: function (xhr) {
                console.log("Login error response:", xhr);
                var errorObj = xhr.responseJSON || { error: { message: 'Login failed' } };
                handleLoginError(errorObj);
            }
        });
    });

    // Handle successful login
    function handleLoginSuccess(result) {
        console.log("Login successful:", result);

        if (loginModal) {
            loginModal.hide();
        }

        if (typeof abp !== 'undefined' && abp.notify && abp.notify.success) {
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
            errorMessage = 'Login failed. Please check your credentials.';
        }

        if (typeof abp !== 'undefined' && abp.notify && abp.notify.error) {
            abp.notify.error(errorMessage);
        } else {
            alert('Error: ' + errorMessage);
        }

        // Reset button state
        var loginButton = loginForm.find('button[type="submit"]');
        loginButton.prop('disabled', false).html('Login');
    }

    // Optional: Reset form when modal is closed
    if (loginModalElement) {
        loginModalElement.addEventListener('hidden.bs.modal', function (event) {
            loginForm.find('.text-danger').remove();
            loginForm[0].reset();
            var loginButton = loginForm.find('button[type="submit"]');
            loginButton.prop('disabled', false).html('Login');
        });
    }
});

$(function () {
    var registerModalElement = document.getElementById('registerModal');
    var registerForm = $('#registerForm');
    registerForm.on('submit', function (e) {
        e.preventDefault();

        if (!registerForm.valid()) { // jQuery Validate
            return;
        }

        // Kiểm tra mật khẩu khớp nhau
        var password = $('#registerPassword').val();
        var confirmPassword = $('#registerConfirmPassword').val();
        if (password !== confirmPassword) {
            abp.notify.error('Passwords do not match.');
            // Hiển thị lỗi validation cụ thể hơn nếu muốn
            return;
        }

        var registerButton = $(this).find('button[type="submit"]');
        var originalButtonText = registerButton.html();
        registerButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Registering...');


        var registerData = {
            appName: 'ProductSelling', // Thay bằng tên ứng dụng của bạn (quan trọng)
            emailAddress: $('#registerEmail').val(),
            userName: $('#registerName').val(), // ABP thường dùng email làm username, nhưng cần kiểm tra lại DTO
            password: password,
           
            extraProperties: { // Nếu cần gửi thêm thông tin như Tên đầy đủ
                
            }
        };
        // Check if abp proxy is available and fallback otherwise
        if (typeof abp !== 'undefined' && abp.auth && abp.auth.login) {
            // Use direct ABP API if available
            abp.auth.register(registerData)
                .then(handleRegisterSuccess)
                .catch(handleRegisterError);
        }
        else if (typeof abp !== 'undefined' && abp.ajax) {
            // Use ABP AJAX if available
            setTimeout(function () {
                abp.ajax({
                    url: '/api/account/register',
                    type: 'POST',
                    data: JSON.stringify(registerData),
                    contentType: 'application/json'
                })
                    .then(handleRegisterSuccess)
                    .catch(handleRegisterError);
            }, 2000);

        }
        else {
            // Fallback to standard AJAX
            $.ajax({
                url: '/api/account/register',
                type: 'POST',
                data: JSON.stringify(registerData),
                contentType: 'application/json',
                success: handleRegisterSuccess,
                error: function (xhr) {
                    handleLoginError(xhr.responseJSON || { error: { message: 'register failed' } });
                }
            });
        }
        function handleRegisterSuccess(result) {
            loginModal.hide();
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
        }

        // Handle login errors
        function handleRegisterError(error) {
            var errorMessage = error?.error?.message || error?.message || 'Login failed. Please check your credentials.';

            if (typeof abp !== 'undefined' && abp.notify) {
                abp.notify.error(errorMessage);
            } else {
                alert('Error: ' + errorMessage);
            }

            console.error("Login error:", error);
            loginButton.prop('disabled', false).html(originalButtonText || 'Login');
        }

    });
    if (registerModalElement) {
       
        registerModalElement.addEventListener('hidden.bs.modal', function (event) {
            registerForm.find('.text-danger').remove();
            registerForm[0].reset();
            var registerButton = registerForm.find('button[type="submit"]');
            registerButton.prop('disabled', false).html('Register');
        });
    }

});

