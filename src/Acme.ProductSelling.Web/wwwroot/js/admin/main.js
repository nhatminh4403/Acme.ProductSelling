const toggleBtn = document.getElementById('toggleBtn');
const sidebar = document.getElementById('sidebar');
const mainContent = document.getElementById('mainContent');
const overlay = document.getElementById('overlay');
const accountBtn = document.getElementById('accountBtn');
const accountDropdown = document.getElementById('accountDropdown');
const accountArrow = document.getElementById('accountArrow');


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