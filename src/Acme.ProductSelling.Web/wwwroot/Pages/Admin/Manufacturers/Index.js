$(function () {
    var manufacturerService = acme.productSelling.manufacturers.manufacturer;
    var l = abp.localization.getResource('ProductSelling');

    var prefix = window.location.pathname.split('/')[1];

    var createModal = new abp.ModalManager({
        // Construct the URL using the dynamic prefix
        viewUrl: `/${prefix}/manufacturers/create-modal`
    });
    var editModal = new abp.ModalManager();

    var dataTable = $('#ManufacturersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(manufacturerService.getList),
            columnDefs: [
                {
                    title: l('Manufacturer:Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Manufacturers.Edit') || abp.auth.isGranted('ProductSelling.Manufacturers.Delete'), // Hiện cột này nếu có quyền Edit hoặc Delete
                    rowAction: {
                        items: [
                            {
                                text: l('Manufacturer:EditManufacturer'),
                                visible: abp.auth.isGranted('ProductSelling.Manufacturers.Edit'),
                                action: function (data) {
                                    editModal.open({
                                        url: `/${prefix}/manufacturers/edit-modal/${data.record.id}`
                                    });
                                }
                            },

                        ]
                    }
                },
                {
                    title: l('Manufacturer:Name'),
                    data: "name"
                },
                {
                    title: l('Manufacturer:Description'),
                    data: "description",
                    render: function (data) {
                        return truncateText(data, 50);
                    }
                },
                {
                    title: l('Manufacturer:ContactInfo'),
                    data: "contactInfo",
                },
                {
                    title: l('Manufacturer:Image'),
                    data: "manufacturerImage",
                    render: function (data) {
                        if (!data) return '';
                        return '<img src="' + data + '" alt="Image" style="max-height: 50px; max-width: 80px; object-fit: cover;" />';
                    },
                    orderable: false,
                    searchable: false,
                    width: "10%"
                }
            ]
        })
    );
    function truncateText(text, maxLength) {
        if (!text) return "";
        return text.length > maxLength ? text.substring(0, maxLength) + "..." : text;
    }
    $('#NewManufacturerButton').click(function (e) {
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
$(function () {

    var nameInput = $('#Manufacturer_Name');

    var slugInput = $('#Manufacturer_UrlSlug');

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