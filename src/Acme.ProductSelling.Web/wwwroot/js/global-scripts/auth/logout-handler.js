//   Depends on: localization.js, notification.js
$(function () {
    'use strict';

    // -------------------------------------------------------------------------
    // CSRF helper — uses ABP's built-in token if available
    // -------------------------------------------------------------------------
    function getCSRFToken() {
        if (typeof abp !== 'undefined' && abp.security && abp.security.antiForgery) {
            const token = abp.security.antiForgery.getToken();
            if (token) return token;
        }
        return $('input[name="__RequestVerificationToken"]').val()
            || $('meta[name="__RequestVerificationToken"]').attr('content')
            || null;
    }

    // -------------------------------------------------------------------------
    // Wire up logout handlers
    // -------------------------------------------------------------------------
    function setupLogoutHandlers() {
        const selector = [
            'a[href^="/Account/Logout"]',
            'form[action*="/Account/Logout"] button[type="submit"]',
        ].join(', ');

        document.querySelectorAll(selector).forEach(function (element) {
            element.addEventListener('click', function (e) {
                e.preventDefault();

                let href;

                if (element.tagName === 'A') {
                    href = element.getAttribute('href');
                } else {
                    const form = element.closest('form');
                    href = (form && form.getAttribute('action')) || '/Account/Logout';
                    const returnInput = form && form.querySelector('input[name="returnUrl"]');
                    if (returnInput) {
                        href += '?returnUrl=' + encodeURIComponent(returnInput.value);
                    }
                }

                // Ensure returnUrl is set so ABP redirects cleanly
                if (href.indexOf('returnUrl') === -1) {
                    href += (href.indexOf('?') === -1 ? '?' : '&') + 'returnUrl=/';
                }

                if (typeof abp !== 'undefined' && abp.notify) {
                    abp.notify.info(L('Logout:Processing'), L('Logout:PleaseWait'));
                }

                try { sessionStorage.setItem('justLoggedOut', 'true'); } catch (_) { /* ignore */ }

                // Submit a dynamically created form to keep the CSRF token
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

    // ABP raises 'abp.setupComplete' when its bootstrap is done; fall back to
    // jQuery ready for pages that load ABP synchronously.
    if (typeof abp !== 'undefined' && abp.event) {
        abp.event.on('abp.setupComplete', setupLogoutHandlers);
    } else {
        $(document).ready(setupLogoutHandlers);
    }

    // Post-logout toast is handled exclusively by session-notification.js
    // to avoid double-firing. Do not add sessionStorage checks here.
});
