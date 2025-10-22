$(function () {
    // 1. Get a reference to the ABP service proxy and localization resources
    var orderService = acme.productSelling.orders.order;
    var l = abp.localization.getResource('ProductSelling');

    // 2. Use event delegation to attach a click handler to the restore buttons
    // This is efficient because it adds only one listener to the table body
    $('#DeletedOrdersTable').on('click', '.restore-button', function (e) {
        e.preventDefault();

        // Get the Order ID from the button's data-id attribute
        var orderId = $(this).data('id');

        // 3. Show a confirmation dialog before proceeding
        abp.message.confirm(
            l('Order:RestoreConfirmation'), // Your localization string, e.g., "Are you sure you want to restore this order?"
            function (isConfirmed) {
                if (isConfirmed) {
                    // 4. If confirmed, call the backend service via AJAX
                    orderService.restoreOrder(orderId)
                        .then(function () {
                            // 5. On success, show a notification
                            abp.notify.success(l('Order:RestoredSuccessfully'));

                            // Remove the entire row from the table with a fade-out effect
                            $('tr[data-row-id="' + orderId + '"]').fadeOut(500, function () {
                                $(this).remove();
                            });
                        })
                        .catch(function (error) {
                            // Let ABP's default error handler show the error message from the backend
                            // No extra code is needed here unless you want custom error logic.
                        });
                }
            }
        );
    });
});