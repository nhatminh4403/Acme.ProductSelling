$(function () {
    var productService = acme.productSelling.products.product; 
    var l = abp.localization.getResource('ProductSelling');

    var createModal = new abp.ModalManager(abp.appPath + 'Products/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Products/EditModal');

    var dataTable = $('#ProductsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]], 
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(productService.getList), 
            columnDefs: [
                {
                    title: l('Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Products.Edit') ||
                        abp.auth.isGranted('ProductSelling.Products.Delete'), 
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
                                    return l('ProductDeletionConfirmationMessage', data.record.name); 
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
                    title: l('Name'), 
                    data: "productName",
                    render: function (data, type, row) {

                        var detailUrl = abp.appPath + 'products/' + row.id; 
                        return '<a href="' + detailUrl + '">' + data + '</a>'; 
                    },
                    width: "5%",
                },
                {
                    title: l('Description'), 
                    data: "description",
                     
                },
                {
                    title: l('Category'), 
                    data: "categoryName" ,
                },
                {
                    title: l('Price'), 
                    data: "price",
                    render: function (data) { 
                        return data.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                    }
                },
                {
                    title: l('Stock'), 
                    data: "stockCount",
                    width: "5%", 
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