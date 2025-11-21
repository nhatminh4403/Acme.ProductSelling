(function () {
    let itemIndex = 0;

    $(document).ready(function () {
        const $container = $('#orderItemsContainer');
        const $template = $('#orderItemTemplate');
        const $noItemsMessage = $('#noItemsMessage');
        const $addBtn = $('#addItemBtn');

        // Add first item on load
        addOrderItem();

        $addBtn.on('click', function () {
            addOrderItem();
        });

        function addOrderItem() {
            const $newItem = $($template.html());
            $newItem.attr('data-item-index', itemIndex);
            $newItem.html($newItem.html().replace(/INDEX/g, itemIndex));

            $container.append($newItem);
            $noItemsMessage.hide();

            // Attach event handlers
            $newItem.find('.product-select').on('change', updateLineTotal);
            $newItem.find('.quantity-input').on('input', updateLineTotal);
            $newItem.find('.remove-item-btn').on('click', function () {
                removeOrderItem($newItem);
            });

            itemIndex++;
        }

        function removeOrderItem($item) {
            $item.remove();
            if ($container.children().length === 0) {
                $noItemsMessage.show();
            }
            updateTotal();
        }

        function updateLineTotal() {
            const $row = $(this).closest('.order-item-row');
            const $productSelect = $row.find('.product-select');
            const $quantityInput = $row.find('.quantity-input');
            const $lineTotal = $row.find('.line-total');

            const selectedOption = $productSelect.find('option:selected');
            if (!selectedOption.val()) {
                $lineTotal.val('0 VND');
                updateTotal();
                return;
            }

            const priceText = selectedOption.data('price');
            const price = parseFloat(priceText.replace(/[^\d]/g, ''));
            const quantity = parseInt($quantityInput.val()) || 0;
            const total = price * quantity;

            $lineTotal.val(total.toLocaleString('vi-VN') + ' VND');
            updateTotal();
        }

        function updateTotal() {
            let grandTotal = 0;
            $('.line-total').each(function () {
                const value = $(this).val().replace(/[^\d]/g, '');
                grandTotal += parseFloat(value) || 0;
            });
            $('#totalAmount').text(grandTotal.toLocaleString('vi-VN') + ' VND');
        }
    });
})();