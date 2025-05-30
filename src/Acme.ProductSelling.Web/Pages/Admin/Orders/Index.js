$(function () {
    var orderService = acme.productSelling.orders.order;
    var l = abp.localization.getResource('ProductSelling');

    var createModal = new abp.ModalManager(abp.appPath + 'Admin/Orders/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Admin/Orders/EditModal');

    // Cấu hình DataTables
    var dataTable = $('#OrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(orderService.getList),
            columnDefs: [
                {
                    title: l('Order:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Orders.Edit')
                        || abp.auth.isGranted('ProductSelling.Orders.Delete'),
                    items: [
                        {
                            text: l('Order:Edit'),
                            visible: abp.auth.isGranted('ProductSelling.Orders.Edit'),
                            action: function (data) {
                                editModal.open({ id: data.record.id });
                            }
                        },
                        {
                            text: l('Order:Delete'),
                            visible: abp.auth.isGranted('ProductSelling.Orders.Delete'),
                            confirmMessage: function (data) {
                                return l('OrderDeletionConfirmationMessage', data.record.name);
                            },
                            action: function (data) {
                                // Gọi API xóa
                                orderService.delete(data.record.id)
                                    .then(function () {
                                        abp.notify.info(l('SuccessfullyDeleted'));
                                        dataTable.ajax.reload();
                                    });
                            }
                        }
                    ]
                }
                ,
                {
                    title: l('Order:OrderNumber'),
                    data: "name"
                },
                {
                    title: l('Order:OrderDate'),
                    data: "description"
                },
                {
                    title: l('Order:TotalAmount'),
                    data: "totalAmount",
                }
                {
                    title: l('Order:OrderStatus'),
                    data: "orderStatus",
                }
            ]
        })
    );

    $('#NewOrderButton').click(function (e) {
        e.preventDefault();
        createModal.open(); // Mở Create Modal
    });

    createModal.onResult(function () {
        dataTable.ajax.reload(); // Load lại dữ liệu bảng
    });
    editModal.onResult(function () {
        dataTable.ajax.reload(); // Load lại dữ liệu bảng
    });

});