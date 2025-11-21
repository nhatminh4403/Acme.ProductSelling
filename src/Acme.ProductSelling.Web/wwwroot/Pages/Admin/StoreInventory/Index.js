(function () {
    const l = abp.localization.getResource('ProductSelling');
    const inventoryService = acme.productSelling.stores.services.storeInventoryAppService;
    const storeService = acme.productSelling.stores.services.storeAppService;

    let stores = [];
    let currentStoreFilter = window.currentUserStoreId || null;

    // Load stores for filter
    if (window.isAdminOrManager) {
        storeService.getList({ maxResultCount: 1000 }).then(function (result) {
            stores = result.items;
            populateStoreFilter();
        });
    }

    const dataTable = $('#StoreInventoryTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, 'asc']],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(inventoryService.getList, function () {
                const params = {};
                if (currentStoreFilter) {
                    params.storeId = currentStoreFilter;
                }
                return params;
            }),
            columnDefs: [
                {
                    title: l('ProductName'),
                    data: 'productName'
                },
                {
                    title: l('Store'),
                    data: 'storeName'
                },
                {
                    title: l('Quantity'),
                    data: 'quantity',
                    render: function (data, type, row) {
                        const badgeClass = row.needsReorder ? 'bg-danger' : (data > 50 ? 'bg-success' : 'bg-warning');
                        return `<span class="badge ${badgeClass}">${data}</span>`;
                    }
                },
                {
                    title: l('ReorderLevel'),
                    data: 'reorderLevel'
                },
                {
                    title: l('ReorderQuantity'),
                    data: 'reorderQuantity'
                },
                {
                    title: l('IsAvailable'),
                    data: 'isAvailableForSale',
                    render: function (data) {
                        return data
                            ? `<span class="badge bg-success">${l('Yes')}</span>`
                            : `<span class="badge bg-secondary">${l('No')}</span>`;
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-primary adjust-inventory-btn" data-inventory='${JSON.stringify(row)}'>
                                    <i class="fa fa-edit"></i> ${l('Adjust')}
                                </button>`;
                    }
                }
            ]
        })
    );

    // Store filter change
    $('#storeFilter').on('change', function () {
        currentStoreFilter = $(this).val() || null;
        dataTable.ajax.reload();
    });

    // Adjust inventory modal
    const $modal = $('#adjustInventoryModal');
    let currentInventoryId = null;
    let currentQuantity = 0;

    $(document).on('click', '.adjust-inventory-btn', function () {
        const inventory = JSON.parse($(this).attr('data-inventory'));
        currentInventoryId = inventory.id;
        currentQuantity = inventory.quantity;

        $('#inventoryId').val(inventory.id);
        $('#productName').val(inventory.productName);
        $('#storeName').val(inventory.storeName);
        $('#currentQuantity').val(inventory.quantity);
        $('#quantityChange').val(0);
        $('#newQuantity').val(inventory.quantity);
        $('#reason').val('');

        $modal.modal('show');
    });

    $('#quantityChange').on('input', function () {
        const change = parseInt($(this).val()) || 0;
        const newQty = currentQuantity + change;
        $('#newQuantity').val(newQty >= 0 ? newQty : 0);

        if (newQty < 0) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    $modal.find('.btn-primary').on('click', function () {
        const quantityChange = parseInt($('#quantityChange').val());
        const reason = $('#reason').val();

        if (isNaN(quantityChange) || quantityChange === 0) {
            abp.message.error(l('PleaseEnterValidQuantityChange'));
            return;
        }

        const newQty = currentQuantity + quantityChange;
        if (newQty < 0) {
            abp.message.error(l('QuantityCannotBeNegative'));
            return;
        }

        inventoryService.adjustInventory(currentInventoryId, {
            quantityChange: quantityChange,
            reason: reason
        }).then(function () {
            abp.notify.success(l('InventoryAdjustedSuccessfully'));
            $modal.modal('hide');
            dataTable.ajax.reload();
        });
    });

    function populateStoreFilter() {
        const $select = $('#storeFilter');
        $select.append('<option value="">-- ' + l('AllStores') + ' --</option>');

        stores.forEach(function (store) {
            $select.append(`<option value="${store.id}">${store.name} (${store.code})</option>`);
        });
    }
})();