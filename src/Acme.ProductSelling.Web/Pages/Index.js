document.addEventListener('DOMContentLoaded', () => {
    // ======= MEGAMENU =======
    const categoryItems = document.querySelectorAll('.category-sidebar-menu .category-menu-item.has-megamenu');
    const carousel = document.getElementById('mainBannerCarousel');
    if (window.innerWidth < 992 || !carousel) return;

    // Tạo overlay nếu chưa có
    let overlay = document.getElementById('megamenu-overlay');
    if (!overlay) {
        overlay = Object.assign(document.createElement('div'), {
            id: 'megamenu-overlay',
            style: `
                position:absolute;top:0;left:0;width:100%;background:white;
                z-index:2;display:none;overflow-y:auto;
            `
        });
        const parent = carousel.parentElement;
        parent.style.position = 'relative';
        parent.appendChild(overlay);
    }

    let activeItem = null, hideTimeout;

    const showOverlay = (item) => {
        clearTimeout(hideTimeout);
        activeItem?.classList.remove('category-menu-item-active');
        item.classList.add('category-menu-item-active');
        activeItem = item;

        overlay.style.height = '100%';
        overlay.innerHTML = item.querySelector('.megamenu-data-source')?.innerHTML || '';
        overlay.style.display = 'block';
    };

    const hideOverlay = () => {
        hideTimeout = setTimeout(() => {
            overlay.style.display = 'none';
            overlay.innerHTML = '';
            activeItem?.classList.remove('category-menu-item-active');
            activeItem = null;
        }, 200);
    };

    categoryItems.forEach(item => {
        if (!item.querySelector('.megamenu-data-source')) return;
        item.addEventListener('mouseenter', () => showOverlay(item));
        item.addEventListener('mouseleave', hideOverlay);
    });

    ['mouseenter', 'mouseleave'].forEach(evt =>
        overlay.addEventListener(evt, evt === 'mouseenter' ? () => clearTimeout(hideTimeout) : hideOverlay)
    );

    window.addEventListener('resize', () => {
        if (overlay.style.display === 'block') overlay.style.height = '100%';
    });

    // ======= RESPONSIVE CAROUSEL (Updated for Infinite Scroll) =======
    class ResponsiveCarousel {
        constructor(container) {
            this.container = container;
            this.track = container.querySelector('.product-carousel-track');
            this.prev = container.querySelector('.carousel-control-prev-custom');
            this.next = container.querySelector('.carousel-control-next-custom');
            this.isMoving = false; // Flag to prevent multiple clicks during transition
            this.init();
        }

        get perView() {
            const w = window.innerWidth;
            return w >= 1200 ? 5 : w >= 992 ? 4 : w >= 768 ? 3 : w >= 576 ? 2 : 1;
        }

        init() {
            this.originalItems = Array.from(this.track.children);
            this.totalOriginalItems = this.originalItems.length;

            // If there are not enough items to scroll, hide controls and stop.
            if (this.totalOriginalItems <= this.perView) {
                this.prev.style.display = 'none';
                this.next.style.display = 'none';
                return;
            }

            // --- Logic for infinite scrolling: Cloning items ---
            const itemsToCloneCount = this.perView;

            // Clone the last items and add them to the beginning.
            const clonesStart = this.originalItems.slice(-itemsToCloneCount).map(item => item.cloneNode(true));
            this.track.prepend(...clonesStart);

            // Clone the first items and add them to the end.
            const clonesEnd = this.originalItems.slice(0, itemsToCloneCount).map(item => item.cloneNode(true));
            this.track.append(...clonesEnd);

            this.items = Array.from(this.track.children);
            // Start the index at the beginning of the *original* items.
            this.index = itemsToCloneCount;

            this.bindEvents();
            // Set the initial position without any animation.
            this.updatePosition(false);
        }

        bindEvents() {
            this.prev.addEventListener('click', () => this.move(-1));
            this.next.addEventListener('click', () => this.move(1));

            // When the transition ends, check if we need to "jump" to the real items.
            this.track.addEventListener('transitionend', () => {
                this.isMoving = false; // Allow moving again

                // Check if we've moved to the prepended clones at the beginning
                if (this.index < this.perView) {
                    this.index += this.totalOriginalItems; // Jump to the corresponding items at the end
                    this.updatePosition(false); // Update without animation
                }
                // Check if we've moved to the appended clones at the end
                else if (this.index >= this.perView + this.totalOriginalItems) {
                    this.index -= this.totalOriginalItems; // Jump to the corresponding items at the start
                    this.updatePosition(false); // Update without animation
                }
            });

            window.addEventListener('resize', () => this.updatePosition(false));

            // Touch swipe
            let startX = 0;
            this.track.addEventListener('touchstart', e => {
                startX = e.touches[0].clientX;
            }, { passive: true });

            this.track.addEventListener('touchend', e => {
                if (this.isMoving) return;
                const diff = startX - e.changedTouches[0].clientX;
                if (Math.abs(diff) > 50) this.move(diff > 0 ? 1 : -1);
            });
        }

        move(dir) {
            if (this.isMoving) return; // Don't move if a transition is in progress
            this.isMoving = true;

            this.index += dir;
            this.updatePosition(true); // Move with animation
        }

        updatePosition(animated = true) {
            // This method handles updating the carousel's visual state.
            this.track.style.transition = animated
                ? 'transform .5s cubic-bezier(0.4, 0, 0.2, 1)'
                : 'none';

            const itemWidth = this.originalItems[0]?.offsetWidth || 0;
            if (itemWidth === 0) return; // Exit if items have no width yet

            const gap = 16; // Corresponds to `gap: 1rem;` in your CSS
            this.track.style.transform = `translateX(-${this.index * (itemWidth + gap)}px)`;
        }
    }

    document.querySelectorAll('.product-carousel-container')
        .forEach(container => new ResponsiveCarousel(container));
});