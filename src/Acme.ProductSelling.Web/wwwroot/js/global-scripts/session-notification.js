//   Depends on: localization.js, notification.js
$(function () {
    'use strict';

    function readAndClear(key) {
        try {
            if (sessionStorage.getItem(key) === 'true') {
                sessionStorage.removeItem(key);
                return true;
            }
        } catch (e) {
            console.warn('[SessionNotification]', e);
        }
        return false;
    }

    if (readAndClear('justLoggedOut')) {
        setTimeout(function () {
            showNotification(L('Logout:Success'), L('Logout:Complete'), 'success');
        }, 500);
    }

    if (readAndClear('justLoggedIn')) {
        setTimeout(function () {
            showNotification(L('Login:Success'), '', 'success');
        }, 500);
    }

    if (readAndClear('justRegistered')) {
        setTimeout(function () {
            showNotification('Chào mừng bạn gia nhập!', 'Đăng ký thành công', 'success');
        }, 500);
    }
});
