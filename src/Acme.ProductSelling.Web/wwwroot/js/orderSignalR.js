(function ($) {
    var l = abp.localization.getResource('ProductSelling');

    // 1. KẾT NỐI HUB
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/signalr-hubs/orders", {
            accessTokenFactory: () => {
                const token = abp.auth.getToken();
                console.log("Token gửi lên SignalR:", token); 
                return token;
            }
        })
        .withAutomaticReconnect()
        .build();

    // 2. LẮNG NGHE SỰ KIỆN
    connection.on("ReceiveOrderStatusUpdate", function (orderId,
        newOrderStatus, newOrderStatusText,
        newPaymentStatus, newPaymentStatusText) {

        console.log(`Received update for Order ${orderId}: OrderStatus=${newOrderStatus}, PaymentStatus=${newPaymentStatus}`);

        var historyRow = $(`#OrderHistoryTable tr[data-order-id='${orderId}']`);
        if (historyRow.length) {
            updateRowStatus(historyRow, newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText);
        }

        var pageOrderId = $('#orderDetailContainer').data('order-id');

        if (pageOrderId && pageOrderId === orderId) {
            updateDetailPage(newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText);
        }
        if ($.fn.DataTable.isDataTable('#OrdersTable')) {
            var adminTable = $('#OrdersTable').DataTable();
            var adminRow = adminTable.row(`#order-row-${orderId}`);
            if (adminRow.length) {
                // Logic cập nhật DataTable...
                // adminTable.cell(adminRow, columnIndex).data(newStatusText).draw(false);
                // highlightRow(adminRow.node());
            }
        }
    });

    // 3. CÁC HÀM HELPER
    function updateRowStatus(rowElement, newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText) {
        var orderStatusCell = rowElement.find('.order-status-cell');
        var orderBadgeClass = getOrderStatusBadgeClass(newOrderStatus);
        var orderBadgeHtml = `<span class="badge ${orderBadgeClass}">${newOrderStatusText}</span>`;
        orderStatusCell.html(orderBadgeHtml);


        var paymentStatusCell = rowElement.find('.payment-status-cell');
        if (paymentStatusCell.length) {
            var paymentBadgeClass = getPaymentStatusBadgeClass(newPaymentStatus);
            var paymentBadgeHtml = `<span class="badge ${paymentBadgeClass}">${newPaymentStatusText}</span>`;
            paymentStatusCell.html(paymentBadgeHtml);
        }
        // Cập nhật nút hủy
        var cancelButtonForm = rowElement.find('.cancel-order-form');
        if ((newOrderStatus.toLowerCase() !== 'placed' || newPaymentStatus.toLowerCase() === 'paid') && cancelButtonForm.length) {
            cancelButtonForm.remove(); // Xóa nút nếu không còn ở trạng thái "Placed"
        }
        // --- LOGIC BỔ SUNG: Cập nhật các hành động khác cho Admin ---
        // Ví dụ, bạn có một nút "Xác nhận thanh toán COD"
/*        var confirmCodPaymentButton = rowElement.find('.confirm-cod-payment-btn');
        if (confirmCodPaymentButton.length) {
            if (newOrderStatus.toLowerCase() === 'shipped' && newPaymentStatus.toLowerCase() === 'pendingondelivery') {
                confirmCodPaymentButton.show();
            } else {
                confirmCodPaymentButton.hide();
            }
        }*/
        highlightRow(rowElement);
    }

    function updateDetailPage(newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText) {
        var statusBadge = $('#orderStatusBadge');
        if (statusBadge.length) {
            var badgeClass = getStatusBadgeClass(newStatus);
            statusBadge.removeClass().addClass('badge ' + badgeClass).text(newStatusText);
            highlightRow(statusBadge.parent());
        }
        var paymentStatusBadge = $('#paymentStatusBadge'); 
        if (paymentStatusBadge.length) {
            var paymentBadgeClass = getPaymentStatusBadgeClass(newPaymentStatus);
            paymentStatusBadge.removeClass().addClass('badge ' + paymentBadgeClass).text(newPaymentStatusText);
        }
        var cancelContainer = $('#cancelOrderContainer');
        if (cancelContainer.length) {
            if (newOrderStatus.toLowerCase() === 'placed' && newPaymentStatus.toLowerCase() !== 'paid') {
                cancelContainer.show();
            } else {
                cancelContainer.hide();
            }
        }
    }

    function getStatusBadgeClass(statusString) {
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
    function getPaymentStatusBadgeClass(status) {
        switch (status.toLowerCase()) {
            case 'unpaid':
                return 'bg-secondary';
            case 'pendingondelivery':
                return 'bg-info text-dark';
            case 'pending':
                return 'bg-warning text-dark';
            case 'paid':
                return 'bg-success';
            case 'failed':
                return 'bg-danger';
            case 'partiallyrefunded':
            case 'refunded':
                return 'bg-dark';
            default:
                return 'bg-light text-dark';
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