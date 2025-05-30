$(document).ready(function () {
    setupCustomLogout();
});
function setupCustomLogout() {
    document.querySelectorAll('a[href^="/Account/Logout"]').forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();

            // Show immediate feedback
            if (typeof abp !== 'undefined' && abp.notify) {
                abp.notify.info('Logging out...', 'Please wait');
            }

            setTimeout(function () {
                // Clear storage
                try {
                    localStorage.clear();
                    sessionStorage.clear();
                } catch (e) {
                    console.warn('Could not clear storage:', e);
                }

                // Clear ABP auth if available
                if (typeof abp !== 'undefined' && abp.auth) {
                    abp.auth.clearToken();
                }

                // Method 1: Try using fetch to logout silently
                fetch('/Account/Logout', {
                    method: 'POST',
                    credentials: 'same-origin',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || '',
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: 'returnUrl=' + encodeURIComponent('/')
                })
                    .then(() => {
                        // Success notification
                        if (typeof abp !== 'undefined' && abp.notify) {
                            abp.notify.success('You have been logged out successfully.', 'Logout Complete');


                            setTimeout(() => {
                                window.location.replace('/');
                            }, 2000);
                        } else {
                            window.location.replace('/');
                        }
                    })
                    .catch(() => {
                        console.log('Fetch failed, using fallback method');

                        const originalLocation = window.location;
                        let redirectBlocked = false;

                        Object.defineProperty(window, 'location', {
                            get: function () { return originalLocation; },
                            set: function (url) {
                                if (url.includes('/Account/Login') && !redirectBlocked) {
                                    console.log('Blocked redirect to login page');
                                    redirectBlocked = true;
                                    setTimeout(() => {
                                        originalLocation.href = '/';
                                    }, 100);
                                } else if (!redirectBlocked) {
                                    originalLocation.href = url;
                                }
                            }
                        });

                        originalLocation.href = '/Account/Logout?returnUrl=' + encodeURIComponent('/');
                    });

            }, 500);
        });
    });
}