$(function () {
    var orderService = acme.productSelling.orders.order;
    var l = abp.localization.getResource('ProductSelling');
    var orderId = '@Model.Order.Id';

    $('#btnShipOrder').click(function () {
        abp.message.confirm(
            l('Order:AreYouSureYouWantToShipOrder', '@Model.Order.OrderNumber'),
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

    $('#btnDeliverOrder').click(function () {
        abp.message.confirm(
            l('Order:AreYouSureYouWantToDeliverOrder', '@Model.Order.OrderNumber'),
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
});