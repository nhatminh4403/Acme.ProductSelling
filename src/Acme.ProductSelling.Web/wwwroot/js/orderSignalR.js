﻿(function ($) {
    // Lấy hàm dịch thuật từ ABP. 'ProductSelling' là tên Resource của bạn.
    var l = abp.localization.getResource('ProductSelling');

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/signalr-hubs/orders") // Endpoint đã cấu hình trong WebModule
        .withAutomaticReconnect() // Tự động kết nối lại
        .build();

    connection.on("ReceiveOrderStatusUpdate", function (orderId, newStatus, newStatusText) {
        console.log(`SignalR: Order [${orderId}] status updated to [${newStatus}]`);

        var staticRow = $(`#OrderHistoryTable tr[data-order-id='${orderId}']`);
        if (staticRow.length) {
            var statusCell = staticRow.find('.order-status-cell');
            var badgeClass = getStatusBadgeClass(newStatus);

            var newBadgeHtml = `<span class="badge ${badgeClass}">${newStatusText}</span>`;
            statusCell.html(newBadgeHtml);

            highlightRow(staticRow);
        }

        if ($.fn.DataTable.isDataTable('#OrdersTable')) {
            var ordersTable = $('#OrdersTable').DataTable();
            var rowNode = ordersTable.row('#' + orderId).node(); // Tìm dòng bằng rowId

            if (rowNode) {
                var rowData = ordersTable.row(rowNode).data();
                rowData.orderStatus = newStatus; // Cập nhật enum
                rowData.statusText = newStatusText; // Cập nhật text đã dịch

                ordersTable.row(rowNode).data(rowData).draw(false);

                highlightRow($(rowNode));
            }
        }
    });

    // 3. HÀM HELPER
    // Hàm lấy class màu cho badge
    function getStatusBadgeClass(statusString) {
        switch (statusString) {
            case 'Placed': return 'bg-info';
            case 'Pending': return 'bg-light text-dark';
            case 'Confirmed': return 'bg-primary';
            case 'Processing': return 'bg-warning';
            case 'Shipped': return 'bg-success';
            case 'Delivered': return 'bg-dark';
            case 'Cancelled': return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    // Hàm thêm/xóa hiệu ứng highlight
    function highlightRow(rowElement) {
        rowElement.addClass('status-updated-highlight');
        setTimeout(function () {
            rowElement.removeClass('status-updated-highlight');
        }, 2500); // Giữ highlight trong 2.5 giây
    }

    // 4. BẮT ĐẦU KẾT NỐI
    function startConnection() {
        connection.start()
            .then(() => console.log("Order SignalR Hub Connected."))
            .catch((err) => {
                console.error("SignalR Connection Error: ", err.toString());
                setTimeout(startConnection, 5000); // Thử kết nối lại sau 5 giây nếu thất bại
            });
    }

    startConnection();

})(jQuery);