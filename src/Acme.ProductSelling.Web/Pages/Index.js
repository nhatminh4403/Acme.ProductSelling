document.addEventListener('DOMContentLoaded', function () {
    const categoryItems = document.querySelectorAll('.category-sidebar-menu .category-menu-item.has-megamenu');
    const megamenuDisplayArea = document.getElementById('megamenu-display-area');
    const carouselElement = document.getElementById('mainBannerCarousel');

    let activeCategoryItem = null; // Biến để lưu trữ LI đang active
    let hideTimeout; // Biến quản lý timeout

    // Chỉ chạy trên màn hình lớn
    if (window.innerWidth < 992 || !megamenuDisplayArea || !carouselElement) {
        return;
    }

    categoryItems.forEach(item => {
        const megamenuDataSource = item.querySelector('.megamenu-data-source');

        if (!megamenuDataSource) return;

        item.addEventListener('mouseenter', function () {
            clearTimeout(hideTimeout); // Xóa timeout ẩn cũ

            // ---- QUAN TRỌNG: Quản lý lớp active ----
            // 1. Xóa lớp active khỏi mục cũ (nếu có và khác mục hiện tại)
            if (activeCategoryItem && activeCategoryItem !== item) {
                activeCategoryItem.classList.remove('category-menu-item-active');
            }
            // 2. Thêm lớp active vào mục hiện tại và lưu tham chiếu
            item.classList.add('category-menu-item-active');
            activeCategoryItem = item;
            // ---- Kết thúc quản lý lớp active ----

            // Lấy nội dung HTML từ nguồn
            const contentHtml = megamenuDataSource.innerHTML;

            // Đặt nội dung vào khu vực hiển thị
            megamenuDisplayArea.innerHTML = contentHtml;

            // Ẩn Carousel và Hiển thị khu vực Megamenu
            carouselElement.style.display = 'none';
            megamenuDisplayArea.style.display = 'block';
        });

        item.addEventListener('mouseleave', function () {
            // Khi rời LI, không xóa lớp active ngay, chỉ lên lịch ẩn
            scheduleHide();
        });
    });

    // Giữ megamenu hiển thị và LI active nếu chuột di vào megamenu
    megamenuDisplayArea.addEventListener('mouseenter', function () {
        clearTimeout(hideTimeout); // Hủy lịch ẩn
        // Không cần làm gì với lớp active ở đây vì nó đã được set khi vào LI
    });

    // Bắt đầu timeout ẩn khi chuột rời khỏi megamenu
    megamenuDisplayArea.addEventListener('mouseleave', function () {
        scheduleHide();
    });

    function scheduleHide() {
        clearTimeout(hideTimeout); // Xóa timeout cũ trước khi đặt cái mới
        hideTimeout = setTimeout(() => {
            // Chỉ thực hiện ẩn và xóa lớp active sau khi hết timeout

            megamenuDisplayArea.style.display = 'none';
            megamenuDisplayArea.innerHTML = '';
            carouselElement.style.display = 'block';

            // ---- QUAN TRỌNG: Xóa lớp active ----
            // Xóa lớp active khỏi mục đã được lưu trữ
            if (activeCategoryItem) {
                activeCategoryItem.classList.remove('category-menu-item-active');
                activeCategoryItem = null; // Reset biến theo dõi
            }
            // ---- Kết thúc xóa lớp active ----

        }, 0); // Độ trễ 300ms (điều chỉnh nếu cần)
    }

});