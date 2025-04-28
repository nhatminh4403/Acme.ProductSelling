$(function () {
    //console.log("Available ABP objects:", abp);
    var productService = acme.productSelling.products.product; // Proxy client cho ProductAppService
    var l = abp.localization.getResource('ProductSelling');

    var createModal = new abp.ModalManager(abp.appPath + 'Products/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Products/EditModal');

    var dataTable = $('#ProductsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]], // Sắp xếp theo tên sản phẩm
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(productService.getList), // Gọi API GetList
            columnDefs: [
                {
                    title: l('Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Products.Edit') || abp.auth.isGranted('ProductSelling.Products.Delete'), // Hiện cột này nếu có quyền Edit hoặc Delete
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Products.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Products.Delete'),
                                confirmMessage: function (data) {
                                    return l('ProductDeletionConfirmationMessage', data.record.name); // Key: "Are you sure..."
                                },
                                action: function (data) {
                                    productService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Name'), // Key localization "Name"
                    data: "productName",
                    render: function (data, type, row) {
                        // data: Giá trị của cột 'productName'
                        // type: Kiểu render (thường là 'display')
                        // row: Toàn bộ dữ liệu của dòng hiện tại (chứa cả 'id')

                        // Tạo thẻ <a> với href đến trang chi tiết
                        // Sử dụng row.id để lấy ID của sản phẩm
                        var detailUrl = abp.appPath + 'products/' + row.id; // Tạo URL đúng
                        return '<a href="' + detailUrl + '">' + data + '</a>'; // Trả về HTML của link
                    }
                },
                {
                    title: l('Description'), // Key localization "Name"
                    data: "description"
                },
                {
                    title: l('Category'), // Key localization "Category" (đã thêm ở bước trước)
                    data: "categoryName" // Lấy từ ProductDto
                },
                {
                    title: l('Price'), // Key localization "Price"
                    data: "price",
                    render: function (data) { // Định dạng giá tiền (ví dụ)
                        return data.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                        // Hoặc định dạng đơn giản hơn nếu cần
                        // return data;
                    }
                },
                {
                    title: l('Stock'), // Key localization "Name"
                    data: "stockCount"
                },
            ]
        })
    );

    $('#NewProductButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});