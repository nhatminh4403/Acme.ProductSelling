document.addEventListener('DOMContentLoaded', function () {

    //=========================================
    // 1. HEADER BEHAVIOR
    //=========================================
    const header = document.getElementById('mainHeader');
    if (header) {
        let lastScroll = 0;
        window.addEventListener('scroll', () => {
            const currentScroll = window.pageYOffset || document.documentElement.scrollTop;
            if (currentScroll > 10) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }
            lastScroll = currentScroll;
        }, { passive: true });
    }

    //=========================================
    // 2. DESKTOP DROPDOWNS (USER & LOGIN)
    //=========================================
    const userDropdown = document.getElementById('userDropdown');
    const loginDropdown = document.getElementById('loginDropdown');

    if (userDropdown) {
        userDropdown.addEventListener('click', (e) => {
            e.stopPropagation();
            userDropdown.classList.toggle('active');
            if (loginDropdown) loginDropdown.classList.remove('active');
        });
    }

    if (loginDropdown) {
        loginDropdown.addEventListener('click', (e) => {
            e.stopPropagation();
            loginDropdown.classList.toggle('active');
            if (userDropdown) userDropdown.classList.remove('active');
        });
    }

    // Close dropdowns when clicking outside
    document.addEventListener('click', () => {
        if (userDropdown) userDropdown.classList.remove('active');
        if (loginDropdown) loginDropdown.classList.remove('active');
    });


    //=========================================
    // 3. CATEGORY MEGAMENU
    //=========================================
    const megamenu = document.getElementById('categoryMegamenu');
    const overlay = document.getElementById('categoryMegamenuOverlay');
    const closeBtn = document.getElementById('megamenuCloseBtn');
    const desktopOpenBtn = document.getElementById('categoryMenuBtn');
    const mobileOpenBtn = document.getElementById('mobileCategoryBtn');
    const categoryLinks = document.querySelectorAll('.category-item-link');
    const contentPanels = document.querySelectorAll('.category-content-panel');

    function openMegamenu() {
        if (megamenu && overlay) {
            megamenu.classList.add('active');
            overlay.classList.add('active');
            document.body.style.overflow = 'hidden'; // Prevent page scroll
        }
    }

    function closeMegamenu() {
        if (megamenu && overlay) {
            megamenu.classList.remove('active');
            overlay.classList.remove('active');
            document.body.style.overflow = ''; // Restore page scroll
        }
    }

    // Event listeners for opening
    desktopOpenBtn?.addEventListener('click', openMegamenu);
    mobileOpenBtn?.addEventListener('click', openMegamenu);

    // Event listeners for closing
    closeBtn?.addEventListener('click', closeMegamenu);
    overlay?.addEventListener('click', closeMegamenu);
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeMegamenu();
            closeMobileAccountMenu(); // Also close mobile account menu
        }
    });

    // Logic for switching category content panels
    categoryLinks.forEach(link => {
        link.addEventListener('mouseenter', (e) => { // Using mouseenter for better desktop UX
            e.preventDefault();
            const categoryId = link.dataset.categoryId;

            categoryLinks.forEach(l => l.classList.remove('active'));
            link.classList.add('active');

            contentPanels.forEach(panel => {
                panel.style.display = panel.dataset.categoryId === categoryId ? 'block' : 'none';
            });
        });
    });

    // Activate the first category by default
    if (categoryLinks.length > 0) categoryLinks[0].classList.add('active');


    //=========================================
    // 4. MOBILE MENU & RESPONSIVE BEHAVIOR
    //=========================================
    const mobileMenuBtn = document.getElementById('mobileMenuBtn');
    const mobileAccountMenu = document.querySelector('.mobile-account-menu');
    const mobileAccountOverlay = document.querySelector('.mobile-account-overlay');
    const mobileAccountClose = document.querySelector('.mobile-account-close');
    const mobileSearchContainer = document.querySelector('.mobile-search');
    const searchToggleBtn = document.getElementById('searchToggleBtn');

    // Mobile Search Toggle
    if (searchToggleBtn && mobileSearchContainer) {
        searchToggleBtn.addEventListener('click', function () {
            mobileSearchContainer.classList.toggle('d-none');
            const icon = this.querySelector('i');
            const isHidden = mobileSearchContainer.classList.contains('d-none');
            icon.className = isHidden ? 'bi bi-search' : 'bi bi-x-lg';

            if (!isHidden) {
                setTimeout(() => mobileSearchContainer.querySelector('.search-input').focus(), 100);
            }
        });
    }

    // Mobile Account Menu
    function closeMobileAccountMenu() {
        if (mobileAccountMenu && mobileAccountOverlay) {
            mobileAccountMenu.classList.remove('active');
            mobileAccountOverlay.classList.remove('active');
            document.body.style.overflow = '';
        }
    }

    mobileMenuBtn?.addEventListener('click', (e) => {
        e.stopPropagation();
        if (mobileAccountMenu && mobileAccountOverlay) {
            mobileAccountMenu.classList.add('active');
            mobileAccountOverlay.classList.add('active');
            document.body.style.overflow = 'hidden';
        }
    });

    mobileAccountClose?.addEventListener('click', closeMobileAccountMenu);
    mobileAccountOverlay?.addEventListener('click', closeMobileAccountMenu);


    //=========================================
    // 5. SCROLL TOGGLE FOR CATEGORY BUTTON ON HOMEPAGE
    //=========================================
    const categoryBtnHomepage = document.getElementById('categoryMenuBtn');
    const searchContainer = document.querySelector('.search-container');
    const heroSection = document.querySelector('section.container.mt-2'); // More specific selector if needed

    if (categoryBtnHomepage && document.body.classList.contains('homepage') && heroSection) {
        function toggleCategoryButtonAndSearch() {
            const heroBottom = heroSection.offsetTop + heroSection.offsetHeight;
            const scrollPosition = window.pageYOffset || document.documentElement.scrollTop;
            const buffer = 100;

            if (scrollPosition > heroBottom - buffer) {
                categoryBtnHomepage.classList.add('show-on-scroll');
                if (searchContainer) searchContainer.classList.add('shrink');
            } else {
                categoryBtnHomepage.classList.remove('show-on-scroll');
                if (searchContainer) searchContainer.classList.remove('shrink');
            }
        }

        let isScrolling = false;
        window.addEventListener('scroll', function () {
            if (!isScrolling) {
                window.requestAnimationFrame(function () {
                    toggleCategoryButtonAndSearch();
                    isScrolling = false;
                });
                isScrolling = true;
            }
        }, { passive: true });

        toggleCategoryButtonAndSearch(); // Initial check on load
    }

    //=========================================
    // 6. MODAL SCROLL BEHAVIOR OVERRIDE
    //=========================================
    // This ensures the page can still scroll even when a Bootstrap modal is open.
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.attributeName === 'class' && document.body.classList.contains('modal-open')) {
                document.body.style.overflow = 'auto';
                document.body.style.paddingRight = '0';
            }
        });
    });

    observer.observe(document.body, {
        attributes: true,
        attributeFilter: ['class']
    });
});
