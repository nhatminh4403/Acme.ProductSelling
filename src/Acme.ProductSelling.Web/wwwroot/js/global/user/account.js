$(document).ready(function () {
    // Password strength indicator
    const passwordInput = $('input[name="PasswordInput.NewPassword"]');
    if (passwordInput.length > 0) {
        passwordInput.on('input', function () {
            const password = $(this).val();
            const strength = calculateAcctPasswordStrength(password);
            updateAcctPasswordStrength(strength);
        });
    }

    // Same as shipping checkbox (if added back in the future)
    $('#sameAsShipping').on('change', function () {
        if ($(this).is(':checked')) {
            const shippingAddress = $('textarea[name="ProfileInput.ShippingAddress"]').val();
            $('textarea[name="ProfileInput.BillingAddress"]').val(shippingAddress);
        }
    });
});

function calculateAcctPasswordStrength(password) {
    let strength = 0;
    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;
    return strength;
}

function updateAcctPasswordStrength(strength) {
    const bars = $('.acct-pwd-bar');
    const text = $('.acct-pwd-text');
    
    let L;
    try {
        L = abp.localization.getResource('ProductSelling');
    } catch (e) {
        L = function(key) { return key.split(':')[1] || key; };
    }
    
    bars.css('background', '#e5e7eb');
    
    if (strength === 0) {
        text.text('').css('color', '#6b7280');
    } else if (strength <= 2) {
        bars.eq(0).css('background', '#ef4444');
        text.text(L('Account:PasswordStrengthWeak') || 'Weak').css('color', '#ef4444');
    } else if (strength <= 4) {
        bars.eq(0).css('background', '#f59e0b');
        bars.eq(1).css('background', '#f59e0b');
        text.text(L('Account:PasswordStrengthMedium') || 'Medium').css('color', '#f59e0b');
    } else {
        bars.css('background', '#10b981');
        text.text(L('Account:PasswordStrengthStrong') || 'Strong').css('color', '#10b981');
    }
}