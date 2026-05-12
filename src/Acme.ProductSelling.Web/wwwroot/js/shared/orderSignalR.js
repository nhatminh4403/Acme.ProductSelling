(function ($) {
    var l = abp.localization.getResource('ProductSelling');

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/signalr-hubs/orders", {
            accessTokenFactory: () => {
                const token = abp.auth.getToken();
                return token;
            }
        })
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveOrderStatusUpdate", function (orderId,
        newOrderStatus, newOrderStatusText,
        newPaymentStatus, newPaymentStatusText) {

        console.log(`[SignalR] Received update for Order ${orderId}: OrderStatus=${newOrderStatus}, PaymentStatus=${newPaymentStatus}`);

        var historyRow = $('#OrderHistoryTable tr').filter(function() {
            return $(this).data('order-id')?.toString().toLowerCase() === orderId.toString().toLowerCase();
        });

        if (historyRow.length) {
            console.log(`[SignalR] Updating history row for Order ${orderId}`);
            updateRowStatus(historyRow, newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText);
        }

        var detailContainer = $('#orderDetailContainer');
        var pageOrderId = detailContainer.data('order-id');

        if (pageOrderId && pageOrderId.toString().toLowerCase() === orderId.toString().toLowerCase()) {
            console.log(`[SignalR] Updating detail page for Order ${orderId}`);
            updateDetailPage(newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText);
        }

        if ($.fn.DataTable.isDataTable('#OrdersTable')) {
            var adminTable = $('#OrdersTable').DataTable();
            var adminRow = adminTable.row(`#order-row-${orderId.toLowerCase()}`);
            if (adminRow.any()) {
                console.log(`[SignalR] Order ${orderId} found in Admin Orders Table. Reloading table...`);
                adminTable.ajax.reload(null, false); 
            }
        }
    });

    function updateRowStatus(rowElement, newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText) {
        var orderStatusCell = rowElement.find('.order-status-cell');

        var orderBadgeClass = getStatusBadgeClass(newOrderStatus);
        var orderBadgeHtml = `<span class="badge ${orderBadgeClass}">${newOrderStatusText}</span>`;
        orderStatusCell.html(orderBadgeHtml);

        var paymentStatusCell = rowElement.find('.payment-status-cell');
        if (paymentStatusCell.length) {
            var paymentBadgeClass = getPaymentStatusBadgeClass(newPaymentStatus);
            var paymentBadgeHtml = `<span class="badge ${paymentBadgeClass}">${newPaymentStatusText}</span>`;
            paymentStatusCell.html(paymentBadgeHtml);
        }

        var cancelButtonForm = rowElement.find('.cancel-order-form');
        if ((newOrderStatus.toLowerCase() !== 'placed' || newPaymentStatus.toLowerCase() === 'paid') && cancelButtonForm.length) {
            cancelButtonForm.remove();
        }

        highlightRow(rowElement);
    }

    function updateDetailPage(newOrderStatus, newOrderStatusText, newPaymentStatus, newPaymentStatusText) {
        var statusBadge = $('#orderStatusBadge');
        if (statusBadge.length) {
            var badgeClass = getStatusBadgeClass(newOrderStatus);
            statusBadge.removeClass().addClass('badge ' + badgeClass).text(newOrderStatusText);
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
            case 'delivered': return 'bg-success';
            case 'cancelled': return 'bg-danger';
            default: return 'bg-light text-dark';
        }
    }

    function getPaymentStatusBadgeClass(status) {
        switch (status.toLowerCase()) {
            case 'unpaid': return 'bg-warning text-dark';
            case 'pendingondelivery':
            case 'pending': return 'bg-info text-dark';
            case 'paid': return 'bg-success';
            case 'failed': return 'bg-danger';
            case 'partiallyrefunded':
            case 'refunded': return 'bg-secondary';
            case 'cancelled': return 'bg-dark';
            default: return 'bg-light text-dark';
        }
    }

    function highlightRow(rowElement) {
        rowElement.addClass('status-updated-highlight');
        setTimeout(() => rowElement.removeClass('status-updated-highlight'), 2500);
    }

    function startConnection() {
        connection.start()
            .then(() => console.log("SignalR Connected to OrderHub."))
            .catch((err) => {
                console.error("SignalR Connection Error: ", err.toString());
                setTimeout(startConnection, 5000);
            });
    }

    startConnection();

})(jQuery);