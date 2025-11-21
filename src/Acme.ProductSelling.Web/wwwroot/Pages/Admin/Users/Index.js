(function () {
    const l = abp.localization.getResource('ProductSelling');
    const userService = acme.productSelling.users.userManagementAppService;
    const storeService = acme.productSelling.stores.services.storeAppService;

    let stores = [];

    // Load stores
    storeService.getList({ maxResultCount: 1000 }).then(function (result) {
        stores = result.items;
        populateStoreSelect();
    });

    // Staff Users Table
    const staffTable = $('#StaffUsersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: false,
            paging: true,
            order: [[0, 'asc']],
            searching: true,
            scrollX: true,
            ajax: function (data, callback) {
                storeService.getList({ maxResultCount: 1000 }).then(function (storeResult) {
                    const promises = storeResult.items.map(function (store) {
                        return userService.getStoreStaff(store.id, { maxResultCount: 1000 });
                    });

                    Promise.all(promises).then(function (results) {
                        const allStaff = [];
                        results.forEach(function (result) {
                            allStaff.push(...result.items);
                        });
                        callback({
                            recordsTotal: allStaff.length,
                            recordsFiltered: allStaff.length,
                            data: allStaff
                        });
                    });
                });
            },
            columnDefs: [
                {
                    title: l('UserName'),
                    data: 'userName'
                },
                {
                    title: l('Email'),
                    data: 'email'
                },
                {
                    title: l('Name'),
                    data: null,
                    render: function (data) {
                        return `${data.name || ''} ${data.surname || ''}`.trim();
                    }
                },
                {
                    title: l('Roles'),
                    data: 'roles',
                    render: function (data) {
                        return data.map(role => `<span class="badge bg-info me-1">${role}</span>`).join('');
                    }
                },
                {
                    title: l('AssignedStore'),
                    data: 'assignedStoreName',
                    render: function (data) {
                        return data ? `<span class="badge bg-success">${data}</span>` : '-';
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let buttons = `<button class="btn btn-sm btn-primary assign-store-btn" data-user='${JSON.stringify(row)}'>
                                          <i class="fa fa-store"></i> ${row.assignedStoreName ? l('Reassign') : l('Assign')}
                                      </button>`;

                        if (row.assignedStoreId) {
                            buttons += ` <button class="btn btn-sm btn-warning unassign-store-btn" data-user-id="${row.id}">
                                            <i class="fa fa-times"></i> ${l('Unassign')}
                                        </button>`;
                        }

                        return buttons;
                    }
                }
            ]
        })
    );

    // Unassigned Staff Table
    const unassignedTable = $('#UnassignedStaffTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, 'asc']],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(userService.getUnassignedStaff),
            columnDefs: [
                {
                    title: l('UserName'),
                    data: 'userName'
                },
                {
                    title: l('Email'),
                    data: 'email'
                },
                {
                    title: l('Name'),
                    data: null,
                    render: function (data) {
                        return `${data.name || ''} ${data.surname || ''}`.trim();
                    }
                },
                {
                    title: l('Roles'),
                    data: 'roles',
                    render: function (data) {
                        return data.map(role => `<span class="badge bg-info me-1">${role}</span>`).join('');
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-primary assign-store-btn" data-user='${JSON.stringify(row)}'>
                                    <i class="fa fa-store"></i> ${l('AssignToStore')}
                                </button>`;
                    }
                }
            ]
        })
    );

    // Modal handling
    const $modal = $('#assignStoreModal');

    $(document).on('click', '.assign-store-btn', function () {
        const user = JSON.parse($(this).attr('data-user'));

        $('#userId').val(user.id);
        $('#userName').val(user.userName);
        $('#userEmail').val(user.email);
        $('#userRoles').val(user.roles.join(', '));
        $('#storeId').val(user.assignedStoreId || '');

        $modal.modal('show');
    });

    $modal.find('.btn-primary').on('click', function () {
        const userId = $('#userId').val();
        const storeId = $('#storeId').val();

        if (!storeId) {
            abp.message.error(l('PleaseSelectStore'));
            return;
        }

        userService.assignStaffToStore(userId, storeId).then(function () {
            abp.notify.success(l('StaffAssignedSuccessfully'));
            $modal.modal('hide');
            staffTable.ajax.reload();
            unassignedTable.ajax.reload();
        });
    });

    $(document).on('click', '.unassign-store-btn', function () {
        const userId = $(this).data('user-id');

        abp.message.confirm(
            l('UnassignStoreConfirmation'),
            l('Confirm'),
            function (confirmed) {
                if (confirmed) {
                    userService.unassignStaffFromStore(userId).then(function () {
                        abp.notify.success(l('StaffUnassignedSuccessfully'));
                        staffTable.ajax.reload();
                        unassignedTable.ajax.reload();
                    });
                }
            }
        );
    });

    function populateStoreSelect() {
        const $select = $('#storeId');
        $select.empty().append('<option value="">-- ' + l('SelectStore') + ' --</option>');

        stores.forEach(function (store) {
            $select.append(`<option value="${store.id}">${store.name} (${store.code})</option>`);
        });
    }
})();