(function () {
    const l = abp.localization.getResource('ProductSelling');
    const orderService = acme.productSelling.orders.services.orderAppService;

    const dataTable = $('#InStoreOrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, 'desc']],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(orderService.getStoreOrders, function () {
                return {
                    orderType: 1, // InStore
                    sorting: 'CreationTime DESC'
                };
            }),
            columnDefs: [
                {
                    title: l('OrderNumber'),
                    data: 'orderNumber',
                    render: function (data, type, row) {
                        return `<a href="/${window.rolePrefix}/orders/detail/${row.id}">${data}</a>`;
                    }
                },
                {
                    title: l('OrderDate'),
                    data: 'orderDate',
                    render: function (data) {
                        return luxon.DateTime.fromISO(data).toFormat('dd/MM/yyyy HH:mm');
                    }
                },
                {
                    title: l('CustomerName'),
                    data: 'customerName'
                },
                {
                    title: l('TotalAmount'),
                    data: 'totalAmount',
                    render: function (data) {
                        return new Intl.NumberFormat('vi-VN', {
                            style: 'currency',
                            currency: 'VND'
                        }).format(data);
                    }
                },
                {
                    title: l('OrderStatus'),
                    data: 'orderStatusText',
                    render: function (data, type, row) {
                        const badgeClass = getStatusBadgeClass(row.orderStatus);
                        return `<span class="badge ${badgeClass}">${data}</span>`;
                    }
                },
                {
                    title: l('PaymentStatus'),
                    data: 'paymentStatusText',
                    render: function (data, type, row) {
                        const badgeClass = getPaymentStatusBadgeClass(row.paymentStatus);
                        return `<span class="badge ${badgeClass}">${data}</span>`;
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `<a href="/${window.rolePrefix}/orders/detail/${row.id}" class="btn btn-sm btn-primary">
                                    <i class="fa fa-eye"></i> ${l('View')}
                                </a>`;
                    }
                }
            ]
        })
    );

    function getStatusBadgeClass(status) {
        const statusMap = {
            0: 'bg-secondary',  // Pending
            1: 'bg-info',       // Placed
            2: 'bg-primary',    // Confirmed
            3: 'bg-warning',    // Processing
            4: 'bg-info',       // Shipped
            5: 'bg-success',    // Delivered
            6: 'bg-danger'      // Cancelled
        };
        return statusMap[status] || 'bg-secondary';
    }

    function getPaymentStatusBadgeClass(status) {
        const statusMap = {
            0: 'bg-secondary',  // Unpaid
            1: 'bg-warning',    // Pending
            2: 'bg-info',       // PendingOnDelivery
            3: 'bg-success',    // Paid
            4: 'bg-danger',     // Failed
            5: 'bg-danger',     // Cancelled
            6: 'bg-warning'     // Refunded
        };
        return statusMap[status] || 'bg-secondary';
    }
})();