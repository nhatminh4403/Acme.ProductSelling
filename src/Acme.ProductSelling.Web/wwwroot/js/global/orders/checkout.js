$(function () {
    var $paymentRadios = $('.payment-method-option input[type="radio"]');

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

    $paymentRadios.on('change', function () {
        updateSelectedState();
    });

    updateSelectedState();
});