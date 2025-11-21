(function () {
    const l = abp.localization.getResource('ProductSelling');
    const inventoryService = acme.productSelling.products.services.storeInventoryAppService;
    const storeService = acme.productSelling.stores.services.storeAppService;

    let stores = [];
    let currentStoreFilter = window.currentUserStoreId || null;
    let criticalOnlyFilter = false;

    // Load stores for filter (admin/manager only)
    if (window.isAdminOrManager) {
        storeService.getList({ maxResultCount: 1000 }).then(function (result) {
            stores = result.items;
            populateStoreFilter();
        });
    }

    const dataTable = $('#LowStockTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[6, 'asc']], // Order by stock level percentage
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(inventoryService.getLowStockItems, function () {
                const params = {};
                if (currentStoreFilter) {
                    params.storeId = currentStoreFilter;
                }
                if (criticalOnlyFilter) {
                    params.criticalOnly = true;
                }
                return params;
            }),
            columnDefs: [
                {
                    title: l('lowstock:Product'),
                    data: 'productName',
                    render: function (data, type, row) {
                        let html = '';
                        if (row.productImageUrl) {
                            html += `<img src="${row.productImageUrl}" style="width:40px;height:40px;object-fit:cover;margin-right:8px;border-radius:4px;" />`;
                        }
                        html += `<span>${data}</span>`;
                        return html;
                    }
                },
                {
                    title: l('lowstock:Store'),
                    data: 'storeName',
                    render: function (data) {
                        return `<span class="badge bg-secondary">${data}</span>`;
                    }
                },
                {
                    title: l('lowstock:CurrentStock'),
                    data: 'quantity',
                    render: function (data, type, row) {
                        const percentage = row.stockLevelPercentage;
                        const badgeClass = percentage <= 25 ? 'bg-danger' : (percentage <= 50 ? 'bg-warning' : 'bg-info');
                        return `<span class="badge ${badgeClass} fs-6">${data}</span>`;
                    }
                },
                {
                    title: l('lowstock:ReorderLevel'),
                    data: 'reorderLevel'
                },
                {
                    title: l('lowstock:SuggestedReorder'),
                    data: null,
                    render: function (data, type, row) {
                        const needed = Math.max(0, row.reorderLevel + row.reorderQuantity - row.quantity);
                        return `<strong class="text-primary">${needed}</strong>`;
                    }
                },
                {
                    title: l('lowstock:Status'),
                    data: null,
                    render: function (data, type, row) {
                        const percentage = row.stockLevelPercentage;
                        let statusHtml = '';

                        if (percentage <= 25) {
                            statusHtml = `<span class="badge bg-danger">
                                <i class="fa fa-fire"></i> ${l('lowstock:Critical')}
                            </span>`;
                        } else if (percentage <= 50) {
                            statusHtml = `<span class="badge bg-warning">
                                <i class="fa fa-exclamation-triangle"></i> ${l('lowstock:Low')}
                            </span>`;
                        } else {
                            statusHtml = `<span class="badge bg-info">
                                <i class="fa fa-exclamation-circle"></i> ${l('lowstock:NearReorder')}
                            </span>`;
                        }

                        return statusHtml;
                    }
                },
                {
                    title: l('lowstock:StockLevel'),
                    data: 'stockLevelPercentage',
                    render: function (data) {
                        const percentage = data;
                        let progressClass = 'bg-success';
                        if (percentage <= 25) progressClass = 'bg-danger';
                        else if (percentage <= 50) progressClass = 'bg-warning';
                        else if (percentage <= 75) progressClass = 'bg-info';

                        return `
                            <div class="progress" style="height: 24px; min-width: 100px;">
                                <div class="progress-bar ${progressClass}" role="progressbar" 
                                     style="width: ${percentage}%" 
                                     aria-valuenow="${percentage}" aria-valuemin="0" aria-valuemax="100">
                                    <strong>${percentage}%</strong>
                                </div>
                            </div>
                        `;
                    }
                },
                {
                    title: l('lowstock:Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <button class="btn btn-sm btn-primary quick-reorder-btn" data-inventory='${JSON.stringify(row)}'>
                                <i class="fa fa-plus"></i> ${l('lowstock:QuickReorder')}
                            </button>
                        `;
                    }
                }
            ],
            drawCallback: function (settings) {
                updateStatistics(settings.json);
            }
        })
    );

    // Store filter change
    $('#storeFilter').on('change', function () {
        currentStoreFilter = $(this).val() || null;
        dataTable.ajax.reload();
    });

    // Critical only filter
    $('#criticalOnlyFilter').on('change', function () {
        criticalOnlyFilter = $(this).is(':checked');
        dataTable.ajax.reload();
    });

    // Quick Reorder Modal
    const $reorderModal = $('#quickReorderModal');

    $(document).on('click', '.quick-reorder-btn', function () {
        const inventory = JSON.parse($(this).attr('data-inventory'));

        $('#reorderInventoryId').val(inventory.id);
        $('#reorderProductName').val(inventory.productName);
        $('#reorderStoreName').val(inventory.storeName);
        $('#reorderCurrentQty').val(inventory.quantity);
        $('#reorderLevel').val(inventory.reorderLevel);

        // Calculate suggested quantity (to reach reorder level + reorder quantity)
        const suggested = Math.max(
            inventory.reorderQuantity,
            inventory.reorderLevel + inventory.reorderQuantity - inventory.quantity
        );
        $('#reorderQuantity').val(suggested);
        $('#reorderHint').text(`${l('lowstock:.DefaultReorderQty')}: ${inventory.reorderQuantity}`);
        $('#reorderNotes').val(`Low stock reorder for ${inventory.productName}`);

        $reorderModal.modal('show');
    });

    $reorderModal.find('.btn-primary').on('click', function () {
        const inventoryId = $('#reorderInventoryId').val();
        const quantity = parseInt($('#reorderQuantity').val());
        const notes = $('#reorderNotes').val();

        if (!quantity || quantity <= 0) {
            abp.message.error(l('lowstock.PleaseEnterValidQuantity'));
            return;
        }

        inventoryService.adjustQuantity(inventoryId, {
            quantityChange: quantity,
            reason: notes || 'Low stock reorder'
        }).then(function () {
            abp.notify.success(l('lowstock.StockReplenished'));
            $reorderModal.modal('hide');
            dataTable.ajax.reload();
        }).catch(function (error) {
            abp.message.error(error.message || l('lowstock.AnErrorOccurred'));
        });
    }); $('#generateReorderListBtn').on('click', function () {
        const $btn = $(this);
        $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> ' + l('Generating'));

        const params = {
            maxResultCount: 1000 // Get all items for report
        };
        if (currentStoreFilter) params.storeId = currentStoreFilter;
        if (criticalOnlyFilter) params.criticalOnly = true;

        inventoryService.getLowStockItems(params).then(function (result) {
            generateReorderListPDF(result.items);
            $btn.prop('disabled', false).html('<i class="fa fa-file-export"></i> ' + l('GenerateReorderList'));
        }).catch(function () {
            $btn.prop('disabled', false).html('<i class="fa fa-file-export"></i> ' + l('GenerateReorderList'));
        });
    });

    function generateReorderListPDF(items) {
        if (!items || items.length === 0) {
            abp.message.warn(l('NoItemsToGenerate'));
            return;
        }

        const now = new Date();
        const formattedDate = now.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });

        let content = `
            <!DOCTYPE html>
            <html>
            <head>
                <title>${l('lowstock:LowStockReorderList')}</title>
                <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css">
                <style>
                    body { 
                        padding: 30px; 
                        font-family: Arial, sans-serif;
                    }
                    .header {
                        border-bottom: 3px solid #333;
                        padding-bottom: 15px;
                        margin-bottom: 20px;
                    }
                    .table {
                        font-size: 14px;
                    }
                    .table thead {
                        background-color: #f8f9fa;
                    }
                    .critical-row {
                        background-color: #ffe6e6;
                    }
                    .warning-row {
                        background-color: #fff9e6;
                    }
                    .footer {
                        margin-top: 30px;
                        border-top: 2px solid #ccc;
                        padding-top: 15px;
                    }
                    @media print {
                        .no-print { display: none; }
                        body { padding: 15px; }
                    }
                </style>
            </head>
            <body>
                <div class="no-print mb-3">
                    <button class="btn btn-primary" onclick="window.print()">
                        <i class="fa fa-print"></i> ${l('lowstock:Print')}
                    </button>
                    <button class="btn btn-secondary" onclick="window.close()">
                        <i class="fa fa-times"></i> ${l('lowstock:Close')}
                    </button>
                </div>

                <div class="header">
                    <h2>${l('lowstock:LowStockReorderList')}</h2>
                    <p class="mb-0"><strong>${l('lowstock:GeneratedAt')}:</strong> ${formattedDate}</p>
                    ${currentStoreFilter ? `<p class="mb-0"><strong>${l('lowstock:Store')}:</strong> ${getStoreName(currentStoreFilter)}</p>` : '<p class="mb-0"><strong>' + l('lowstock:Scope') + ':</strong> ' + l('lowstock:AllStores') + '</p>'}
                    ${criticalOnlyFilter ? `<p class="mb-0 text-danger"><strong>${l('lowstock:Filter')}:</strong> ${l('lowstock:CriticalItemsOnly')}</p>` : ''}
                    <p class="mb-0"><strong>${l('lowstock:TotalItems')}:</strong> ${items.length}</p>
                </div>

                <table class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>${l('lowstock:Product')}</th>
                            <th>${l('lowstock:Store')}</th>
                            <th class="text-center">${l('lowstock:CurrentStock')}</th>
                            <th class="text-center">${l('lowstock:ReorderLevel')}</th>
                            <th class="text-center">${l('lowstock:SuggestedQuantity')}</th>
                            <th class="text-center">${l('lowstock:Status')}</th>
                        </tr>
                    </thead>
                    <tbody>
        `;

        items.forEach((item, index) => {
            const needed = Math.max(0, item.reorderLevel + item.reorderQuantity - item.quantity);
            const percentage = item.stockLevelPercentage;
            const rowClass = percentage <= 25 ? 'critical-row' : (percentage <= 50 ? 'warning-row' : '');
            const statusText = percentage <= 25 ? l('lowstock:Critical') : (percentage <= 50 ? l('lowstock:Low') : l('lowstock:NearReorder'));

            content += `
                <tr class="${rowClass}">
                    <td>${index + 1}</td>
                    <td><strong>${item.productName}</strong></td>
                    <td>${item.storeName}</td>
                    <td class="text-center"><strong>${item.quantity}</strong></td>
                    <td class="text-center">${item.reorderLevel}</td>
                    <td class="text-center"><strong class="text-primary">${needed}</strong></td>
                    <td class="text-center">
                        <span class="badge ${percentage <= 25 ? 'bg-danger' : (percentage <= 50 ? 'bg-warning' : 'bg-info')}">
                            ${statusText} (${percentage}%)
                        </span>
                    </td>
                </tr>
            `;
        });

        // Calculate totals
        const totalSuggestedQty = items.reduce((sum, item) => {
            return sum + Math.max(0, item.reorderLevel + item.reorderQuantity - item.quantity);
        }, 0);

        const criticalCount = items.filter(item => item.stockLevelPercentage <= 25).length;
        const lowCount = items.filter(item => item.stockLevelPercentage > 25 && item.stockLevelPercentage <= 50).length;

        content += `
                    </tbody>
                    <tfoot class="table-light">
                        <tr>
                            <td colspan="5" class="text-end"><strong>${l('lowstock:TotalSuggestedQuantity')}:</strong></td>
                            <td class="text-center"><strong class="text-primary fs-5">${totalSuggestedQty}</strong></td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>

                <div class="footer">
                    <div class="row">
                        <div class="col-md-6">
                            <h5>${l('lowstock:Summary')}</h5>
                            <ul>
                                <li><strong>${l('lowstock:TotalItems')}:</strong> ${items.length}</li>
                                <li><strong>${l('lowstock:CriticalItems')}:</strong> <span class="text-danger">${criticalCount}</span></li>
                                <li><strong>${l('lowstock:LowStockItems')}:</strong> <span class="text-warning">${lowCount}</span></li>
                            </ul>
                        </div>
                        <div class="col-md-6">
                            <h5>${l('lowstock:Legend')}</h5>
                            <ul>
                                <li><span class="badge bg-danger">Critical</span> - ≤25% ${l('lowstock:OfReorderLevel')}</li>
                                <li><span class="badge bg-warning">Low</span> - 26-50% ${l('lowstock:OfReorderLevel')}</li>
                                <li><span class="badge bg-info">Near Reorder</span> - 51-100% ${l('lowstock:OfReorderLevel')}</li>
                            </ul>
                        </div>
                    </div>
                    
                    <div class="mt-4 text-center text-muted">
                        <p class="mb-0">_____________________________</p>
                        <p class="mb-0">${l('lowstock:PreparedBy')} / ${l('lowstock:Date')}</p>
                    </div>
                </div>
            </body>
            </html>
        `;

        // Open in new window for printing
        const printWindow = window.open('', '_blank', 'width=900,height=700');
        printWindow.document.write(content);
        printWindow.document.close();
    }

    function updateStatistics(jsonData) {
        if (!jsonData) return;

        const totalCount = jsonData.recordsTotal || 0;

        // Get current page data for statistics
        const tableData = dataTable.rows().data().toArray();

        let criticalCount = 0;
        tableData.forEach(item => {
            if (item.stockLevelPercentage <= 25) {
                criticalCount++;
            }
        });

        $('#lowStockCount').text(totalCount);
        $('#criticalStockCount').text(criticalCount);
        $('#totalItems').text(totalCount);
    }

    function populateStoreFilter() {
        const $select = $('#storeFilter');
        stores.forEach(function (store) {
            $select.append(`<option value="${store.id}">${store.name} (${store.code})</option>`);
        });
    }

    function getStoreName(storeId) {
        const store = stores.find(s => s.id === storeId);
        return store ? store.name : storeId;
    }
})();