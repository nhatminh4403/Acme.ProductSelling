$(function () { // Đảm bảo DOM đã load xong

    // Tham chiếu đến service API tự động tạo bởi ABP
    var categoryService = volo.abp.proxyClient.abp.application.category;
    // Hoặc nếu dùng CrudAppService thì có thể là:
    // var categoryService = myProjectName.categories.category; // Kiểm tra lại namespace chính xác trong file proxy JS

    var l = abp.localization.getResource('ProductSellingResource'); // Lấy resource localization cho JS

    var createModal = new abp.ModalManager(abp.appPath + 'Categories/CreateModal'); // Đường dẫn đến Create Modal Page
    var editModal = new abp.ModalManager(abp.appPath + 'Categories/EditModal');   // Đường dẫn đến Edit Modal Page

    // Cấu hình DataTables
    var dataTable = $('#CategoriesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({ // Sử dụng helper của ABP
            serverSide: true, // Xử lý phân trang, sắp xếp ở server
            paging: true,
            order: [[1, "asc"]], // Sắp xếp mặc định theo cột thứ 2 (Name) tăng dần
            searching: false, // Tắt searching mặc định của DataTables nếu dùng filter riêng
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(categoryService.getList), // Hàm gọi API lấy danh sách
            columnDefs: [
                {
                    title: l('Actions'), // Key localization "Actions"
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'), // Key localization "Edit"
                                // Chỉ hiển thị nút Edit nếu có quyền
                                visible: abp.auth.isGranted('ProductSellingPermissions.Categories.Edit'),
                                action: function (data) {
                                    // Mở Edit Modal, truyền ID của dòng hiện tại
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'), // Key localization "Delete"
                                // Chỉ hiển thị nút Delete nếu có quyền
                                visible: abp.auth.isGranted('ProductSellingPermissions.Categories.Delete'),
                                // Hiển thị confirm trước khi xóa
                                confirmMessage: function (data) {
                                    return l('CategoryDeletionConfirmationMessage', data.record.name); // Key localization ví dụ: "Are you sure you want to delete the category {0}?"
                                },
                                action: function (data) {
                                    // Gọi API xóa
                                    categoryService.delete(data.record.id)
                                        .then(function () {
                                            // Thông báo thành công và load lại bảng
                                            abp.notify.info(l('SuccessfullyDeleted')); // Key localization "SuccessfullyDeleted"
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Name'), // Key localization "Name"
                    data: "name"     // Tên thuộc tính trong CategoryDto trả về từ API
                },
                {
                    title: l('CreationTime'), // Key "CreationTime"
                    data: "creationTime",
                    dataFormat: "datetime" // Định dạng ngày giờ
                }
                // Thêm các cột khác nếu cần
            ]
        })
    );

    // Xử lý sự kiện click nút "Create Category"
    $('#NewCategoryButton').click(function (e) {
        e.preventDefault();
        createModal.open(); // Mở Create Modal
    });

    // Sự kiện sau khi Create Modal đóng và có thay đổi
    createModal.onResult(function () {
        dataTable.ajax.reload(); // Load lại dữ liệu bảng
    });

    // Sự kiện sau khi Edit Modal đóng và có thay đổi
    editModal.onResult(function () {
        dataTable.ajax.reload(); // Load lại dữ liệu bảng
    });

});