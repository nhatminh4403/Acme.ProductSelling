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
            order: [[2, "desc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(orderService.getList),
            rowId: 'id',
            columnDefs: [
                {
                    title: l('Order:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Orders.Edit')
                        || abp.auth.isGranted('ProductSelling.Orders.Delete'),
                    rowAction: { // UPDATED: Sử dụng cấu trúc rowAction của ABP cho gọn gàng hơn
                        items: [
                            {
                                text: l('Edit'), // Chữ 'Edit' đơn giản
                                visible: abp.auth.isGranted('ProductSelling.Orders.Edit'),
                                action: function (data) {
                                    // Thay vì modal, bạn có thể muốn điều hướng đến trang chi tiết
                                    // hoặc mở modal để thay đổi trạng thái.
                                    // Ví dụ điều hướng:
                                    window.location.href = '/Admin/Orders/Detail/' + data.record.id;
                                    // Hoặc mở modal sửa
                                    // editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'), // Chữ 'Delete' đơn giản
                                visible: abp.auth.isGranted('ProductSelling.Orders.Delete'),
                                confirmMessage: function (data) {
                                    return l('OrderDeletionConfirmationMessage', data.record.orderNumber); // UPDATED: Dùng orderNumber
                                },
                                action: function (data) {
                                    orderService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                }
                ,
                {
                    title: l('Order:OrderNumber'),
                    data: "orderNumber"
                },
                {
                    title: l('Order:OrderDate'),
                    render: function (data) {
                        return luxon
                            .DateTime
                            .fromISO(data, { locale: abp.localization.currentCulture.name })
                            .toLocaleString(luxon.DateTime.DATETIME_SHORT);
                    }
                },
                {
                    title: l('CustomerName'),
                    data: "customerName" // NEW: Thêm cột tên khách hàng
                },
                {
                    title: l('Order:TotalAmount'),
                    data: "totalAmount",
                },
                {
                    title: l('Order:OrderStatus'),
                    data: "orderStatus",
                    // UPDATED: Sử dụng render function để hiển thị badge
                    render: function (data, type, row) {
                        // `row.statusText` là chuỗi đã được dịch từ server
                        var statusText = row.statusText || l(data);
                        var badgeClass = getStatusBadgeClass(data);
                        return `<span class="badge ${badgeClass}">${statusText}</span>`;
                    }
                }
            ]
        })
    );
    function getStatusBadgeClass(statusString) {
        switch (statusString) {
            case 'Placed': return 'bg-info';
            case 'Pending': return 'bg-light text-dark';
            case 'Confirmed': return 'bg-primary';
            case 'Processing': return 'bg-warning';
            case 'Shipped': return 'bg-success';
            case 'Delivered': return 'bg-dark';
            case 'Cancelled': return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

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