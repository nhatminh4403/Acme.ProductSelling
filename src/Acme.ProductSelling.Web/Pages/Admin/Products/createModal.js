(function ($) {
    // Hàm này sẽ được gọi từ Razor page, với dữ liệu categorySpecTypes được truyền vào
    abp.modals.ProductCreateModal = function () { // Đặt tên theo namespace để dễ quản lý nếu có nhiều modal scripts

        function initialize(categorySpecTypesJson) {
            // Parse JSON data nhận được
            var categorySpecTypes = JSON.parse(categorySpecTypesJson);
            var categorySelect = $('#Product_CategoryId');
            console.log("Category Spec Types:", categorySpecTypes);
            console.log("Category Select Element:", categorySelect);
            var allSpecSections = $('.specs-section'); // Lấy tất cả các div specification

            function updateSpecificationView() {
                var selectedCategoryId = categorySelect.val();
                allSpecSections.hide(); // Ẩn tất cả các section trước

                if (selectedCategoryId && categorySpecTypes[selectedCategoryId]) {
                    var specType = categorySpecTypes[selectedCategoryId]; // Lấy SpecificationType (e.g., "Monitor", "CPU")
                    $('#spec-' + specType).show(); // Hiển thị section tương ứng
                }
            }

            // Gọi khi dropdown category thay đổi
            categorySelect.on('change', function () {
                var newValue = $(this).val(); // hoặc categorySelect.val()
                console.log("New selected Category ID (value): " + newValue);

                // Lấy text hiển thị của option vừa được chọn
                var newText = $(this).find('option:selected').text();
                console.log("New selected Category Name (text): " + newText);
                updateSpecificationView();
            });

            // Gọi lần đầu khi trang tải để thiết lập trạng thái ban đầu
            // (nếu có category được chọn sẵn hoặc để xử lý trường hợp không có category nào được chọn)
            updateSpecificationView();

            console.log("Product Create Modal Initialized with spec types:", categorySpecTypes);
        }

        // Trả về một đối tượng có hàm init để có thể gọi từ bên ngoài
        return {
            init: initialize
        };
    };
})(jQuery);

