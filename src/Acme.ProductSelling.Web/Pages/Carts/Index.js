// Đặt trong $(function () { ... }); để đảm bảo DOM sẵn sàng

// Khai báo service proxy (thay bằng đường dẫn đúng của bạn)
// Kiểm tra trong Console (F12) của trình duyệt nếu không chắc
var cartService = acme.productSelling.carts.cart; // Ví dụ
var localizationResource = abp.localization.getResource('ProductSelling'); // Tên resource của bạn

// --- Xử lý nút Add To Cart ---
// Sử dụng event delegation cho các nút có thể được thêm động
$('body').on('click', '.add-to-cart-button', function (e) {
    e.preventDefault();
    var $button = $(this); // Lưu trữ đối tượng jQuery của nút

    // 1. KIỂM TRA ĐĂNG NHẬP
    if (!abp.currentUser.isAuthenticated) {
        // Lấy URL hiện tại để quay lại sau khi đăng nhập
        var returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
        // Thay '/Account/Login' bằng URL trang đăng nhập thực tế của bạn
        window.location.href = abp.appPath + 'Account/Login?ReturnUrl=' + returnUrl;
        return; // Dừng thực thi nếu chưa đăng nhập
    }

    // 2. Lấy thông tin sản phẩm
    var productId = $button.data('product-id');
    // Ví dụ lấy số lượng từ input có id="quantity-for-PRODUCT_ID"
    var quantityInputId = '#quantity-for-' + productId;
    var quantity = parseInt($(quantityInputId).val() || '1'); // Mặc định là 1

    if (!productId || quantity <= 0) {
        abp.notify.warn(localizationResource('InvalidProductOrQuantity'));
        return;
    }

    // 3. Xử lý trạng thái nút (Loading)
    $button.prop('disabled', true).addClass('disabled');
    var originalButtonHtml = $button.html(); // Lưu lại nội dung gốc của nút
    $button.html('<i class="fas fa-spinner fa-spin me-1"></i>' + localizationResource('AddingToCart')); // Hiển thị loading

    // 4. Gọi API Service
    cartService.addItem({ // Tạo đối tượng AddToCartInput
        productId: productId,
        quantity: quantity
    })
        .then(function () {
            abp.notify.success(localizationResource('ItemAddedToCart'));
            // 5. Cập nhật số lượng trên widget giỏ hàng
            updateCartWidgetCount(); // Gọi hàm cập nhật widget
        })
        .catch(function (error) {
            // Hiển thị lỗi trả về từ server hoặc lỗi chung
            abp.notify.error(error.message || localizationResource('CouldNotAddItemToCart'));
            console.error("Add to cart error:", error); // Ghi log lỗi chi tiết
        })
        .finally(function () {
            // 6. Khôi phục trạng thái nút
            $button.prop('disabled', false).removeClass('disabled').html(originalButtonHtml);
        });
});

// --- Hàm cập nhật số lượng trên Widget giỏ hàng ---
function updateCartWidgetCount() {
    var $widget = $('#shopping-cart-widget'); // ID của widget trong Layout
    var $countElement = $widget.find('.cart-item-count'); // Selector của phần tử hiển thị số lượng

    if ($widget.length === 0 || $countElement.length === 0) {
        return; // Không tìm thấy widget hoặc phần tử đếm
    }

    // Chỉ gọi API nếu đã đăng nhập
    if (abp.currentUser.isAuthenticated) {
        cartService.getItemCount()
            .then(function (count) {
                $countElement.text(count); // Cập nhật số
                // Hiển thị/ẩn badge dựa trên số lượng
                if (count > 0) {
                    $countElement.removeClass('d-none'); // Hoặc logic tương tự để hiển thị badge
                } else {
                    $countElement.addClass('d-none'); // Ẩn badge nếu không có item
                }
            })
            .catch(function (error) {
                console.error("Error updating cart widget count:", error);
                // Có thể ẩn badge nếu lỗi
                $countElement.addClass('d-none');
            });
    } else {
        // Nếu chưa đăng nhập, hiển thị 0 và ẩn badge
        $countElement.text('0').addClass('d-none');
    }
}

// Gọi hàm cập nhật widget lần đầu khi trang tải
updateCartWidgetCount();

}); // Kết thúc $(function () { ... });