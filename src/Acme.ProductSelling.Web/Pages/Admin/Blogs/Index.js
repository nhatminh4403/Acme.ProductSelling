$(function () {
    var categoryService = acme.productSelling.blogs.blog;
    var l = abp.localization.getResource('ProductSelling');

    var dataTable = $('#BlogsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(categoryService.getList),
            columnDefs: [
                {
                    title: l('Blogs:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Blogs.Edit') || abp.auth.isGranted('ProductSelling.Blogs.Delete'), // Hiện cột này nếu có quyền Edit hoặc Delete
                    rowAction: {
                        items: [
                            {
                                text: l('Blog:Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Blogs.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Blog:Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Blogs.Delete'),
                                confirmMessage: function (data) {
                                    return l('Blog:BlogDeletionConfirmationMessage', data.record.name);
                                },
                                action: function (data) {
                                    categoryService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('Blog:SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Blog:MainImage'),
                    data: "mainImageUrl",
                    render: function (data) {
                        return data ? '<img src="' + data + '" alt="Main Image" style="max-height: 100px; max-width: 100px;" />' : '';
                    }


                },
                {
                    title: l('Blog:Title'),
                    data: "title"
                },
                {
                    title: l('Blog:Content'),
                    data: "content"
                },
                {
                    title: l('Blog:Author'),
                    data: "author"
                },
                
                {
                    title: l('Blog:PublishedDate'),
                    data: "publishedDate",
                    render: function (data) {
                        return data ? moment(data).format('YYYY-MM-DD') : '';
                    }
                }

            ]
        })
    );

});