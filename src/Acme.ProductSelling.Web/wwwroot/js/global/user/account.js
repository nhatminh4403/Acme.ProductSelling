$(document).ready(function () {
    // Tab switching functionality
    $('.tab-button').on('click', function () {
        const tabName = $(this).data('tab');

        // Update active states
        $('.tab-button').removeClass('active');
        $(this).addClass('active');

        $('.tab-content').removeClass('active');
        $(`#${tabName}-tab`).addClass('active');

        // Update URL hash without reload
        history.pushState(null, null, `#${tabName}`);
    });

    // Handle initial hash
    const hash = window.location.hash.substring(1);
    if (hash) {
        $(`.tab-button[data-tab="${hash}"]`).click();
    }

    // Password strength indicator
    $('input[name="PasswordInput.NewPassword"]').on('input', function () {
        const password = $(this).val();
        const strength = calculatePasswordStrength(password);
        updatePasswordStrength(strength);
    });

    // Same as shipping checkbox
    $('#sameAsShipping').on('change', function () {
        if ($(this).is(':checked')) {
            const shippingAddress = $('textarea[name="ProfileInput.ShippingAddress"]').val();
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
    const bar = $('#password-strength-bar');
    const text = $('#password-strength-text');

    bar.removeClass('strength-weak strength-medium strength-strong');

    if (strength <= 2) {
        bar.css('width', '33%').addClass('strength-weak');
        text.text('@L["Account:PasswordStrengthWeak"]').css('color', '#dc3545');
    } else if (strength <= 4) {
        bar.css('width', '66%').addClass('strength-medium');
        text.text('@L["Account:PasswordStrengthMedium"]').css('color', '#ffc107');
    } else {
        bar.css('width', '100%').addClass('strength-strong');
        text.text('@L["Account:PasswordStrengthStrong"]').css('color', '#28a745');
    }
}

function resetForm(formId) {
    $(`#${formId}`)[0].reset();
    $('#password-strength-bar').css('width', '0');
    $('#password-strength-text').text('');
}