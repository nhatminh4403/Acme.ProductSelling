(function () {
    const l = abp.localization.getResource('ProductSelling');

    // Using the refactored services
    const queryService = acme.productSelling.orders.services.orderQuery;
    const inStoreService = acme.productSelling.orders.services.inStoreOrder;

    const dataTable = $('#PendingPaymentOrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, 'desc']],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(queryService.getList, function () {
                return {

                    paymentStatus: 2, // Unpaid/Pending
                    sorting: 'CreationTime DESC'
                };
            }),
            columnDefs: [
                {
                    title: l('OrderNumber'),
                    data: 'orderNumber'
                },
                {
                    title: l('Type'),
                    data: 'orderType',
                    render: function (data) {
                        // Assuming 0 = Online, 1 = InStore based on your DTOs
                        const badge = data === 1 ? 'bg-success' : 'bg-info';
                        const text = data === 1 ? l('InStore') : l('Online');
                        return `<span class="badge ${badge}">${text}</span>`;
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
                    title: l('TotalAmount'),
                    data: 'totalAmount',
                    render: function (data) {
                        return formatCurrency(data);
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        if (row.orderType === 1) { // IN-STORE
                            return `<button class="btn btn-sm btn-success btn-checkout-instore" data-id="${row.id}" data-total="${row.totalAmount}" data-num="${row.orderNumber}">
                                        <i class="fa fa-cash-register"></i> ${l('CounterCheckout')}
                                    </button>`;
                        } else { // ONLINE
                            return `<button class="btn btn-sm btn-primary btn-confirm-online" data-id="${row.id}" data-num="${row.orderNumber}">
                                        <i class="fa fa-check-circle"></i> ${l('ConfirmTransfer')}
                                    </button>`;
                        }
                    }
                }
            ]
        })
    );

    const $inStoreModal = $('#completePaymentModal');
    let currentOrder = {};

    // 1. ACTION: In-Store Payment (Calculator)
    $(document).on('click', '.btn-checkout-instore', function () {
        currentOrder.id = $(this).data('id');
        currentOrder.total = $(this).data('total');

        $('#orderNumberLabel').text($(this).data('num'));
        $('#orderTotal').val(formatCurrency(currentOrder.total));
        $('#paidAmount').val(currentOrder.total);
        calculateChange(currentOrder.total);

        $inStoreModal.modal('show');
    });

    $('#paidAmount').on('input', function () {
        calculateChange(currentOrder.total);
    });

    $inStoreModal.find('.btn-save-payment').on('click', function () {
        const paid = parseFloat($('#paidAmount').val());
        inStoreService.completeInStorePayment(currentOrder.id, { paidAmount: paid })
            .then(function () {
                abp.notify.success(l('PaymentCompleted'));
                $inStoreModal.modal('hide');
                dataTable.ajax.reload();
            });
    });

    // 2. ACTION: Online Payment (Simple Manual Confirmation)
    $(document).on('click', '.btn-confirm-online', function () {
        const id = $(this).data('id');
        const num = $(this).data('num');

        abp.message.confirm(
            l('ConfirmPaymentManualMessage', num),
            l('AreYouSure'),
            function (isConfirmed) {
                if (isConfirmed) {
                    queryService.updateStatus(id, {
                        newStatus: 2, // Confirmed/Processing
                        newPaymentStatus: 1 // Paid
                    }).then(function () {
                        abp.notify.success(l('OrderMarkedAsPaid'));
                        dataTable.ajax.reload();
                    });
                }
            }
        );
    });

    // Helpers
    function calculateChange(total) {
        const paid = parseFloat($('#paidAmount').val()) || 0;
        const change = paid - total;
        $('#changeAmount').val(formatCurrency(change >= 0 ? change : 0));
    }

    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }
})();