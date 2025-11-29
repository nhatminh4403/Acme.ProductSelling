$(function () {
    // Handle quantity increase
    $('.btn-increase').on('click', function (e) {
        e.preventDefault();
        var itemId = $(this).data('item-id');
        var input = $(`.quantity-input[data-item-id="${itemId}"]`);
        var currentVal = parseInt(input.val()) || 1;
        var maxVal = parseInt(input.attr('max')) || 100;

        if (currentVal < maxVal) {
            input.val(currentVal + 1);
            updateCartItem(itemId, currentVal + 1);
        }
    });

    // Handle quantity decrease
    $('.btn-decrease').on('click', function (e) {
        e.preventDefault();
        var itemId = $(this).data('item-id');
        var input = $(`.quantity-input[data-item-id="${itemId}"]`);
        var currentVal = parseInt(input.val()) || 1;
        var minVal = parseInt(input.attr('min')) || 1;

        if (currentVal > minVal) {
            input.val(currentVal - 1);
            updateCartItem(itemId, currentVal - 1);
        }
    });

    // Handle manual quantity input
    $('.quantity-input').on('change', function () {
        var itemId = $(this).data('item-id');
        var newQuantity = parseInt($(this).val());
        var minVal = parseInt($(this).attr('min')) || 1;
        var maxVal = parseInt($(this).attr('max')) || 100;

        // Validate quantity
        if (isNaN(newQuantity) || newQuantity < minVal) {
            $(this).val(minVal);
            newQuantity = minVal;
        } else if (newQuantity > maxVal) {
            $(this).val(maxVal);
            newQuantity = maxVal;
        }

        updateCartItem(itemId, newQuantity);
    });

    // Handle remove item
    $('.btn-remove-item').on('click', function (e) {
        e.preventDefault();
        var itemId = $(this).data('item-id');
        var productName = $(this).data('product-name');

        // Show confirmation dialog
        abp.message.confirm(
            'Are you sure you want to remove "' + productName + '" from your cart?',
            'Remove Item',
            function (confirmed) {
                if (confirmed) {
                    removeCartItem(itemId);
                }
            }
        );
    });

    // Handle clear cart
    $('#btnClearCart').on('click', function (e) {
        e.preventDefault();

        // Show confirmation dialog
        abp.message.confirm(
            'Are you sure you want to clear your entire cart?',
            'Clear Cart',
            function (confirmed) {
                if (confirmed) {
                    clearCart();
                }
            }
        );
    });

    // Debounce timer
    var updateTimer;

    // Function to update cart item quantity
    function updateCartItem(cartItemId, quantity) {
        // Clear previous timer
        clearTimeout(updateTimer);

        // Set new timer to avoid too many requests
        updateTimer = setTimeout(function () {
            // Create form data
            var formData = new FormData();
            formData.append('cartItemId', cartItemId);
            formData.append('quantity', quantity);

            // Get anti-forgery token
            var token = $('input[name="__RequestVerificationToken"]').val();

            // Show loading state
            var input = $(`.quantity-input[data-item-id="${cartItemId}"]`);
            input.prop('disabled', true);

            // Send AJAX request
            $.ajax({
                url: '?handler=UpdateItem',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                headers: {
                    'RequestVerificationToken': token
                },
                success: function (response) {
                    // Reload page to show updated totals
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error('Error updating cart:', error);
                    abp.message.error('Failed to update cart item. Please try again.');
                    input.prop('disabled', false);
                },
                complete: function () {
                    input.prop('disabled', false);
                }
            });
        }, 800); // Wait 800ms after user stops typing
    }

    // Function to remove cart item
    function removeCartItem(cartItemId) {
        // Create form data
        var formData = new FormData();
        formData.append('cartItemId', cartItemId);

        // Get anti-forgery token
        var token = $('input[name="__RequestVerificationToken"]').val();

        // Show loading
        abp.ui.setBusy();

        // Send AJAX request
        $.ajax({
            url: '?handler=RemoveItem',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                abp.notify.success('Item removed from cart');
                location.reload();
            },
            error: function (xhr, status, error) {
                console.error('Error removing item:', error);
                abp.message.error('Failed to remove item. Please try again.');
            },
            complete: function () {
                abp.ui.clearBusy();
            }
        });
    }

    // Function to clear entire cart
    function clearCart() {
        // Get anti-forgery token
        var token = $('input[name="__RequestVerificationToken"]').val();

        // Show loading
        abp.ui.setBusy();

        // Send AJAX request
        $.ajax({
            url: '?handler=ClearCart',
            type: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                abp.notify.success('Cart cleared successfully');
                location.reload();
            },
            error: function (xhr, status, error) {
                console.error('Error clearing cart:', error);
                abp.message.error('Failed to clear cart. Please try again.');
            },
            complete: function () {
                abp.ui.clearBusy();
            }
        });
    }
});