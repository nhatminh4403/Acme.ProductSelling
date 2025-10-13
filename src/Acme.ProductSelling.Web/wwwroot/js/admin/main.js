const toggleBtn = document.getElementById('toggleBtn');
const sidebar = document.getElementById('sidebar');
const mainContent = document.getElementById('mainContent');
const overlay = document.getElementById('overlay');

toggleBtn.addEventListener('click', function () {
    if (window.innerWidth <= 768) {
        sidebar.classList.toggle('mobile-open');
        overlay.classList.toggle('active');
    } else {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');
    }
});

overlay.addEventListener('click', function () {
    sidebar.classList.remove('mobile-open');
    overlay.classList.remove('active');
});

function toggleSubmenu(element) {
    const submenu = element.nextElementSibling; // The <ul> is the next sibling of the clicked <div>
    const arrow = element.querySelector('.arrow-icon');

    if (submenu && submenu.classList.contains('submenu')) {
        submenu.classList.toggle('open');
    }

    if (arrow) {
        arrow.classList.toggle('rotated');
    }
}

// Handle window resize
window.addEventListener('resize', function () {
    if (window.innerWidth > 768) {
        sidebar.classList.remove('mobile-open');
        overlay.classList.remove('active');
    }
});