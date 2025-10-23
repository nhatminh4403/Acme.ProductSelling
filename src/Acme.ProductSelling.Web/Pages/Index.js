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

    // ======= RESPONSIVE CAROUSEL =======
    class ResponsiveCarousel {
        constructor(container) {
            this.container = container;
            this.track = container.querySelector('.product-carousel-track');
            this.prev = container.querySelector('.carousel-control-prev-custom');
            this.next = container.querySelector('.carousel-control-next-custom');
            this.items = Array.from(this.track.children);
            this.index = 0;
            this.init();
        }

        get perView() {
            const w = window.innerWidth;
            return w >= 1200 ? 5 : w >= 992 ? 4 : w >= 768 ? 3 : w >= 576 ? 2 : 1;
        }

        init() {
            this.update();
            this.prev.addEventListener('click', () => this.move(-1));
            this.next.addEventListener('click', () => this.move(1));
            window.addEventListener('resize', () => this.update());

            // Touch swipe
            let startX = 0;
            this.track.addEventListener('touchstart', e => startX = e.touches[0].clientX);
            this.track.addEventListener('touchend', e => {
                const diff = startX - e.changedTouches[0].clientX;
                if (Math.abs(diff) > 50) this.move(diff > 0 ? 1 : -1);
            });
        }

        move(dir) {
            const max = Math.max(0, this.items.length - this.perView);
            this.index = Math.min(Math.max(this.index + dir, 0), max);
            this.update();
        }

        update() {
            const itemWidth = this.items[0].offsetWidth;
            const gap = 16;
            this.track.style.transform = `translateX(-${this.index * (itemWidth + gap)}px)`;
            const max = Math.max(0, this.items.length - this.perView);
            this.prev.disabled = this.index === 0;
            this.next.disabled = this.index >= max;
        }
    }

    document.querySelectorAll('.product-carousel-container')
        .forEach(container => new ResponsiveCarousel(container));
});
