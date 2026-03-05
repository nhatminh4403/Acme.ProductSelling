//Depends on: localization.js, notification.js
(function (global) {
    'use strict';

    // -------------------------------------------------------------------------
    // CSRF + ABP headers
    // Required for raw $.ajax calls to /api/account/login and /api/account/register.
    // These are OpenIddict / Razor Pages endpoints — no JS dynamic proxy exists.
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

    /**
     * Build the headers object required for raw $.ajax calls to ABP endpoints.
     * Includes anti-forgery token and multi-tenancy header when present.
     *
     * @returns {object}
     */
    function getABPHeaders() {
        const headers = {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
        };

        const csrfToken = getCSRFToken();
        if (csrfToken) {
            headers['RequestVerificationToken'] = csrfToken;
            headers['X-CSRF-TOKEN'] = csrfToken;
        }

        if (typeof abp !== 'undefined' && abp.multiTenancy && abp.multiTenancy.getTenantIdCookie) {
            const tenantId = abp.multiTenancy.getTenantIdCookie();
            if (tenantId) headers['__tenant'] = tenantId;
        }

        return headers;
    }

    // -------------------------------------------------------------------------
    // Role-based redirect after successful authentication
    // Uses the ABP proxy for acme.productSelling.account.getRolePrefix()
    // -------------------------------------------------------------------------

    /**
     * Redirect the user to the appropriate page after login / registration.
     * Checks for a ReturnUrl query-string first, then asks the server for the
     * correct role-based prefix via the ABP dynamic proxy.
     */
    function executeRedirect() {
        const returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
        if (returnUrl) {
            window.location.href = decodeURIComponent(returnUrl);
            return;
        }

        // ABP proxy: acme.productSelling.account is auto-generated from IAccountAppService
        const accountProxy = acme &&
            acme.productSelling &&
            acme.productSelling.account;

        if (accountProxy && accountProxy.getRolePrefix) {
            accountProxy.getRolePrefix()
                .then(function (result) {
                    window.location.href = (result && result.hasAdminAccess)
                        ? '/' + result.prefix
                        : '/';
                })
                .catch(function () {
                    window.location.reload();
                });
        } else {
            window.location.reload();
        }
    }

    /**
     * Sync recently-viewed history, then redirect.
     * Handles the case where RecentlyViewedManager is not loaded.
     *
     * @param {number} delayMs
     */
    function syncThenRedirect(delayMs) {
        delayMs = delayMs || 1500;

        if (typeof RecentlyViewedManager !== 'undefined') {
            RecentlyViewedManager.syncWithServer()
                .then(function () { setTimeout(executeRedirect, delayMs); })
                .catch(function () { setTimeout(executeRedirect, delayMs); });
        } else {
            setTimeout(executeRedirect, delayMs);
        }
    }

    /**
     * Extract a human-readable error message from an ABP error response.
     *
     * @param {object|string} error
     * @param {string}        fallbackKey  L() key used when nothing else works
     * @returns {string}
     */
    function extractErrorMessage(error, fallbackKey) {
        if (!error) return L(fallbackKey);

        const validationErrors = error && error.error && error.error.validationErrors;
        if (validationErrors && typeof validationErrors === 'object') {
            const messages = [];
            for (const key in validationErrors) {
                if (Array.isArray(validationErrors[key])) {
                    messages.push(validationErrors[key].join(', '));
                }
            }
            if (messages.length > 0) return messages.join('; ');
        }

        if (error.error && error.error.message) return error.error.message;
        if (error.error && error.error.details) return error.error.details;
        if (typeof error.error === 'string') return error.error;
        if (error.message) return error.message;
        if (typeof error === 'string') return error;

        return L(fallbackKey);
    }

    /**
     * Disable a submit button and show a spinner.
     *
     * @param {jQuery} $button
     * @param {string} [spinnerLabel]
     * @returns {string} Original inner HTML
     */
    function disableButton($button, spinnerLabel) {
        const original = $button.html();
        $button.prop('disabled', true).html(
            '<i class="fas fa-spinner fa-spin me-1"></i> ' + (spinnerLabel || 'Đang xử lý...')
        );
        return original;
    }

    /**
     * Re-enable a submit button.
     *
     * @param {jQuery} $button
     * @param {string} originalHtml
     * @param {string} [fallbackLabel]
     */
    function restoreButton($button, originalHtml, fallbackLabel) {
        if ($button) {
            $button.prop('disabled', false).html(originalHtml || fallbackLabel || 'OK');
        }
    }

    /**
     * Hide a Bootstrap 5 modal safely.
     *
     * @param {Element|null} modalElement
     */
    function hideModal(modalElement) {
        if (!modalElement) return;
        try {
            const instance = bootstrap.Modal.getInstance(modalElement);
            if (instance) instance.hide();
        } catch (e) {
            console.warn('[Auth] Could not hide modal:', e);
        }
    }

    /**
     * Reset a jQuery-validate form and clear Bootstrap validation classes.
     *
     * @param {jQuery} $form
     */
    function resetForm($form) {
        if (!$form || !$form.length || !$form[0]) return;

        if (typeof $form.validate === 'function') {
            try { $form.validate().resetForm(); } catch (e) {
                console.warn('[Auth] Could not reset validate:', e);
            }
        }

        $form.find('.is-invalid').removeClass('is-invalid');
        $form.find('.invalid-feedback').remove();
        $form[0].reset();
    }

    // Expose utilities on a shared namespace
    global.AuthUtils = {
        getABPHeaders: getABPHeaders,
        executeRedirect: executeRedirect,
        syncThenRedirect: syncThenRedirect,
        extractErrorMessage: extractErrorMessage,
        disableButton: disableButton,
        restoreButton: restoreButton,
        hideModal: hideModal,
        resetForm: resetForm,
    };

}(window));
