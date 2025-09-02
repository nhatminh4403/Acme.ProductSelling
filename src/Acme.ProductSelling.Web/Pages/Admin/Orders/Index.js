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
                                    return l('Order:OrderDeletionConfirmationMessage', data.record.orderNumber);
                                },
                                action: function (data) {
                                    orderService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('Order:SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            },

                            //{
                            //    text: l('ConfirmCodPayment'),
                            //    visible: function (data) {

                            //    },
                            //    confirmMessage: function (data) {
                            //        console.log(data.record.orderNumber);
                            //    }
                            //}
                            {
                                text: l('ConfirmCodPayment'),
                                // Nút chỉ hiện khi:
                                // 1. User có quyền.
                                // 2. Đơn hàng là COD.
                                // 3. Trạng thái thanh toán đang là PendingOnDelivery.
                                visible: function (data) {
                                    return abp.auth.isGranted('ProductSelling.Orders.ConfirmCodPayment') &&
                                        data.paymentMethod === 'COD' &&
                                        data.paymentStatus === 'PendingOnDelivery'; // Cần thêm PaymentStatus vào OrderDto
                                },
                                // Yêu cầu xác nhận trước khi thực hiện
                                confirmMessage: function (data) {
                                    return l('Order:AreYouSureYouWantToConfirmCodPaymentForOrder', data.orderNumber);
                                },
                                action: function (data) {
                                    orderService
                                        .markAsCodPaidAndCompleted(data.id)
                                        .then(function () {
                                            dataTable.ajax.reload(); // Tải lại bảng sau khi thành công
                                            abp.notify.success(l('Order:OrderUpdatedSuccessfully'));
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Order:OrderNumber'),
                    data: "orderNumber",
                    render: function (data, type, row) {
                        // Link đến trang chi tiết
                        return `<a href="/Admin/Orders/OrderDetail/${row.id}">${data}</a>`;
                    }
                },
                {
                    title: l('Order:OrderDate'),
                    data: "orderDate",
                    render: function (data) {
                        if (!data) return "";

                        return luxon.DateTime
                            .fromISO(data, { locale: abp.localization.currentCulture.name })
                            .toFormat("dd/MM/yyyy HH:mm:ss"); // đổi format tại đây
                    }
                },
                {
                    title: l('CustomerName'),
                    data: "customerName" // NEW: Thêm cột tên khách hàng
                },
                {
                    title: l('Order:TotalAmount'),
                    data: "totalAmount",
                    render: function (data) {
                        // Format tiền tệ VNĐ
                        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data);
                    }
                },
                {
                    title: l('Order:OrderStatus'),
                    data: "orderStatus",
                    // UPDATED: Sử dụng render function để hiển thị badge
                    render: function (data, type, row) {
                        // `row.statusText` là chuỗi đã được dịch từ server
                        console.log(data);
                        var statusText = row.statusText || l(data);
                        var badgeClass = getStatusBadgeClass(data);
                        return `<span class="badge ${badgeClass}">${statusText}</span>`;
                    }
                },
                {
                    title: l('Order:PaymentMethod'),
                    data: "paymentMethod"
                },
                {
                    title: l('Order:PaymentStatus'),
                    data: "paymentStatus", // Cần thêm thuộc tính này vào OrderDto
                    render: function (data, type, row) {
                        var status = data || 'Unpaid';
                        var badgeClass = getPaymentStatusBadgeClass(status);
                        //return `<span class="badge ${badgeClass}">/*${l('Enum:PaymentStatus.' + status)}*/</span>`;
                        return `<span class="badge ${badgeClass}">${status}</span>`;
                    }
                },
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
    function getPaymentStatusBadgeClass(status) {
        switch (status) {
            case 'Unpaid': return 'bg-secondary';
            case 'PendingOnDelivery': return 'bg-info text-dark';
            case 'Pending': return 'bg-warning text-dark';
            case 'Paid': return 'bg-success';
            case 'Failed': return 'bg-danger';
            case 'Refunded': return 'bg-dark';
            default: return 'bg-dark text-light';
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