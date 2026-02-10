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
    function switchToCategory(link) {
        const categoryId = link.dataset.categoryId;

        if (!categoryId) return;

        // Update active state on sidebar links
        categoryLinks.forEach(l => l.parentElement.classList.remove('active'));
        link.parentElement.classList.add('active');

        // Switch content panels
        contentPanels.forEach(panel => {
            if (panel.dataset.categoryId === categoryId) {
                panel.style.display = 'block';
                // Add fade-in animation
                panel.style.opacity = '0';
                setTimeout(() => {
                    panel.style.opacity = '1';
                }, 10);
            } else {
                panel.style.display = 'none';
            }
        });
    }

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
        link.addEventListener('click', (e) => {
            e.preventDefault();
            switchToCategory(link);
        });
    });

    // Activate the first category by default
    if (categoryLinks.length > 0) categoryLinks[0].classList.add('active');
    //=========================================
    // SMOOTH SCROLL FOR CATEGORY GROUPS
    //=========================================
    const categoryGroupHeaders = document.querySelectorAll('.category-group-header');

    categoryGroupHeaders.forEach(header => {
        header.addEventListener('click', function () {
            // Optional: Collapse/expand groups
            const nextElement = this.nextElementSibling;
            if (nextElement && nextElement.classList.contains('category-item')) {
                // Toggle group visibility (if implementing collapsible groups)
                // Implementation depends on your requirements
            }
        });
    });

    //=========================================
    // KEYBOARD NAVIGATION SUPPORT
    //=========================================
    let currentCategoryIndex = 0;
    const navigableCategories = Array.from(categoryLinks);

    document.addEventListener('keydown', function (e) {
        const megamenu = document.getElementById('categoryMegamenu');
        if (!megamenu || !megamenu.classList.contains('active')) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                currentCategoryIndex = Math.min(currentCategoryIndex + 1, navigableCategories.length - 1);
                navigableCategories[currentCategoryIndex].focus();
                switchToCategory(navigableCategories[currentCategoryIndex]);
                break;

            case 'ArrowUp':
                e.preventDefault();
                currentCategoryIndex = Math.max(currentCategoryIndex - 1, 0);
                navigableCategories[currentCategoryIndex].focus();
                switchToCategory(navigableCategories[currentCategoryIndex]);
                break;

            case 'Enter':
                e.preventDefault();
                navigableCategories[currentCategoryIndex].click();
                break;
        }
    });

    //=========================================
    // ANALYTICS TRACKING (Optional)
    //=========================================
    function trackCategoryView(categoryId, categoryName) {
        // Implement your analytics tracking here
        console.log(`Category viewed: ${categoryName} (${categoryId})`);

        // Example: Google Analytics
        // if (typeof gtag !== 'undefined') {
        //     gtag('event', 'category_view', {
        //         'category_id': categoryId,
        //         'category_name': categoryName
        //     });
        // }
    }

    //=========================================
    // PERFORMANCE OPTIMIZATION
    //=========================================
    // Lazy load category content panels
    const lazyLoadCategoryContent = () => {
        const observerOptions = {
            root: document.querySelector('.megamenu-content'),
            rootMargin: '50px',
            threshold: 0.1
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const panel = entry.target;
                    // Load any deferred content here
                    panel.classList.add('loaded');
                }
            });
        }, observerOptions);

        contentPanels.forEach(panel => {
            observer.observe(panel);
        });
    };

    // Initialize lazy loading if needed
     lazyLoadCategoryContent();

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
