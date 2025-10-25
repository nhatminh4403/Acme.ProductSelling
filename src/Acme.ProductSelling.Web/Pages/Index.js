document.addEventListener('DOMContentLoaded', () => {
    // ======= MEGAMENU & CAROUSEL SYNC =======
    const categoryMenu = document.querySelector('.category-sidebar-menu');
    const categoryItems = document.querySelectorAll('.category-sidebar-menu .category-menu-item.has-megamenu');
    const carouselEl = document.getElementById('mainBannerCarousel');
    const carouselContainer = carouselEl ? carouselEl.parentElement : null;

    if (window.innerWidth >= 992 && carouselEl && categoryMenu && carouselContainer) {
        const mainBannerCarousel = new bootstrap.Carousel(carouselEl);
        let overlay = document.getElementById('megamenu-overlay');

        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'megamenu-overlay';
            carouselContainer.style.position = 'relative';
            carouselContainer.appendChild(overlay);
        }

        Object.assign(overlay.style, {
            position: 'absolute',
            left: '0',
            width: '100%',
            background: 'white',
            zIndex: '2',
            display: 'none',
            overflowY: 'auto',
            marginLeft: '1rem'
        });

        let activeItem = null, hideTimeout;

        const updateOverlayPosition = () => {
            const menuRect = categoryMenu.getBoundingClientRect();
            const carouselContainerRect = carouselContainer.getBoundingClientRect();
            const topPosition = menuRect.top - carouselContainerRect.top;
            overlay.style.top = `${topPosition}px`;
            overlay.style.height = `${menuRect.height}px`;
        };

        const showOverlay = (item) => {
            clearTimeout(hideTimeout);
            if (activeItem !== item) {
                activeItem?.classList.remove('category-menu-item-active');
                item.classList.add('category-menu-item-active');
                activeItem = item;
                overlay.innerHTML = item.querySelector('.megamenu-data-source')?.innerHTML || '';
            }
            updateOverlayPosition();
            overlay.style.display = 'block';
            mainBannerCarousel.pause();
        };

        const hideOverlay = () => {
            hideTimeout = setTimeout(() => {
                overlay.style.display = 'none';
                overlay.innerHTML = '';
                activeItem?.classList.remove('category-menu-item-active');
                activeItem = null;
                mainBannerCarousel.cycle();
            }, 200);
        };

        categoryItems.forEach(item => {
            if (!item.querySelector('.megamenu-data-source')) return;
            item.addEventListener('mouseenter', () => showOverlay(item));
            item.addEventListener('mouseleave', hideOverlay);
        });

        overlay.addEventListener('mouseenter', () => clearTimeout(hideTimeout));
        overlay.addEventListener('mouseleave', hideOverlay);

        window.addEventListener('resize', () => {
            if (overlay.style.display === 'block') {
                updateOverlayPosition();
            }
        });
    }

    // ======= INFINITE RESPONSIVE PRODUCT CAROUSEL =======
    class ResponsiveCarousel {
        constructor(container) {
            this.container = container;
            this.track = container.querySelector('.product-carousel-track');
            this.prev = container.querySelector('.carousel-control-prev-custom');
            this.next = container.querySelector('.carousel-control-next-custom');
            if (!this.track || !this.prev || !this.next) return;

            this.isMoving = false;
            this.autoScrollInterval = null; // To hold the interval ID
            this.init();
        }

        get perView() {
            const w = window.innerWidth;
            return w >= 1200 ? 5 : w >= 992 ? 4 : w >= 768 ? 3 : w >= 576 ? 2 : 1;
        }

        init() {
            this.originalItems = Array.from(this.track.children);
            this.totalOriginalItems = this.originalItems.length;

            if (this.totalOriginalItems <= this.perView) {
                this.prev.style.display = 'none';
                this.next.style.display = 'none';
                return;
            }

            const itemsToCloneCount = this.perView;

            // Clone items for infinite effect
            const clonesStart = this.originalItems.slice(-itemsToCloneCount).map(item => item.cloneNode(true));
            this.track.prepend(...clonesStart);
            const clonesEnd = this.originalItems.slice(0, itemsToCloneCount).map(item => item.cloneNode(true));
            this.track.append(...clonesEnd);

            this.items = Array.from(this.track.children);
            this.index = itemsToCloneCount;

            this.bindEvents();
            this.updatePosition(false);
            this.startAutoScroll(); // Start auto-scrolling on initialization
        }

        startAutoScroll() {
            // Clear any existing interval to prevent duplicates
            if (this.autoScrollInterval) clearInterval(this.autoScrollInterval);

            // Set an interval to move the carousel. Duration is set to 7000ms (7 seconds).
            this.autoScrollInterval = setInterval(() => {
                this.move(1); // Move to the next item
            }, 7000); // You can change this value (5000 to 10000ms)
        }

        stopAutoScroll() {
            // Clear the interval to stop auto-scrolling
            clearInterval(this.autoScrollInterval);
        }

        bindEvents() {
            this.prev.addEventListener('click', () => this.move(-1));
            this.next.addEventListener('click', () => this.move(1));

            // When a transition finishes, check if we need to jump to the reset position
            this.track.addEventListener('transitionend', () => {
                this.isMoving = false; // Allow moving again
                if (this.index < this.perView) {
                    this.index += this.totalOriginalItems;
                    this.updatePosition(false);
                } else if (this.index >= this.perView + this.totalOriginalItems) {
                    this.index -= this.totalOriginalItems;
                    this.updatePosition(false);
                }
            });

            // Pause auto-scroll on mouse hover
            this.container.addEventListener('mouseenter', () => this.stopAutoScroll());
            this.container.addEventListener('mouseleave', () => this.startAutoScroll());

            window.addEventListener('resize', () => this.updatePosition(false));

            // Touch swipe support
            let startX = 0;
            this.track.addEventListener('touchstart', e => { startX = e.touches[0].clientX; }, { passive: true });
            this.track.addEventListener('touchend', e => {
                if (this.isMoving) return;
                const diff = startX - e.changedTouches[0].clientX;
                if (Math.abs(diff) > 50) this.move(diff > 0 ? 1 : -1);
            });
        }

        move(dir) {
            if (this.isMoving) return;
            this.isMoving = true;
            this.index += dir;
            this.updatePosition(true);
        }

        updatePosition(animated = true) {
            // The existing CSS transition provides the smooth, fluid animation
            this.track.style.transition = animated
                ? 'transform .5s cubic-bezier(0.4, 0, 0.2, 1)'
                : 'none';

            const itemWidth = this.originalItems[0]?.offsetWidth || 0;
            if (itemWidth === 0) return;
            const gap = 16; // Assuming a 1rem (16px) gap, adjust if necessary
            this.track.style.transform = `translateX(-${this.index * (itemWidth + gap)}px)`;
        }
    }

    // Initialize all infinite carousels on the page
    document.querySelectorAll('.product-carousel-container')
        .forEach(container => new ResponsiveCarousel(container));
});