$(function () {
    var cartService = acme.productSelling.carts.cart;
    var localizationResource = abp.localization.getResource('ProductSelling');

    $('body').on('click', '.add-to-cart-button', function (e) {
        e.preventDefault();
        var $button = $(this); // Lưu trữ đối tượng jQuery của nút
        console.log("abp.currentUser.isAuthenticated: " + abp.currentUser.isAuthenticated)
        if (!abp.currentUser.isAuthenticated) {
            var returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = abp.appPath + 'Account/Login?ReturnUrl=' + returnUrl;
            return;

        }

        var productId = $button.data('product-id');

        var quantityInputId = '#quantity-for-' + productId;
        var $quantityInput = $(quantityInputId);

        if ($quantityInput.length === 0) {
            $quantityInput = $button.closest('.product-item, .product-detail').find('input[type="number"]');
        }

        var quantity = 1;
        if ($quantityInput.length > 0) {
            quantity = parseInt($quantityInput.val() || '1');
        }

        if (!productId || quantity <= 0) {
            abp.notify.warn(localizationResource('InvalidProductOrQuantity') || 'Invalid product or quantity');
            return;
        }

        $button.prop('disabled', true).addClass('disabled');
        var originalButtonHtml = $button.html();
        $button.html('<i class="fas fa-spinner fa-spin me-1"></i>' + (localizationResource('AddingToCart') || 'Adding...'));

        // Log để debug


        try {
            if (typeof cartService.addItem !== 'function') {
                console.error("cartService.addItem is not a function", cartService);
                abp.notify.error("Cart service not available");
                $button.prop('disabled', false).removeClass('disabled');
                $button.html(originalButtonHtml);
                return;
            }

            cartService.addItem({
                productId: productId,
                quantity: quantity
            }, {
                success: function () {
                    abp.notify.success(localizationResource('ItemAddedToCart') || 'Item added to cart');
                    updateCartWidgetCount();
                },
                error: function (error) {
                    abp.notify.error(error.message || localizationResource('CouldNotAddItemToCart') || 'Could not add item to cart');
                    console.error("Add to cart error:", error);
                },
                complete: function () {
                    $button.prop('disabled', false).removeClass('disabled');
                    $button.html(originalButtonHtml);
                }
            });
        } catch (error) {
            console.error("Exception in add to cart:", error);
            abp.notify.error("Error adding item to cart");
            $button.prop('disabled', false).removeClass('disabled');
            $button.html(originalButtonHtml);
        }
    });


    function updateCartWidgetCount() {
        var $cartButton = $('a[href="/cart"]');
        var $countElement = $cartButton.find('.badge');

        if ($cartButton.length === 0) {
            console.warn("Cart button element not found");
            return;
        }
        if ($countElement.length === 0) {
            console.warn("Cart count badge element not found");
            // Create a badge element if it doesn't exist
            $countElement = $('<span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">0</span>');
            $cartButton.append($countElement);
        }


        // Chỉ gọi API nếu đã đăng nhập
        if (abp.currentUser.isAuthenticated) {
            try {
                // Kiểm tra xem có phương thức getItemCount không
                if (typeof cartService.getItemCount !== 'function') {
                    console.error("cartService.getItemCount is not a function", cartService);
                    return;
                }

                cartService.getItemCount({
                    success: function (count) {
                        console.log("Cart count updated:", count);
                        $countElement.text(count);
                        if (count > 0) {
                            $countElement.removeClass('d-none');
                        } else {
                            $countElement.addClass('d-none');
                        }
                    },
                    error: function (error) {
                        console.error("Error updating cart widget count:", error);
                        $countElement.addClass('d-none');
                    }
                });
            } catch (error) {
                console.error("Exception in update cart count:", error);
            }
        } else {
            $countElement.text('0').addClass('d-none');
        }
    }

    // Gọi hàm cập nhật widget lần đầu khi trang tải
    try {
        if (abp.currentUser.isAuthenticated) {
            console.log("Initializing cart widget...");
            updateCartWidgetCount();
        }
    } catch (error) {
        console.error("Error initializing cart widget:", error);
    }
});