document.addEventListener('DOMContentLoaded', function () {
    const megamenu = document.getElementById('categoryMegamenu');
    const overlay = document.getElementById('categoryMegamenuOverlay');
    const closeBtn = document.getElementById('megamenuCloseBtn');
    const desktopOpenBtn = document.getElementById('categoryMenuBtn');
    const mobileOpenBtn = document.getElementById('mobileCategoryBtn');
    const categoryLinks = document.querySelectorAll('.category-item-link');
    const contentPanels = document.querySelectorAll('.category-content-panel');
    const groupContentPanels = document.querySelectorAll('.group-content-panel');
    const megamenuContent = document.getElementById('megamenuContent');

    sessionStorage.setItem('appLoaded', '1');



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


    function switchContent(link) {
        const isGroup = link.dataset.isGroup === 'true';
        const hasMegamenu = link.dataset.hasMegamenu === 'true';

        // Show megamenu content area


        // Update active state on sidebar links
        categoryLinks.forEach(l => l.parentElement.classList.remove('active'));
        link.parentElement.classList.add('active');

        if (megamenuContent) {
            if (hasMegamenu) {
                megamenuContent.style.display = 'flex';
                // Small timeout to ensure display:flex is painted before opacity starts
                setTimeout(() => {
                    megamenuContent.classList.add('show');
                }, 10);
            } else {
                megamenuContent.classList.remove('show');
                megamenuContent.style.display = 'none';
                return; // Exit early, nothing else to render
            }
        }
        if (isGroup) {
            // Show group content panel
            const groupId = link.dataset.groupId;

            contentPanels.forEach(panel => {
                panel.style.display = 'none';
                panel.classList.remove('active');
            });

            // Show the selected group panel
            groupContentPanels.forEach(panel => {
                if (panel.dataset.groupId === groupId) {
                    panel.style.display = 'block';
                    panel.classList.add('active');
                    panel.style.opacity = '1';
                } else {
                    panel.style.display = 'none';
                    panel.classList.remove('active');
                }
            });
        } else {
            const categoryId = link.dataset.categoryId;
            groupContentPanels.forEach(panel => {
                panel.style.display = 'none';
                panel.classList.remove('active');
            });

            contentPanels.forEach(panel => {
                if (panel.dataset.categoryId === categoryId) {
                    panel.style.display = 'block';
                    panel.classList.add('active');
                    panel.style.opacity = '1';
                } else {
                    panel.style.display = 'none';
                    panel.classList.remove('active');
                }
            });
        }
    }

    // Logic for switching category content panels
    categoryLinks.forEach(link => {
        // Use mouseenter for desktop hover experience
        link.addEventListener('mouseenter', (e) => {
            switchContent(link);
        });

        // Handle click
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const hasMegamenu = link.dataset.hasMegamenu === 'true';

            if (hasMegamenu) {
                // Prevent navigation and show the menu
                e.preventDefault();
                switchContent(link);
            } else {
                // Do nothing! Let the browser follow the actual 'href' link 
                // because there is no megamenu to show.
                closeMegamenu();
            }
        });
    });
    // Activate the first category by default
    if (categoryLinks.length > 0) {
        categoryLinks[0].parentElement.classList.add('active');
        // Ensure the first panel is shown
        if (contentPanels.length > 0) {
            contentPanels[0].style.display = 'block';
            contentPanels[0].classList.add('active');
        }
    }
    // SMOOTH SCROLL FOR CATEGORY GROUPS
    const allPanels = [...contentPanels, ...groupContentPanels];
    allPanels.forEach(panel => {
        panel.style.transition = 'opacity 0.1s ease-in-out';
    });
    const megamenuSidebar = document.querySelector('.megamenu-sidebar');

    if (megamenu) {
        // Hide content when mouse leaves entire megamenu
        megamenu.addEventListener('click', (e) => {
            if (e.target === megamenu) {
                closeMegamenu();
            }
        });
    }

    // KEYBOARD NAVIGATION SUPPORT
    let currentIndex = 0;
    const navigableLinks = Array.from(categoryLinks);

    document.addEventListener('keydown', function (e) {
        const megamenu = document.getElementById('categoryMegamenu');
        if (!megamenu || !megamenu.classList.contains('active')) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                currentIndex = Math.min(currentIndex + 1, navigableLinks.length - 1);
                navigableLinks[currentIndex].focus();
                switchContent(navigableLinks[currentIndex]);
                break;

            case 'ArrowUp':
                e.preventDefault();
                currentIndex = Math.max(currentIndex - 1, 0);
                navigableLinks[currentIndex].focus();
                switchContent(navigableLinks[currentIndex]);
                break;

            case 'Enter':
                e.preventDefault();
                if (navigableLinks[currentIndex].dataset.isGroup === 'true') {
                    // Already showing group content, do nothing
                } else {
                    // Navigate to category page
                    const categoryLink = navigableLinks[currentIndex].getAttribute('href');
                    if (categoryLink && categoryLink !== '#') {
                        window.location.href = categoryLink;
                    }
                }
                break;
        }
    });


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

    // Initialize lazy loading 
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

    if (categoryBtnHomepage) {
        // Remove any inline style left over from HTML (e.g. opacity:0; pointer-events:none)
        // CSS now owns the hidden state
        categoryBtnHomepage.removeAttribute('style');

        if (document.documentElement.classList.contains('homepage') && heroSection) {
            // HOMEPAGE: show button only after scrolling past the hero
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
        } else {
            // ✅ NON-HOMEPAGE: always show the button immediately
            categoryBtnHomepage.classList.add('show-on-scroll');
            if (searchContainer) searchContainer.classList.add('shrink');
        }
    }

    //=========================================
    // 6. MODAL SCROLL BEHAVIOR OVERRIDE
    //=========================================
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
