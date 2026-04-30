// Depends on: localization.js, notification.js, auth/auth-utils.js

$(function () {
    'use strict';

    const $loginModalElement = $('#loginModal');
    const $loginForm = $('#loginForm');

    if (!$loginForm.length) return;

    let $loginButton;
    let originalLoginButtonHtml;
    let isLoginInProgress = false;

    $loginForm.on('submit', function (e) {
        e.preventDefault();

        if (isLoginInProgress) return;

        if (typeof $.fn.validate === 'function' && !$loginForm.valid()) return;

        const email = $('#loginEmail').val().trim();
        const password = $('#loginPassword').val();

        if (!email || !password) {
            showNotification('Vui lòng điền đầy đủ thông tin bắt buộc.', L('Validation:ErrorTitle'), 'error');
            return;
        }

        isLoginInProgress = true;
        $loginButton = $(this).find('button[type="submit"]');
        originalLoginButtonHtml = AuthUtils.disableButton($loginButton, 'Đang xử lý...');

        showNotification(L('Login:Processing'), L('Login:Initiated'), 'info', { timeOut: 1000 });

        const dto = {
            userNameOrEmailAddress: email,
            password: password,
            rememberMe: $('#rememberMeCheck').is(':checked'),
        };

        $.ajax({
            url: '/api/account/login',
            type: 'POST',
            data: JSON.stringify(dto),
            headers: AuthUtils.getABPHeaders(),
            dataType: 'json',
            timeout: 15000,
            success: function () {
                handleLoginSuccess();
            },
            error: function (xhr) {
                let errorData;
                try {
                    errorData = JSON.parse(xhr.responseText);
                } catch (e) {
                    errorData = { error: { message: 'Lỗi server (' + xhr.status + '): ' + xhr.statusText } };
                }
                handleLoginError(errorData);
            },
            complete: function () {
                isLoginInProgress = false;
                AuthUtils.restoreButton($loginButton, originalLoginButtonHtml, L('Login:Default'));
            },
        });
    });

    // -------------------------------------------------------------------------
    // Success
    // -------------------------------------------------------------------------
    function handleLoginSuccess() {
        AuthUtils.hideModal($loginModalElement[0]);
        sessionStorage.setItem('justLoggedIn', 'true');
        showNotification(L('Login:Success'), '', 'success', { timeOut: 2000 });

        if ($loginButton) {
            $loginButton.html('<i class="fas fa-sync fa-spin"></i> Đang đồng bộ...');
        }

        AuthUtils.syncThenRedirect(1500);
    }

    // -------------------------------------------------------------------------
    // Error
    // -------------------------------------------------------------------------
    function handleLoginError(error) {
        const message = AuthUtils.extractErrorMessage(error, 'Login:Error');
        showNotification(message, L('Login:ErrorTitle'), 'error');
    }

    // -------------------------------------------------------------------------
    // Modal close — reset state
    // -------------------------------------------------------------------------
    if ($loginModalElement.length) {
        $loginModalElement.on('hidden.bs.modal', function () {
            isLoginInProgress = false;
            AuthUtils.resetForm($loginForm);
            AuthUtils.restoreButton($loginButton, originalLoginButtonHtml, L('Login:Default'));
        });
    }
});