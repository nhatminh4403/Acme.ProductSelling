$(function () {
    var orderService = acme.productSelling.orders.order;
    var l = abp.localization.getResource('ProductSelling');

    // Get order ID from page
    var orderId = $('#orderIdValue').val();

    if (!orderId) {
        // Try to get from data attribute or window variable
        orderId = $('body').data('order-id') || window.orderId;
    }

    // Ship Order button
    $('#btnShipOrder').click(function () {
        var orderNumber = $(this).data('order-number') || window.orderNumber;

        abp.message.confirm(
            l('Order:AreYouSureYouWantToShipOrder', orderNumber),
            l('Confirmation')
        ).then(function (confirmed) {
            if (confirmed) {
                orderService.shipOrder(orderId)
                    .then(function () {
                        abp.notify.success(l('Order:OrderShippedSuccessfully'));
                        location.reload();
                    })
                    .catch(function (error) {
                        abp.notify.error(error.message || l('Order:ShipOrderFailed'));
                    });
            }
        });
    });

    // Deliver Order button
    $('#btnDeliverOrder').click(function () {
        var orderNumber = $(this).data('order-number') || window.orderNumber;

        abp.message.confirm(
            l('Order:AreYouSureYouWantToDeliverOrder', orderNumber),
            l('Confirmation')
        ).then(function (confirmed) {
            if (confirmed) {
                orderService.deliverOrder(orderId)
                    .then(function () {
                        abp.notify.success(l('Order:OrderDeliveredSuccessfully'));
                        location.reload();
                    })
                    .catch(function (error) {
                        abp.notify.error(error.message || l('Order:DeliverOrderFailed'));
                    });
            }
        });
    });

    // Real-time order updates via SignalR (if implemented)
    if (window.orderHub) {
        window.orderHub.on('ReceiveOrderStatusUpdate', function (orderId, status, statusText, paymentStatus, paymentStatusText) {
            if (orderId === window.currentOrderId) {
                abp.notify.info(l('Order:OrderStatusUpdated'));
                // Optionally reload the page or update UI dynamically
                setTimeout(function () {
                    location.reload();
                }, 1500);
            }
        });
    }
});