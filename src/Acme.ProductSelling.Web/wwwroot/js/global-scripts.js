function handleLoginSuccess(result) {
    console.log("Processing login success:", result);

    // Hide modal if it exists
    if ($loginModalElement.length > 0) {
        try {
            var bsModal = bootstrap.Modal.getInstance($loginModalElement[0]);
            if (bsModal) {
                bsModal.hide();
            }
        } catch (e) {
            console.warn("Could not hide login modal:", e);
        }
    }

    // Enhanced redirect logic
    setTimeout(() => {
        var returnUrl = new URLSearchParams(window.location.search).get('ReturnUrl');
        if (returnUrl) {
            window.location.href = decodeURIComponent(returnUrl);
        } else {
            window.location.reload();
        }
    }, 1000);
}

function handleLoginError(error) {
    console.error("Processing login error:", error);

    var errorMessage;

    // Handle ABP error format
    if (error && error.error) {
        if (error.error.message) {
            errorMessage = error.error.message;
        } else if (error.error.details) {
            errorMessage = error.error.details;
        } else if (typeof error.error === 'string') {
            errorMessage = error.error;
        }
    } else if (error && error.message) {
        errorMessage = error.message;
    } else if (typeof error === 'string') {
        errorMessage = error;
    }

    // Handle validation errors (common ABP pattern)
    if (error && error.error && error.error.validationErrors) {
        var validationMessages = [];
        for (var key in error.error.validationErrors) {
            if (error.error.validationErrors[key]) {
                validationMessages.push(error.error.validationErrors[key].join(', '));
            }
        }
        if (validationMessages.length > 0) {
            errorMessage = validationMessages.join('; ');
        }
    }

    if (!errorMessage) {
        errorMessage = L('Login:ErrorDefault');
    }

    abp.notify.error(errorMessage, L('Login:ErrorTitle'));
}

if (loginForm.length) {
    loginForm.on('submit', function (e) {
        e.preventDefault();

        // Enhanced validation
        if (typeof $.fn.validate === 'function' && !loginForm.valid()) {
            return;
        }

        // Basic client-side validation
        const email = $('#loginEmail').val().trim();
        const password = $('#loginPassword').val();

        if (!email || !password) {
            abp.notify.error('Please fill in all required fields.', 'Validation Error');
            return;
        }

        loginButton = $(this).find('button[type="submit"]');
        originalLoginButtonText = loginButton.html();
        loginButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i> Processing...');

        // FIXED: Correct ABP API login payload
        var loginData = {
            userNameOrEmailAddress: email,
            password: password,
            rememberMe: $('#rememberMeCheck').is(':checked')
        };

        function performLogin() {
            // FIXED: Use proper ABP API call with correct headers
            $.ajax({
                url: '/api/account/login',
                type: 'POST',
                data: JSON.stringify(loginData),
                headers: getABPHeaders(),
                dataType: 'json',
                timeout: 15000,
                success: function (result) {
                    console.log("Login successful:", result);
                    handleLoginSuccess(result);
                },
                error: function (xhr, status, error) {
                    console.error("Login failed:", {
                        status: xhr.status,
                        statusText: xhr.statusText,
                        responseText: xhr.responseText,
                        error: error
                    });

                    let errorData;
                    try {
                        errorData = JSON.parse(xhr.responseText);
                    } catch (e) {
                        errorData = {
                            error: {
                                message: `Server error (${xhr.status}): ${xhr.statusText || error}`,
                                details: xhr.responseText
                            }
                        };
                    }
                    handleLoginError(errorData);
                },
                complete: function () {
                    // Reset button state
                    if (loginButton) {
                        loginButton.prop('disabled', false).html(originalLoginButtonText || 'Login');
                    }
                }
            });
        }

        // Show only one notification for processing
        abp.notify.info(L('Login:Processing'), L('Login:Initiated'), { timeOut: 800 });
        setTimeout(performLogin, 800);
    });
} 