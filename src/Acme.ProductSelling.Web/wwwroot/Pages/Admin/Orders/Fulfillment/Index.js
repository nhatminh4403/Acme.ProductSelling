(function () {
    const l = abp.localization.getResource('ProductSelling');
    const orderService = acme.productSelling.orders.services.orderAppService;

    const dataTable = $('#FulfillmentOrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, 'asc']],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(orderService.getStoreOrders, function () {
                return {
                    orderType: 1, // InStore
                    orderStatus: 2, // Confirmed (paid and ready for fulfillment)
                    paymentStatus: 3, // Paid
                    sorting: 'CompletedAt ASC'
                };
            }),
            columnDefs: [
                {
                    title: l('OrderNumber'),
                    data: 'orderNumber'
                },
                {
                    title: l('CompletedAt'),
                    data: 'completedAt',
                    render: function (data) {
                        return data ? luxon.DateTime.fromISO(data).toFormat('dd/MM/yyyy HH:mm') : '-';
                    }
                },
                {
                    title: l('CustomerName'),
                    data: 'customerName'
                },
                {
                    title: l('Seller'),
                    data: 'sellerName'
                },
                {
                    title: l('Cashier'),
                    data: 'cashierName'
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
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-primary view-items-btn" data-id="${row.id}">
                                    <i class="fa fa-box-open"></i> ${l('ViewItems')}
                                </button>
                                <button class="btn btn-sm btn-success fulfill-btn" data-order='${JSON.stringify(row)}'>
                                    <i class="fa fa-check"></i> ${l('Fulfill')}
                                </button>`;
                    }
                }
            ]
        })
    );

    const $modal = $('#fulfillOrderModal');
    let currentOrderId = null;

    $(document).on('click', '.fulfill-btn', function () {
        const order = JSON.parse($(this).attr('data-order'));
        currentOrderId = order.id;

        $('#fulfillOrderId').val(order.id);
        $('#fulfillOrderNumber').text(order.orderNumber);
        $('#fulfillCustomerName').text(order.customerName);

        // Load order items
        orderService.get(order.id).then(function (response) {
            const $tbody = $('#fulfillItemsBody');
            $tbody.empty();

            response.orderItems.forEach(function (item) {
                $tbody.append(`
                    <tr>
                        <td>${item.productName}</td>
                        <td>${item.quantity}</td>
                        <td>
                            <div class="form-check">
                                <input class="form-check-input item-prepared" type="checkbox" 
                                       data-item-id="${item.id}" />
                                <label class="form-check-label">
                                    ${l('Prepared')}
                                </label>
                            </div>
                        </td>
                    </tr>
                `);
            });

            $modal.modal('show');
        });
    });

    $modal.find('.btn-primary').on('click', function () {
        const allPrepared = $('.item-prepared:checked').length === $('.item-prepared').length;

        if (!allPrepared) {
            abp.message.confirm(
                l('NotAllItemsPreparedConfirm'),
                l('Confirm'),
                function (confirmed) {
                    if (confirmed) {
                        fulfillOrder();
                    }
                }
            );
        } else {
            fulfillOrder();
        }
    });

    function fulfillOrder() {
        orderService.fulfillInStoreOrder(currentOrderId).then(function () {
            abp.notify.success(l('OrderFulfilledSuccessfully'));
            $modal.modal('hide');
            dataTable.ajax.reload();
        });
    }

    $(document).on('click', '.view-items-btn', function () {
        const orderId = $(this).data('id');
        window.location.href = `/${window.rolePrefix}/orders/detail/${orderId}`;
    });
})();