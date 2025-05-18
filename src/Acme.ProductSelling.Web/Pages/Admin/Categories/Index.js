$(function () { 
     var categoryService = acme.productSelling.categories.category; 
    var l = abp.localization.getResource('ProductSelling'); 

    var createModal = new abp.ModalManager(abp.appPath + 'Categories/CreateModal'); 
    var editModal = new abp.ModalManager(abp.appPath + 'Categories/EditModal');  

    var dataTable = $('#CategoriesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({ 
            serverSide: true, 
            paging: true,
            order: [[1, "asc"]], 
            searching: false, 
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(categoryService.getList), 
            columnDefs: [
                {
                    title: l('Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Categories.Edit') || abp.auth.isGranted('ProductSelling.Categories.Delete'), // Hiện cột này nếu có quyền Edit hoặc Delete
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Categories.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Categories.Delete'),
                                confirmMessage: function (data) {
                                    return l('CategoryDeletionConfirmationMessage', data.record.name); 
                                },
                                action: function (data) {
                                    categoryService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted')); 
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Name'), 
                    data: "name"    
                },
                {
                    title: l('Description'), 
                    data: "description"     
                },
            ]
        })
    );

    $('#NewCategoryButton').click(function (e) {
        e.preventDefault();
        createModal.open(); // Mở Create Modal
    });

    createModal.onResult(function () {
        dataTable.ajax.reload(); // Load lại dữ liệu bảng
    });
    editModal.onResult(function () {
        dataTable.ajax.reload(); // Load lại dữ liệu bảng
    });

});
