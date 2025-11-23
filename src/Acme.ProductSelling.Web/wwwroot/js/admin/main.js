document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('toggleBtn');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    const overlay = document.getElementById('overlay');
    const accountBtn = document.getElementById('accountBtn');
    const accountDropdown = document.getElementById('accountDropdown');
    const accountArrow = document.getElementById('accountArrow');
    const logoutBtn = document.getElementById('logout-btn');

    // Apply saved state to elements
    const savedState = sessionStorage.getItem('sidebarCollapsed');
    if (savedState === 'true') {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('expanded');
    }

    setTimeout(() => {
        const preloadStyle = document.getElementById('sidebar-preload-style');
        if (preloadStyle) {
            preloadStyle.remove();
        }
    }, 100);

    toggleBtn.addEventListener('click', function () {
        if (window.innerWidth <= 768) {
            sidebar.classList.toggle('mobile-open');
            overlay.classList.toggle('active');
        } else {
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('expanded');

            const isCollapsed = sidebar.classList.contains('collapsed');
            sessionStorage.setItem('sidebarCollapsed', isCollapsed);
        }
    });

    overlay.addEventListener('click', function () {
        sidebar.classList.remove('mobile-open');
        overlay.classList.remove('active');
    });

    accountBtn.addEventListener('click', function (e) {
        e.stopPropagation();
        accountDropdown.classList.toggle('show');
        accountArrow.classList.toggle('rotated');
    });

    document.addEventListener('click', function (e) {
        if (!accountBtn.contains(e.target) && !accountDropdown.contains(e.target)) {
            accountDropdown.classList.remove('show');
            accountArrow.classList.remove('rotated');
        }
    });

    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('mobile-open');
            overlay.classList.remove('active');

            const savedState = sessionStorage.getItem('sidebarCollapsed');
            if (savedState === 'true') {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
            } else {
                sidebar.classList.remove('collapsed');
                mainContent.classList.remove('expanded');
            }
        }
    });

    if (logoutBtn) {
        logoutBtn.addEventListener('click', handleLogout);
    }

    // Handle header dropdown logout button
    const headerLogoutBtn = document.querySelector('#accountDropdown .dropdown-item-custom.danger');
    if (headerLogoutBtn) {
        headerLogoutBtn.addEventListener('click', function (e) {
            e.preventDefault();
            handleLogout();
        });
    }
});

function toggleSubmenu(element) {
    const submenu = element.nextElementSibling;
    const arrow = element.querySelector('.arrow-icon');
    const parentMenuItem = element.closest('.menu-item');

    if (submenu && submenu.classList.contains('submenu')) {
        submenu.classList.toggle('open');
    }

    if (arrow) {
        arrow.classList.toggle('rotated');
    }

    if (parentMenuItem) {
        parentMenuItem.classList.toggle('open');
    }
}

function handleLogout() {
    if (typeof abp !== 'undefined' && abp.message && abp.message.confirm) {
        abp.message.confirm(
            'Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?',
            'Đăng xuất',
            function (isConfirmed) {
                if (isConfirmed) {
                    performLogout();
                }
            },
            {
                confirmButtonText: 'Đăng xuất',
                cancelButtonText: 'Hủy',
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d',
                icon: 'question'
            }
        );
    } else {
        // Fallback
        if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
            performLogout();
        }
    }
}

function performLogout() {
    if (typeof abp !== 'undefined' && abp.notify) {
        abp.notify.info('Đang đăng xuất...', 'Vui lòng đợi');
    }

    try {
        sessionStorage.setItem('justLoggedOut', 'true');
    } catch (e) {
        console.warn('Could not set sessionStorage:', e);
    }

    // Use GET request with query string, same as the header logout
    window.location.href = '/Account/Logout?returnUrl=/';
}