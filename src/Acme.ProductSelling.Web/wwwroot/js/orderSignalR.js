(function ($) {
    var l = abp.localization.getResource('ProductSelling');

    // 1. KẾT NỐI HUB
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/signalr-hubs/orders", {
            accessTokenFactory: () => {
                const token = abp.auth.getToken();
                console.log("Token gửi lên SignalR:", token); // Check token ở đây
                return token;
            }
        })
        .withAutomaticReconnect()
        .build();

    // 2. LẮNG NGHE SỰ KIỆN
    connection.on("ReceiveOrderStatusUpdate", function (orderId, newStatus, newStatusText) {
        // ----- Xử lý cho trang Lịch sử đơn hàng (bảng tĩnh) -----
        var historyRow = $(`#OrderHistoryTable tr[data-order-id='${orderId}']`);
        if (historyRow.length) {
            updateRowStatus(historyRow, newStatus, newStatusText);
        }

        // ----- Xử lý cho trang Chi tiết đơn hàng -----
        // Giả sử trang chi tiết có một thẻ body hoặc container chứa data-order-id
        var pageOrderId = $('#orderDetailContainer').data('order-id');
        if (pageOrderId && pageOrderId === orderId) {
            updateDetailPage(newStatus, newStatusText);
        }

        // ----- Xử lý cho bảng DataTable của Admin (nếu có) -----
        if ($.fn.DataTable.isDataTable('#OrdersTable')) {
            var adminTable = $('#OrdersTable').DataTable();
            // Cần một cách để tìm dòng, ví dụ bằng ID hoặc class
            var adminRow = adminTable.row(`#order-row-${orderId}`);
            if (adminRow.length) {
                // Logic cập nhật DataTable...
                // adminTable.cell(adminRow, columnIndex).data(newStatusText).draw(false);
                // highlightRow(adminRow.node());
            }
        }
    });

    // 3. CÁC HÀM HELPER

    function updateRowStatus(rowElement, newStatus, newStatusText) {
        // Cập nhật badge trạng thái
        var statusCell = rowElement.find('.order-status-cell');
        var badgeClass = getStatusBadgeClass(newStatus);
        var newBadgeHtml = `<span class="badge ${badgeClass}">${newStatusText}</span>`;
        statusCell.html(newBadgeHtml);

        // Cập nhật nút hủy
        var cancelButtonForm = rowElement.find('.cancel-order-form');
        if (newStatus !== 'Placed' && cancelButtonForm.length) {
            cancelButtonForm.remove(); // Xóa nút nếu không còn ở trạng thái "Placed"
        }

        highlightRow(rowElement);
    }

    function updateDetailPage(newStatus, newStatusText) {
        // Cập nhật badge trạng thái
        var statusBadge = $('#orderStatusBadge');
        if (statusBadge.length) {
            var badgeClass = getStatusBadgeClass(newStatus);
            statusBadge.removeClass().addClass('badge ' + badgeClass).text(newStatusText);
            highlightRow(statusBadge.parent());
        }

        // Ẩn/hiện container của nút hủy
        var cancelContainer = $('#cancelOrderContainer');
        if (cancelContainer.length) {
            if (newStatus === 'Placed') {
                cancelContainer.show();
            } else {
                cancelContainer.hide();
            }
        }
    }

    function getStatusBadgeClass(statusString) {
        // Case-insensitive comparison
        switch (statusString.toLowerCase()) {
            case 'placed': return 'bg-info';
            case 'pendingpayment': return 'bg-warning text-dark';
            case 'pending': return 'bg-secondary';
            case 'confirmed': return 'bg-primary';
            case 'processing': return 'bg-warning text-dark';
            case 'shipped': return 'bg-success';
            case 'completed':
            case 'delivered': return 'bg-dark';
            case 'cancelled': return 'bg-danger';
            default: return 'bg-light text-dark';
        }
    }

    function highlightRow(rowElement) {
        rowElement.addClass('status-updated-highlight');
        setTimeout(() => rowElement.removeClass('status-updated-highlight'), 2500);
    }

    // 4. BẮT ĐẦU KẾT NỐI
    function startConnection() {
        connection.start()
            .then(() => console.log("SignalR Connected to OrderHub."))
            .catch((err) => {
                console.error("SignalR Connection Error: ", err.toString());
                setTimeout(startConnection, 5000); // Thử lại sau 5 giây
            });
    }

    startConnection();

})(jQuery);