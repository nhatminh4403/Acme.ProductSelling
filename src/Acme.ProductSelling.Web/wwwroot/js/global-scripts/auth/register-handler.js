// Depends on: localization.js, notification.js, auth/auth-utils.js

$(function () {
    'use strict';

    const registerModalElement = document.getElementById('registerModal');
    const $registerForm = $('#registerForm');

    if (!$registerForm.length) return;

    let $registerButton;
    let originalRegisterButtonHtml;
    let isRegisterInProgress = false;

    // -------------------------------------------------------------------------
    // Form submission
    // -------------------------------------------------------------------------
    $registerForm.on('submit', function (e) {
        e.preventDefault();

        if (isRegisterInProgress) return;

        if (typeof $.fn.validate === 'function' && !$registerForm.valid()) return;

        const name = $('#registerName').val().trim();
        const surname = $('#registerSurname').val().trim();
        const email = $('#registerEmail').val().trim();
        const password = $('#registerPassword').val();
        const confirmPassword = $('#registerConfirmPassword').val();
        const phoneNumber = $('#registerPhone').val();

        if (!name || !surname || !email || !password || !confirmPassword ) {
            showNotification('Vui lòng điền đầy đủ thông tin bắt buộc.', L('Validation:ErrorTitle'), 'error');
            return;
        }

        if (password !== confirmPassword) {
            showNotification(L('Register:PasswordsDoNotMatch'), L('Validation:ErrorTitle'), 'error');
            return;
        }

        if (password.length < 6) {
            showNotification('Mật khẩu phải có ít nhất 6 ký tự.', L('Validation:ErrorTitle'), 'error');
            return;
        }

        isRegisterInProgress = true;
        $registerButton = $(this).find('button[type="submit"]');
        originalRegisterButtonHtml = AuthUtils.disableButton($registerButton, 'Đang xử lý...');

        showNotification(L('Register:Processing'), L('Register:Initiated'), 'info', { timeOut: 1000 });

        const dto = {
            userName: email,
            emailAddress: email,
            password: password,
            surname: surname,
            name: name,
            appName: 'ProductSelling',
            phoneNumber: phoneNumber,
        };

        // ABP proxy: replaces POST /api/app/account/register
        // acme.productSelling.account is auto-generated from your app service interface
        acme.productSelling.account.account.register(dto)
            .then(function () {
                handleRegisterSuccess(email, password);
            })
            .catch(function (err) {
                handleRegisterError(err && err.error ? err : { error: err });
            })
            .then(function () {
                // always runs (polyfill for .finally)
                isRegisterInProgress = false;
                AuthUtils.restoreButton($registerButton, originalRegisterButtonHtml, L('Register:Button'));
            });
    });

    // -------------------------------------------------------------------------
    // Success — close modal, then auto-login
    // -------------------------------------------------------------------------
    function handleRegisterSuccess(email, password) {
        showNotification(L('Register:SuccessAndLoggingIn'), '', 'success');
        AuthUtils.hideModal(registerModalElement);
        attemptAutoLogin(email, password);
    }

    // -------------------------------------------------------------------------
    // Error
    // -------------------------------------------------------------------------
    function handleRegisterError(error) {
        const message = AuthUtils.extractErrorMessage(error, 'Register:ErrorDefault');
        showNotification(message, 'Lỗi đăng ký', 'error');
    }

    // -------------------------------------------------------------------------
    // Auto-login after successful registration
    // ABP proxy replaces POST /api/account/login
    // -------------------------------------------------------------------------
    function attemptAutoLogin(email, password) {
        showNotification('Đang tự động đăng nhập...', 'Vui lòng đợi', 'info');

        const dto = {
            userNameOrEmailAddress: email,
            password: password,
            rememberMe: false,
        };

        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(dto),
            headers: AuthUtils.getABPHeaders(),
            dataType: 'json',
            timeout: 10000,
            success: function () {
                sessionStorage.setItem('justRegistered', 'true');
                showNotification(L('Login:Success'), '', 'success');
                AuthUtils.syncThenRedirect(1500);
            },
            error: function (xhr) {
                let errorData;
                try {
                    errorData = JSON.parse(xhr.responseText);
                } catch (e) {
                    errorData = { error: { message: 'Lỗi server (' + xhr.status + '): ' + xhr.statusText } };
                }
                const message = AuthUtils.extractErrorMessage(errorData, 'AutoLogin:Error');
                showNotification(message, L('AutoLogin:WarnTitle'), 'warn');
            },
        });
    }

    // -------------------------------------------------------------------------
    // Modal close — reset state
    // -------------------------------------------------------------------------
    if (registerModalElement) {
        registerModalElement.addEventListener('hidden.bs.modal', function () {
            isRegisterInProgress = false;
            AuthUtils.resetForm($registerForm);
            AuthUtils.restoreButton($registerButton, originalRegisterButtonHtml, L('Register:Button'));
        });
    }
});