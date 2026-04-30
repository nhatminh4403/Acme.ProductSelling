//Depends on: localization.js
(function (global) {
    'use strict';

    const activeNotifications = new Set();
    const DEFAULT_TIMEOUT_MS = 5000;

    function showNotification(message, title, type, options) {
        type = type || 'info';
        options = options || {};
        const key = `${type}|${title}|${message}`;
        if (activeNotifications.has(key)) return;

        activeNotifications.add(key);
        setTimeout(function () {
            activeNotifications.delete(key);
        }, options.timeOut || DEFAULT_TIMEOUT_MS);

        if (typeof abp !== 'undefined' && abp.notify && typeof abp.notify[type] === 'function') {
            abp.notify[type](message, title, options);
        } else {
            // Graceful console fallback when toastr / abp.notify is not ready
            console.log('[Notification][' + type.toUpperCase() + ']', title ? title + ': ' : '', message);
        }
    }


    global.abp = global.abp || {};
    global.abp.notify = global.abp.notify || {};
    global.abp.message = global.abp.message || {};

    function applyNotifyPatch() {
        ['success', 'error', 'info', 'warn'].forEach(function (type) {
            const original = global.abp.notify[type];

            // Guard: don't double-patch if setupComplete fires more than once
            if (original && original._dedupPatched) return;

            const patched = function (message, title, options) {
                const key = `${type}|${title}|${message}`;
                if (activeNotifications.has(key)) return;

                activeNotifications.add(key);
                setTimeout(function () {
                    activeNotifications.delete(key);
                }, (options && options.timeOut) || DEFAULT_TIMEOUT_MS);

                if (typeof original === 'function') {
                    original.call(this, message, title, options);
                } else {
                    console.log('[abp.notify][' + type.toUpperCase() + ']', title ? title + ': ' : '', message);
                }
            };

            patched._dedupPatched = true;
            global.abp.notify[type] = patched;
        });
    }

    if (typeof abp !== 'undefined' && abp.event) {
        abp.event.on('abp.setupComplete', applyNotifyPatch);
    } else {
        // ABP not present or loaded synchronously — patch immediately
        applyNotifyPatch();
    }

    // Expose as a global so other modules can call showNotification() directly
    global.showNotification = showNotification;

}(window));