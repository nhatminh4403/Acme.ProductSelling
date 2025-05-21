$(function () { 
    var manufacturerService = acme.productSelling.manufacturers.manufacturer; 
    var l = abp.localization.getResource('ProductSelling'); 

    var createModal = new abp.ModalManager(abp.appPath + 'Manufacturers/CreateModal'); 
    var editModal = new abp.ModalManager(abp.appPath + 'Manufacturers/EditModal');  

    var dataTable = $('#ManufacturersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({ 
            serverSide: true, 
            paging: true,
            order: [[1, "asc"]], 
            searching: false, 
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(manufacturerService.getList), 
            columnDefs: [
                {
                    title: l('Actions'),
                    visible: abp.auth.isGranted('ProductSelling.Manufacturers.Edit') || abp.auth.isGranted('ProductSelling.Manufacturers.Delete'), // Hiện cột này nếu có quyền Edit hoặc Delete
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('ProductSelling.Manufacturers.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            
                        ]
                    }
                },
                {
                    title: l('Name'), 
                    data: "name"    
                },
                {
                    title: l('Description'), 
                    data: "description",
                    render: function (data) {
                        return truncateText(data, 50);
                    }
                },
                {
                    title: l('ContactInfo'),
                    data: "contactInfo",
                },
                {
                    title: l('Image'),
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
