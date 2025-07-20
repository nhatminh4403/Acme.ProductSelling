$(function () {
    // Tìm các radio button của phương thức thanh toán
    var $paymentRadios = $('.payment-method-option input[type="radio"]');

    // Hàm để cập nhật trạng thái 'selected'
    function updateSelectedState() {
        $paymentRadios.each(function () {
            var $radio = $(this);
            var $label = $radio.closest('.payment-method-option');

            if ($radio.is(':checked')) {
                $label.addClass('selected');
            } else {
                $label.removeClass('selected');
            }
        });
    }

    // Lắng nghe sự kiện thay đổi
    $paymentRadios.on('change', function () {
        updateSelectedState();
    });

    // Cập nhật trạng thái lần đầu khi trang tải
    updateSelectedState();
});