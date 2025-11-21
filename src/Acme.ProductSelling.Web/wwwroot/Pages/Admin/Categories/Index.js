$(function () {
    var categoryService = acme.productSelling.categories.category;
    var l = abp.localization.getResource('ProductSelling');
    var prefix = window.location.pathname.split('/')[1];

    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Admin/Categories/CreateModal'
    });
    var editModal = new abp.ModalManager();

    var dataTable = $('#CategoriesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(categoryService.getList),
            columnDefs: [
                {
                    title: l('Category:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Categories.Edit') || abp.auth.isGranted('ProductSelling.Categories.Delete'),
                    rowAction: {
                        items: [
                            {
                                text: l('Category:Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Categories.Edit'),
                                action: function (data) {
                                    editModal.open({ url: `/${prefix}/categories/edit-modal/${data.record.id}` { id: data.record.id });
                                }
                            },
                            {
                                text: l('Category:Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Categories.Delete'),
                                confirmMessage: function (data) {
                                    return l('Category:CategoryDeletionConfirmationMessage', data.record.name);
                                },
                                action: function (data) {
                                    categoryService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('Category:SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Category:Name'),
                    data: "name"
                },
                {
                    title: l('Category:Description'),
                    data: "description"
                },
            ]
        })
    );

    $('#NewCategoryButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

});
$(function () {

    var nameInput = $('#Category_Name');

    var slugInput = $('#Category_UrlSlug');

    if (nameInput.length && slugInput.length) {
        nameInput.on('input keyup', function () {
            var nameValue = $(this).val();
            var slugValue = generateSlug(nameValue);
            slugInput.val(slugValue);
        });

        if (nameInput.val()) {
            slugInput.val(generateSlug(nameInput.val()));
        }
    }

    function generateSlug(text) {
        if (!text) {
            return "";
        }

        text = text.replace(/Đ/g, 'D');
        text = text.replace(/đ/g, 'd');


        text = text.normalize('NFD').replace(/[\u0300-\u036f]/g, '');


        text = text.replace(/\s+/g, '-');

        text = text.replace(/[^\w-]+/g, '');

        text = text.toLowerCase();

        text = text.replace(/-+/g, '-');
        text = text.replace(/^-+|-+$/g, '');

        return text;
    }
});