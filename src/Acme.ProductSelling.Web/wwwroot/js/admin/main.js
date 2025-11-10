document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('toggleBtn');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    const overlay = document.getElementById('overlay');
    const accountBtn = document.getElementById('accountBtn');
    const accountDropdown = document.getElementById('accountDropdown');
    const accountArrow = document.getElementById('accountArrow');
    const loggoutBtn = document.getElementById('logout-btn');


    // Apply saved state to elements
    const savedState = localStorage.getItem('sidebarCollapsed');
    if (savedState === 'true') {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('expanded');
    }

    // Remove the preload style after a brief moment to enable transitions
    setTimeout(() => {
        const preloadStyle = document.getElementById('sidebar-preload-style');
        if (preloadStyle) {
            preloadStyle.remove();
        }
    }, 100);

    // Toggle button functionality
    toggleBtn.addEventListener('click', function () {
        if (window.innerWidth <= 768) {
            // Mobile behavior - don't save state
            sidebar.classList.toggle('mobile-open');
            overlay.classList.toggle('active');
        } else {
            // Desktop behavior - save state
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('expanded');

            // Save the current state to localStorage
            const isCollapsed = sidebar.classList.contains('collapsed');
            localStorage.setItem('sidebarCollapsed', isCollapsed);
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

    // Handle window resize
    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('mobile-open');
            overlay.classList.remove('active');

            // Restore saved state when resizing back to desktop
            const savedState = localStorage.getItem('sidebarCollapsed');
            if (savedState === 'true') {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
            } else {
                sidebar.classList.remove('collapsed');
                mainContent.classList.remove('expanded');
            }
        }
    });
    
});

// Make toggleSubmenu available globally
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
    // Add your logout logic here
    if (confirm('Are you sure you want to logout?')) {
        console.log('Logging out...');
        window.location.href = '/Account/Logout?returnUrl=/';
    }
}
// In your layout or shared JS file
