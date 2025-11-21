(function () {
    const l = abp.localization.getResource('ProductSelling');
    const orderService = acme.productSelling.orders.services.orderAppService;

    const dataTable = $('#PendingPaymentOrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, 'asc']],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(orderService.getStoreOrders, function () {
                return {
                    orderType: 1, // InStore
                    orderStatus: 1, // Placed or Pending - adjust based on your workflow
                    paymentStatus: 0, // Unpaid
                    sorting: 'CreationTime ASC'
                };
            }),
            columnDefs: [
                {
                    title: l('OrderNumber'),
                    data: 'orderNumber'
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
                    title: l('Seller'),
                    data: 'sellerName'
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
                        return `<button class="btn btn-sm btn-success complete-payment-btn" data-order='${JSON.stringify(row)}'>
                                    <i class="fa fa-cash-register"></i> ${l('CompletePayment')}
                                </button>`;
                    }
                }
            ]
        })
    );

    const $modal = $('#completePaymentModal');
    const $form = $('#completePaymentForm');
    let currentOrderId = null;

    $(document).on('click', '.complete-payment-btn', function () {
        const order = JSON.parse($(this).attr('data-order'));
        currentOrderId = order.id;

        $('#orderId').val(order.id);
        $('#orderNumber').val(order.orderNumber);
        $('#customerName').val(order.customerName);
        $('#orderTotal').val(formatCurrency(order.totalAmount));
        $('#paidAmount').val(order.totalAmount);
        calculateChange(order.totalAmount);

        $modal.modal('show');
    });

    $('#paidAmount').on('input', function () {
        const total = parseFloat($('#orderTotal').val().replace(/[^\d]/g, ''));
        calculateChange(total);
    });

    function calculateChange(total) {
        const paid = parseFloat($('#paidAmount').val()) || 0;
        const change = paid - total;
        $('#changeAmount').val(formatCurrency(change >= 0 ? change : 0));
    }

    $modal.find('.btn-primary').on('click', function () {
        const paidAmount = parseFloat($('#paidAmount').val());
        if (!paidAmount || paidAmount <= 0) {
            abp.message.error(l('PleaseEnterValidAmount'));
            return;
        }

        orderService.completeInStorePayment(currentOrderId, {
            paidAmount: paidAmount
        }).then(function () {
            abp.notify.success(l('PaymentCompletedSuccessfully'));
            $modal.modal('hide');
            dataTable.ajax.reload();
        });
    });

    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }
})();