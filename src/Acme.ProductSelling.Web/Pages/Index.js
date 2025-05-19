document.addEventListener('DOMContentLoaded', function () {
    const categoryItems = document.querySelectorAll('.category-sidebar-menu .category-menu-item.has-megamenu');
    const carouselElement = document.getElementById('mainBannerCarousel');
    let activeCategoryItem = null;
    let hideTimeout;

    // Exit early if mobile or elements don't exist
    if (window.innerWidth < 992 || !carouselElement) {
        return;
    }

    // Create megamenu overlay container if it doesn't exist
    let megamenuOverlay = document.getElementById('megamenu-overlay');
    if (!megamenuOverlay) {
        megamenuOverlay = document.createElement('div');
        megamenuOverlay.id = 'megamenu-overlay';
        megamenuOverlay.style.position = 'absolute';
        megamenuOverlay.style.top = '0';
        megamenuOverlay.style.left = '0';
        megamenuOverlay.style.width = '100%';
        megamenuOverlay.style.background = 'white';
        megamenuOverlay.style.zIndex = '2';
        megamenuOverlay.style.display = 'none';
        megamenuOverlay.style.overflowY = 'auto';
        
        // Make carousel container position relative
        const carouselContainer = carouselElement.parentElement;
        carouselContainer.style.position = 'relative';
        carouselContainer.appendChild(megamenuOverlay);
    }

    categoryItems.forEach(item => {
        const megamenuDataSource = item.querySelector('.megamenu-data-source');
        if (!megamenuDataSource) return;
        
        item.addEventListener('mouseenter', function () {
            clearTimeout(hideTimeout);
            
            if (activeCategoryItem && activeCategoryItem !== item) {
                activeCategoryItem.classList.remove('category-menu-item-active');
            }
            
            item.classList.add('category-menu-item-active');
            activeCategoryItem = item;
            
            // Set height to match carousel
            megamenuOverlay.style.height = '100%';
            
            const contentHtml = megamenuDataSource.innerHTML;
            megamenuOverlay.innerHTML = contentHtml;
            megamenuOverlay.style.display = 'block';
        });
        
        item.addEventListener('mouseleave', function () {
            scheduleHide();
        });
    });
    
    megamenuOverlay.addEventListener('mouseenter', function () {
        clearTimeout(hideTimeout);
    });
    
    megamenuOverlay.addEventListener('mouseleave', function () {
        scheduleHide();
    });
    
    function scheduleHide() {
        clearTimeout(hideTimeout);
        hideTimeout = setTimeout(() => {
            megamenuOverlay.style.display = 'none';
            megamenuOverlay.innerHTML = '';
            
            if (activeCategoryItem) {
                activeCategoryItem.classList.remove('category-menu-item-active');
                activeCategoryItem = null; 
            }
        }, 200);
    }
    
    // Update height on window resize
    window.addEventListener('resize', function() {
        if (megamenuOverlay.style.display === 'block') {
            megamenuOverlay.style.height = '100%';
        }
    });
});