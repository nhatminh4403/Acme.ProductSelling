using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.PaymentGateway.VnPay.Services;
using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Controllers
{
    [Route("/thanh-toan")]
    [Authorize]
    public class PaymentReturnController : AbpController
    {
        //private readonly IOrderAppService _orderAppService;
        private readonly IOrderPublicAppService _orderPublicAppService;
        private readonly ILogger<PaymentReturnController> _logger;
        private readonly IVnPayService _vnPayService;
        public PaymentReturnController(
            //IOrderAppService orderAppService,
            ILogger<PaymentReturnController> logger,
            IVnPayService vnPayService,
            IOrderPublicAppService orderPublicAppService)
        {
            //_orderAppService = orderAppService;
            _logger = logger;
            _vnPayService = vnPayService;
            _orderPublicAppService = orderPublicAppService;
        }

        [HttpGet("payment-failed")]
        public IActionResult PaymentFailed()
        {
            var model = new PaymentResultViewModel();
            return View(model);
        }
        [HttpGet("vnpay-return")]
        [Authorize]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);

                if (!response.Success)
                {
                    _logger.LogWarning("VNPay invalid signature. Code: {Code}", response.VnPayResponseCode);
                    return View("PaymentFailed", new PaymentResultViewModel
                    {
                        Message = L["PaymentFailedPleaseTryAgain"],
                        RedirectUrl = Url.Page("/Orders/Index")
                    });
                }

                if (!Guid.TryParse(response.OrderId, out var orderId))
                    return View("PaymentFailed", new PaymentResultViewModel
                    {
                        Message = "Mã đơn hàng không hợp lệ.",
                        RedirectUrl = Url.Page("/Orders/Index")
                    });

                var order = await _orderPublicAppService.GetAsync(orderId);

                if (response.VnPayResponseCode == "00")
                {
                    return View("PaymentSuccess", new PaymentResultViewModel
                    {
                        IsSuccess = true,
                        Message = L["PaymentSuccessfulThankYou"],
                        RedirectUrl = Url.Page("/Orders/OrderConfirmation",
                            new { orderId = order.Id, orderNumber = order.OrderNumber })
                    });
                }

                return View("PaymentFailed", new PaymentResultViewModel
                {
                    Message = L["PaymentFailedPleaseTryAgain"],
                    RedirectUrl = Url.Page("/Orders/PaymentFailed",
                        new { orderId = order.Id, errorCode = response.VnPayResponseCode })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay return");
                return View("PaymentFailed", new PaymentResultViewModel
                {
                    Message = L["AnErrorOccurred"],
                    RedirectUrl = Url.Page("/Orders/Index")
                });
            }
        }

        [HttpGet("momo-callback")]
        [Authorize]
        public async Task<IActionResult> MoMoCallback()
        {
            try
            {
                var resultCode = int.Parse(Request.Query["resultCode"].ToString());
                var orderId = Request.Query["orderId"].ToString();
                long.TryParse(Request.Query["transId"].ToString(), out var transId);

                if (!Guid.TryParse(orderId, out var orderGuid))
                    return View("PaymentFailed", new PaymentResultViewModel
                    {
                        Message = "Mã đơn hàng không hợp lệ.",
                        RedirectUrl = Url.Page("/Orders/OrderHistory")
                    });

                if (resultCode == 0)
                {
                    try
                    {
                        var order = await _orderPublicAppService.ConfirmMoMoOrderAsync(orderGuid, transId);
                        return View("PaymentSuccess", new PaymentResultViewModel
                        {
                            IsSuccess = true,
                            Message = L["PaymentSuccessfulThankYou"],
                            RedirectUrl = Url.Page("/Orders/OrderConfirmation",
                                new { orderId = order.Id, orderNumber = order.OrderNumber })
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to confirm MoMo order {OrderId}", orderId);
                        // IPN will handle it — still show success
                        return View("PaymentSuccess", new PaymentResultViewModel
                        {
                            IsSuccess = true,
                            Message = "Thanh toán thành công. Đơn hàng đang được xử lý.",
                            RedirectUrl = Url.Page("/Orders/OrderHistory")
                        });
                    }
                }

                return View("PaymentFailed", new PaymentResultViewModel
                {
                    Message = $"Giao dịch thất bại. Vui lòng thử lại.",
                    RedirectUrl = Url.Page("/Orders/PaymentFailed",
                        new { orderId = orderGuid, errorCode = resultCode })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MoMo callback");
                return View("PaymentFailed", new PaymentResultViewModel
                {
                    Message = L["AnErrorOccurred"],
                    RedirectUrl = Url.Page("/Orders/OrderHistory")
                });
            }
        }

        [HttpGet("paypal-success")]
        [Authorize]
        public async Task<IActionResult> PayPalSuccess([FromQuery] string token, [FromQuery] string PayerID, [FromQuery] Guid orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || orderId == Guid.Empty)
                    return View("PaymentFailed", new PaymentResultViewModel
                    {
                        Message = L["AnErrorOccurredWhileProcessingPayment"],
                        RedirectUrl = Url.Page("/Orders/OrderHistory")
                    });

                // ConfirmPayPalOrderAsync handles ExecutePaymentAsync internally — don't call it separately
                var order = await _orderPublicAppService.ConfirmPayPalOrderAsync(orderId, token);

                return View("PaymentSuccess", new PaymentResultViewModel
                {
                    IsSuccess = true,
                    Message = L["PaymentSuccessfulThankYou"],
                    RedirectUrl = Url.Page("/Orders/OrderConfirmation",
                        new { orderId = order.Id, orderNumber = order.OrderNumber })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayPal success for orderId: {orderId}", orderId);
                return View("PaymentFailed", new PaymentResultViewModel
                {
                    Message = L["AnErrorOccurredWhileProcessingPayment"],
                    RedirectUrl = Url.Page("/Orders/OrderDetail", new { id = orderId })
                });
            }
        }

        [HttpGet("paypal-cancel")]
        [Authorize]
        public async Task<IActionResult> PayPalCancel([FromQuery] Guid orderId)
        {
            _logger.LogInformation("User cancelled PayPal payment. OrderId: {OrderId}", orderId);

            return View("PaymentFailed", new PaymentResultViewModel
            {
                Message = L["PaymentCancelledByUser"],
                RedirectUrl = orderId != Guid.Empty
                    ? Url.Page("/Orders/OrderDetail", new { id = orderId })
                    : Url.Page("/Orders/OrderHistory")
            });
        }
    }
}