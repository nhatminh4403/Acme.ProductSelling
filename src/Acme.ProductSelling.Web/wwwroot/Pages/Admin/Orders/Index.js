$(function () {
    var orderService = acme.productSelling.orders.order;
    var storeService = acme.productSelling.stores.store;
    var l = abp.localization.getResource('ProductSelling');

    let stores = [];
    let currentFilters = {
        includeDeleted: false,
        storeId: window.currentUserStoreId || null,
        orderType: null,
        orderStatus: null
    };

    // Load stores for admin/manager
    if (window.isAdminOrManager) {
        storeService.getList({ maxResultCount: 1000 }).then(function (result) {
            stores = result.items;
            const $select = $('#storeFilter');
            stores.forEach(function (store) {
                $select.append(`<option value="${store.id}">${store.name} (${store.code})</option>`);
            });

            // Set initial filter if staff
            if (currentFilters.storeId) {
                $select.val(currentFilters.storeId);
            }
        });
    }

    var getFilter = function () {
        return currentFilters;
    };

    // Apply filters button
    $('#applyFiltersBtn').on('click', function () {
        currentFilters.storeId = $('#storeFilter').val() || null;
        currentFilters.orderType = $('#orderTypeFilter').val() || null;
        currentFilters.orderStatus = $('#orderStatusFilter').val() || null;
        dataTable.ajax.reload();
    });

    $('#IncludeDeletedCheckbox').on('change', function () {
        currentFilters.includeDeleted = $(this).is(':checked');
        dataTable.ajax.reload();
    });

    // DataTables configuration
    var dataTable = $('#OrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[2, "desc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(orderService.getList, getFilter),
            rowId: 'id',
            columnDefs: [
                {
                    title: l('Order:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Orders.Edit') ||
                        abp.auth.isGranted('ProductSelling.Orders.Delete'),
                    rowAction: {
                        items: [
                            {
                                text: l('View'),
                                action: function (data) {
                                    window.location.href = `/${window.rolePrefix}/orders/details/${data.record.orderNumber}`;
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
                                    return abp.auth.isGranted('ProductSelling.Orders.Edit') &&
                                        (data.orderStatusText === 'Confirmed' || data.orderStatusText === 'Processing');
                                },
                                confirmMessage: function (data) {
                                    return l('Order:AreYouSureYouWantToShipOrder', data.orderNumber);
                                },
                                action: function (data) {
                                    orderService.shipOrder(data.record.id)
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
                                    orderService.deliverOrder(data.record.id)
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
                                    orderService.markAsCodPaidAndCompleted(data.record.id)
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
                    title: l('Order:Number'),
                    data: "orderNumber",
                    render: function (data, type, row) {
                        return `<a href="/${window.rolePrefix}/orders/details/${data}">${data}</a>`;
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
                    title: l('Store'),
                    data: "storeName",
                    visible: window.isAdminOrManager,
                    render: function (data) {
                        return data ? `<span class="badge bg-secondary">${data}</span>` : '-';
                    }
                },
                {
                    title: l('OrderType'),
                    data: "orderType",
                    render: function (data) {
                        return data === 1
                            ? `<span class="badge bg-info">${l('InStore')}</span>`
                            : `<span class="badge bg-primary">${l('Online')}</span>`;
                    }
                },
                {
                    title: l('Order:TotalAmount'),
                    data: "totalAmount",
                    render: function (data) {
                        return new Intl.NumberFormat('vi-VN', {
                            style: 'currency',
                            currency: 'VND'
                        }).format(data);
                    }
                },
                {
                    title: l('Order:OrderStatus'),
                    data: "orderStatusText",
                    render: function (data, type, row) {
                        var statusText = data || 'Unknown';
                        var badgeClass = getStatusBadgeClass(statusText);
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
                    data: "paymentStatusText",
                    render: function (data, type, row) {
                        var status = data || 'Unpaid';
                        var badgeClass = getPaymentStatusBadgeClass(status);
                        var localizedText = l('Enum:PaymentStatus.' + status);
                        var displayText = localizedText !== ('Enum:PaymentStatus.' + status)
                            ? localizedText
                            : status;

                        return `<span class="badge ${badgeClass}">${displayText}</span>`;
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
            case 'Processing': return 'bg-warning text-dark';
            case 'Shipped': return 'bg-success';
            case 'Delivered': return 'bg-dark';
            case 'Cancelled': return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    function getPaymentStatusBadgeClass(status) {
        switch (status) {
            case 'Unpaid': return 'bg-secondary';
            case 'PendingOnDelivery': return 'bg-info text-white';
            case 'Pending': return 'bg-warning text-dark';
            case 'Paid': return 'bg-success';
            case 'Failed': return 'bg-danger';
            case 'Refunded': return 'bg-dark';
            case 'Cancelled': return 'bg-secondary';
            default: return 'bg-dark text-light';
        }
    }
});