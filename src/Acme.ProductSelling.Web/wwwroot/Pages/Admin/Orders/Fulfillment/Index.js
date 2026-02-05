(function () {
    const l = abp.localization.getResource('ProductSelling');

    // Using the refactored services
    const queryService = acme.productSelling.orders.services.orderQuery;
    const inStoreService = acme.productSelling.orders.services.inStoreOrder;
    const publicService = acme.productSelling.orders.services.orderPublic; // For detail 'get' method

    const dataTable = $('#FulfillmentOrdersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, 'asc']], // Order by time they were paid
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(queryService.getList, function () {
                return {
                    orderType: 1,      
                    orderStatus: 2,    // Confirmed/Paid (Ready to fulfill)
                    paymentStatus: 3,  // Paid
                    sorting: 'CreationTime ASC'
                };
            }),
            columnDefs: [
                {
                    title: l('OrderNumber'),
                    data: 'orderNumber'
                },
                {
                    title: l('PaidAt'),
                    data: 'creationTime', // Or use a custom LastModificationTime/StatusChange time
                    render: function (data) {
                        return luxon.DateTime.fromISO(data).toFormat('dd/MM/yyyy HH:mm');
                    }
                },
                {
                    title: l('Customer'),
                    data: 'customerName'
                },
                {
                    title: l('Seller'),
                    data: 'sellerName' // Mapping must exist in OrderDto
                },
                {
                    title: l('Cashier'),
                    data: 'cashierName' // Mapping must exist in OrderDto
                },
                {
                    title: l('TotalItems'),
                    data: 'orderItems',
                    render: function (data) {
                        return data ? data.length : 0;
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <button class="btn btn-sm btn-success fulfill-btn" data-id="${row.id}" data-num="${row.orderNumber}" data-name="${row.customerName}">
                                <i class="fa fa-dolly"></i> ${l('Fulfill')}
                            </button>
                            <button class="btn btn-sm btn-info view-detail-btn" data-id="${row.id}">
                                <i class="fa fa-eye"></i>
                            </button>`;
                    }
                }
            ]
        })
    );

    const $modal = $('#fulfillOrderModal');
    let currentOrderId = null;

    // 1. ACTION: Open Fulfillment Modal
    $(document).on('click', '.fulfill-btn', function () {
        currentOrderId = $(this).data('id');

        $('#fulfillOrderNumber').text($(this).data('num'));
        $('#fulfillCustomerName').text($(this).data('name'));

        // Load order details including items from Public or Query service
        publicService.get(currentOrderId).then(function (response) {
            const $tbody = $('#fulfillItemsBody');
            $tbody.empty();

            response.orderItems.forEach(function (item) {
                $tbody.append(`
                    <tr>
                        <td class="align-middle"><strong>${item.productName}</strong></td>
                        <td class="text-center align-middle">
                            <span class="badge bg-secondary h5 mb-0">${item.quantity}</span>
                        </td>
                        <td class="text-end">
                            <input type="checkbox" class="btn-check item-prepared-check" 
                                   id="btn-check-${item.id}" autocomplete="off">
                            <label class="btn btn-outline-success" for="btn-check-${item.id}">
                                <i class="fa fa-check"></i> ${l('Packed')}
                            </label>
                        </td>
                    </tr>
                `);
            });

            $modal.modal('show');
        });
    });

    // 2. ACTION: Submit Fulfillment
    $modal.find('.btn-complete-fulfillment').on('click', function () {
        const totalItems = $('.item-prepared-check').length;
        const checkedItems = $('.item-prepared-check:checked').length;

        if (checkedItems < totalItems) {
            abp.message.confirm(
                l('MissingItemsFulfillmentWarning', totalItems - checkedItems),
                l('ItemsMissing'),
                function (confirmed) {
                    if (confirmed) {
                        executeFulfillment();
                    }
                }
            );
        } else {
            executeFulfillment();
        }
    });

    function executeFulfillment() {
        inStoreService.fulfillInStoreOrder(currentOrderId).then(function () {
            abp.notify.success(l('OrderFulfilledAndGivenToCustomer'));
            $modal.modal('hide');
            dataTable.ajax.reload();
        });
    }

    // 3. ACTION: View Full Details
    $(document).on('click', '.view-detail-btn', function () {
        const id = $(this).data('id');
        // Redirection based on your dynamic prefix logic
        const prefix = window.location.pathname.split('/')[1];
        window.location.href = `/${prefix}/orders/detail/${id}`;
    });

})();