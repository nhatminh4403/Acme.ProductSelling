$(document).ready(function () {
    // Password strength indicator
    $('input[name="PasswordInput.NewPassword"]').on('input', function () {
        const password = $(this).val();
        const strength = calculatePasswordStrength(password);
        updatePasswordStrength(strength);
    });

    // Same as shipping checkbox (if used anywhere)
    $('#sameAsShipping').on('change', function () {
        if ($(this).is(':checked')) {
            const shippingAddress = $('textarea[name="ShippingAddressInput.ShippingAddress"], textarea[name="ProfileInput.ShippingAddress"]').val();
            $('textarea[name="ProfileInput.BillingAddress"]').val(shippingAddress);
        }
    });
});

function calculatePasswordStrength(password) {
    let strength = 0;

    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;

    return strength;
}

function updatePasswordStrength(strength) {
    const L = abp.localization.getResource('ProductSelling');
    const bar = $('#password-strength-bar');
    const text = $('#password-strength-text');

    bar.removeClass('strength-weak strength-medium strength-strong');

    if (strength === 0) {
        bar.css('width', '0');
        text.text('');
    } else if (strength <= 2) {
        bar.css('width', '33%').addClass('strength-weak');
        text.text(L("Account:PasswordStrengthWeak")).css('color', '#ef4444');
    } else if (strength <= 4) {
        bar.css('width', '66%').addClass('strength-medium');
        text.text(L("Account:PasswordStrengthMedium")).css('color', '#f59e0b');
    } else {
        bar.css('width', '100%').addClass('strength-strong');
        text.text(L("Account:PasswordStrengthStrong")).css('color', '#10b981');
    }
}

function resetForm(formId) {
    $(`#${formId}`)[0].reset();
    $('#password-strength-bar').css('width', '0');
    $('#password-strength-text').text('');
}