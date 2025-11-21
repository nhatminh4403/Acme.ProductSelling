$(function () {
    var l = abp.localization.getResource('ProductSelling');
    var prefix = window.location.pathname.split('/')[1];


    var dataTable = $('#StoresTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(acme.productSelling.stores.store.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Stores.Edit'),
                                action: function (data) {
                                    window.location.href = `/${prefix}/stores/edit/` + data.record.id;

                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Stores.Delete'),
                                confirmMessage: function (data) {
                                    return l('StoreDeletionConfirmationMessage', data.record.name);
                                },
                                action: function (data) {
                                    acme.productSelling.stores.store
                                        .delete(data.record.id)
                                        .then(function () {
                                            abp.notify.success(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            },
                            {
                                text: function (data) {
                                    return data.isActive ? l('Deactivate') : l('Activate');
                                },
                                visible: abp.auth.isGranted('ProductSelling.Stores.Edit'),
                                action: function (data) {
                                    var action = data.record.isActive
                                        ? acme.productSelling.stores.store.deactivate
                                        : acme.productSelling.stores.store.activate;

                                    action(data.record.id).then(function () {
                                        abp.notify.success(l('Successfully' + (data.record.isActive ? 'Deactivated' : 'Activated')));
                                        dataTable.ajax.reload();
                                    });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Code'),
                    data: "code"
                },
                {
                    title: l('Name'),
                    data: "name"
                },
                {
                    title: l('Address'),
                    data: "address"
                },
                {
                    title: l('City'),
                    data: "city"
                },
                {
                    title: l('PhoneNumber'),
                    data: "phoneNumber"
                },
                {
                    title: l('Status'),
                    data: "isActive",
                    render: function (data) {
                        return data
                            ? '<span class="badge bg-success">' + l('Active') + '</span>'
                            : '<span class="badge bg-secondary">' + l('Inactive') + '</span>';
                    }
                }
            ]
        })
    );

    $('#NewStoreButton').click(function (e) {
        e.preventDefault();
        window.location.href = `/${prefix}/stores/create`;
    });
});