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
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Orders.Edit'),
                                action: function (data) {
                                    window.location.href = '/Admin/Orders/Detail/' + data.record.id;
                                }
                            },
                            {
                                text: l('Delete'),
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
                            {
                                text: l('ShipOrder'),
                                visible: function (data) {
                                    // Check using orderStatusText string
                                    return abp.auth.isGranted('ProductSelling.Orders.Edit') &&
                                        (data.orderStatusText === 'Confirmed' || data.orderStatusText === 'Processing');
                                },
                                confirmMessage: function (data) {
                                    return l('Order:AreYouSureYouWantToShipOrder', data.orderNumber);
                                },
                                action: function (data) {
                                    orderService
                                        .shipOrder(data.record.id)
                                        .then(function () {
                                            dataTable.ajax.reload();
                                            abp.notify.success(l('Order:OrderShippedSuccessfully'));
                                        })
                                        .catch(function (error) {
                                            abp.notify.error(error.message || l('Order:ShipOrderFailed'));
                                        });
                                }
                            },
                            {
                                text: l('DeliverOrder'),
                                visible: function (data) {
                                    return abp.auth.isGranted('ProductSelling.Orders.Edit') &&
                                        data.orderStatusText === 'Shipped';
                                },
                                confirmMessage: function (data) {
                                    return l('Order:AreYouSureYouWantToDeliverOrder', data.orderNumber);
                                },
                                action: function (data) {
                                    orderService
                                        .deliverOrder(data.record.id)
                                        .then(function () {
                                            dataTable.ajax.reload();
                                            abp.notify.success(l('Order:OrderDeliveredSuccessfully'));
                                        })
                                        .catch(function (error) {
                                            abp.notify.error(error.message || l('Order:DeliverOrderFailed'));
                                        });
                                }
                            },
                            {
                                text: l('ConfirmCodPayment'),
                                visible: function (data) {
                                    return abp.auth.isGranted('ProductSelling.Orders.ConfirmCodPayment') &&
                                        data.paymentMethod === 'COD' &&
                                        data.paymentStatusText === 'PendingOnDelivery';
                                },
                                confirmMessage: function (data) {
                                    return l('Order:AreYouSureYouWantToConfirmCodPaymentForOrder', data.orderNumber);
                                },
                                action: function (data) {
                                    orderService
                                        .markAsCodPaidAndCompleted(data.record.id)
                                        .then(function () {
                                            dataTable.ajax.reload();
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
                            .toFormat("dd/MM/yyyy HH:mm:ss");
                    }
                },
                {
                    title: l('CustomerName'),
                    data: "customerName"
                },
                {
                    title: l('Order:TotalAmount'),
                    data: "totalAmount",
                    render: function (data) {
                        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data);
                    }
                },
                {
                    title: l('Order:OrderStatus'),
                    data: "orderStatusText", // FIXED: Use orderStatusText instead of orderStatus
                    render: function (data, type, row) {
                        // data is now "Placed", "Confirmed", etc.
                        var statusText = data || 'Unknown';
                        var badgeClass = getStatusBadgeClass(statusText);

                        // Try to get localized text
                        var localizedText = l('Enum:OrderStatus.' + statusText);
                        var displayText = localizedText !== ('Enum:OrderStatus.' + statusText)
                            ? localizedText
                            : statusText;

                        return `<span class="badge ${badgeClass}">${displayText}</span>`;
                    }
                },
                {
                    title: l('Order:PaymentMethod'),
                    data: "paymentMethod"
                },
                {
                    title: l('Order:PaymentStatus'),
                    data: "paymentStatusText", // FIXED: Use paymentStatusText instead of paymentStatus
                    render: function (data, type, row) {
                        var status = data || 'Unpaid';
                        var badgeClass = getPaymentStatusBadgeClass(status);

                        // Try to get localized text
                        var localizedText = l('Enum:PaymentStatus.' + status);
                        var displayText = localizedText !== ('Enum:PaymentStatus.' + status)
                            ? localizedText
                            : status;

                        return `<span class="badge ${badgeClass}">${displayText}</span>`;
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
            case 'Cancelled': return 'bg-secondary';
            default: return 'bg-dark text-light';
        }
    }

    $('#NewOrderButton').click(function (e) {
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