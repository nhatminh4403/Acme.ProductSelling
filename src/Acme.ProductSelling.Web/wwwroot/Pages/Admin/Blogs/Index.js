$(function () {
    var blogService = acme.productSelling.blogs.blog;
    var l = abp.localization.getResource('ProductSelling');
    var currentPath = window.location.pathname;

    // Normalize prefix to handle leading slash safely
    var prefix = currentPath.split('/')[1];
    var isBlogger = prefix.toLowerCase() === 'blogger';
    var currentUserId = abp.currentUser.id;


    var getFilterMethod = function (input) {
        if (isBlogger) {
            return blogService.getBlogByBlogger(input);
        } else {
            return blogService.getList(input);
        }
    };

    var dataTable = $('#BlogsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "desc"]], 
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(getFilterMethod),
            columnDefs: [
                {
                    title: l('Blogs:Actions'),
                    visible: true, // Visibility handled inside rowAction items
                    rowAction: {
                        items: [
                            {
                                text: l('Blog:Edit'),
                                // Only show if permission granted AND (Admin OR is the Author/Blogger)
                                visible: abp.auth.isGranted('ProductSelling.Blogs.Edit'),
                                action: function (data) {
                                    window.location.href = `/${prefix}/blogs/edit?id=${data.record.id}`;
                                }
                            },
                            {
                                text: l('Blog:Delete'),
                                visible: abp.auth.isGranted('ProductSelling.Blogs.Delete'),
                                confirmMessage: function (data) {
                                    return l('Blog:BlogDeletionConfirmationMessage', data.record.title);
                                },
                                action: function (data) {
                                    blogService.delete(data.record.id)
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
                        return data
                            ? `<img src="${data}" alt="Img" style="max-height: 50px; max-width: 50px; object-fit: cover;" class="rounded shadow-sm" />`
                            : '<span class="text-muted">-</span>';
                    }
                },
                {
                    title: l('Blog:Title'),
                    data: "title",
                    render: function (data, type, row) {
                        // Adjust 'details' to 'Edit' or a read page depending on your flow
                        // Assuming bloggers go to Edit, Admins might go to details
                        var url = `/${prefix}/blogs/edit?id=${row.id}`;
                        return `<a href="${url}" class="text-decoration-none fw-bold">${data}</a>`;
                    }
                },
                // Only show Author column if Admin (Blogger knows who they are)
                {
                    title: l('Blog:Author'),
                    data: "authorName", // Ensure your BlogDto has 'AuthorName' mapped
                    visible: !isBlogger,
                    defaultContent: ""
                },
                {
                    title: l('Blog:CreationTime'),
                    data: "creationTime",
                    render: function (data) {
                        return abp.utils.formatDate(data); // Use ABP util for cleaner consistent formatting
                    }
                }
            ]
        })
    );

    $('#NewBlogPostButton').click(function (e) {
        e.preventDefault();
        window.location.href = `/${prefix}/blogs/create`;
    });
}); 