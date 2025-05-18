document.addEventListener('DOMContentLoaded', function () {
    const categoryItems = document.querySelectorAll('.category-sidebar-menu .category-menu-item.has-megamenu');
    const megamenuDisplayArea = document.getElementById('megamenu-display-area');
    const carouselElement = document.getElementById('mainBannerCarousel');

    let activeCategoryItem = null;
    let hideTimeout;
    if (window.innerWidth < 992 || !megamenuDisplayArea || !carouselElement) {
        return;
    }

    categoryItems.forEach(item => {
        const megamenuDataSource = item.querySelector('.megamenu-data-source');
        const megamenuContentWrapper = item.querySelector('.megamenu-content-wrapper');
        if (!megamenuDataSource) return;

        item.addEventListener('mouseenter', function () {
            clearTimeout(hideTimeout);

            if (activeCategoryItem && activeCategoryItem !== item) {
                activeCategoryItem.classList.remove('category-menu-item-active');
            }

            item.classList.add('category-menu-item-active');
            activeCategoryItem = item;

            const contentHtml = megamenuDataSource.innerHTML;
            megamenuDisplayArea.innerHTML = contentHtml;

            const carouselHeight = carouselElement.offsetHeight;
            megamenuDisplayArea.style.height = `${carouselHeight}px`;
            //megamenuContentWrapper.style.height = `${carouselHeight}px`;
            carouselElement.style.display = 'none';
            megamenuDisplayArea.style.display = 'block';

            
        });

        item.addEventListener('mouseleave', function () {
            scheduleHide();
        });
    });
    megamenuDisplayArea.addEventListener('mouseenter', function () {
        clearTimeout(hideTimeout);
    });
    megamenuDisplayArea.addEventListener('mouseleave', function () {
        scheduleHide();
    });

    function scheduleHide() {
        clearTimeout(hideTimeout);
        hideTimeout = setTimeout(() => {

            megamenuDisplayArea.style.display = 'none';
            megamenuDisplayArea.innerHTML = '';
            megamenuDisplayArea.style.height = '';
            carouselElement.style.display = 'block';

            if (activeCategoryItem) {
                activeCategoryItem.classList.remove('category-menu-item-active');
                activeCategoryItem = null; 
            }
           

        }, 0);
    }

});