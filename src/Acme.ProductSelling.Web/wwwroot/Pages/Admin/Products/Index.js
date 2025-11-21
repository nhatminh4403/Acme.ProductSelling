$(function () {
    // Wait for ABP to be fully initialized
    abp.event.on('abp.dynamicScriptsInitialized', function () {
        initializeProductsPage();
    });

    // If already initialized, run immediately
    if (window.abp && abp.appPath) {
        initializeProductsPage();
    }
});
function initializeProductsPage() {
    var productService = acme.productSelling.products.product;
    var l = abp.localization.getResource('ProductSelling');
    var prefix = window.location.pathname.split('/')[1];

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
                    title: l('Product:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Products.Edit') ||
                        abp.auth.isGranted('ProductSelling.Products.Delete'),
                    rowAction: {
                        items: [
                            {
                                text: l('Product:Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Products.Edit'),
                                action: function (data) {
                                    window.location.href = `/${prefix}/products/edit/` + data.record.id;
                                }
                            },
                            {
                                text: l('Product:Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Products.Delete'),
                                confirmMessage: function (data) {
                                    return l('Product:ProductDeletionConfirmationMessage', data.record.name);
                                },
                                action: function (data) {
                                    productService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('Product:SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    },
                    width: "10%",
                },
                {
                    title: l('Product:Name'),
                    data: "productName",
                    render: function (data, type, row) {
                        var detailUrl = abp.appPath + `${prefix}/products/detail/` + row.id;

                        var shortName = data.length > 20 ? data.substring(0, 20) + '...' : data;

                        return '<a href="' + detailUrl + '" class="text-decoration-none" title="' + data + '">' + shortName + '</a>';
                    },
                    width: "10%",
                    className: "text-end  "
                },
                {
                    title: l('Product:Category'),
                    data: "categoryId",
                    width: "5%",
                    className: "text-end"

                },
                {
                    title: l('Product:OriginalPrice'),
                    data: "originalPrice",
                    render: function (data) {
                        return data.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                    },
                    width: "8%",
                    className: "text-end"
                },
                {
                    title: l('Product:DiscountedPrice'),
                    data: "discountedPrice",
                    render: function (data) {
                        return data.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    },
                    className: "text-end",
                    width: "8%",
                },
                {
                    title: l('Product:DiscountPercent'),
                    data: "discountPercent",
                    render: function (data) {
                        return data + '%';
                    },
                    className: "text-end",
                    width: "8%"
                },

                {
                    title: l('Product:Stock'),
                    data: "stockCount",
                    className: "text-end",
                    width: "8%",
                },
            ]
        })
    );

    $('#NewProductButton').on('click', function () {
        window.location.href = `/${prefix}/products/create`;
    });
};
