using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.PaymentGateway.VnPay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Controllers
{
    [Route("/thanh-toan")]
    public class PaymentReturnController : Controller
    {
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger<PaymentReturnController> _logger;
        private readonly IVnPayService _vnPayService;
        public PaymentReturnController(
            IOrderAppService orderAppService,
            ILogger<PaymentReturnController> logger,
            IVnPayService vnPayService)
        {
            _orderAppService = orderAppService;
            _logger = logger;
            _vnPayService = vnPayService;
        }

        [HttpGet("vnpay-return")]
        [Authorize]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {

                //var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();

                //var getTxnRef = Request.Query["vnp_TxnRef"].ToString();

                //var getOrderId = getTxnRef.Contains("_") ? getTxnRef.Split('_')[0] : getTxnRef;
                //var vnp_TxnRef = getTxnRef.Contains("_") ? getTxnRef.Split('_')[1] : getTxnRef; 

                //_logger.LogInformation(
                //    "User returned from VNPay. OrderId: {OrderId}, ResponseCode: {Code}",
                //    vnp_TxnRef, vnp_ResponseCode
                //);
                //if (string.IsNullOrEmpty(vnp_TxnRef) || !Guid.TryParse(vnp_TxnRef, out var orderId))
                //{
                //    return RedirectToPage("/Error", new { message = "Invalid order reference" });
                //}

                //var order = await _orderAppService.GetAsync(orderId);

                //if (order == null)
                //{
                //    return RedirectToPage("/Error", new { message = "Order not found" });
                //}

                var response = _vnPayService.PaymentExecute(Request.Query);

                _logger.LogInformation(
                     "User returned from VNPay. TxnRef: {TxnRef}, ResponseCode: {Code}, Success: {Success}",
                     Request.Query["vnp_TxnRef"], response.VnPayResponseCode, response.Success
                );

                if (!response.Success)
                {
                    // Có thể sai chữ ký hoặc code != 00
                    _logger.LogWarning("Thanh toán thất bại hoặc Chữ ký không hợp lệ. Mã lỗi: {Code}", response.VnPayResponseCode);

                    // Xử lý lấy OrderId để redirect đúng trang (nếu parse được)
                    Guid.TryParse(response.OrderId, out var failOrderId);

                    return RedirectToPage("/Orders/PaymentFailed", new
                    {
                        orderId = failOrderId,
                        errorCode = response.VnPayResponseCode
                    });
                }
                if (string.IsNullOrEmpty(response.OrderId) || !Guid.TryParse(response.OrderId, out var orderId))
                {
                    return RedirectToPage("/Error", new { message = "Invalid order ID format" });
                }

                var order = await _orderAppService.GetAsync(orderId);

                if (order == null)
                {
                    return RedirectToPage("/Error", new { message = "Order not found" });
                }

                decimal paidAmount = decimal.Parse(response.Amount);
                if (order.TotalAmount != paidAmount)
                {
                    _logger.LogCritical("Gian lận số tiền! Đơn hàng: {Total}, Thanh toán: {Paid}", order.TotalAmount, paidAmount);
                    return RedirectToPage("/Error", new { message = "Số tiền thanh toán không khớp" });
                }

                var vnp_ResponseCode = response.VnPayResponseCode;

                if (vnp_ResponseCode == "00")
                {
                    // Payment successful
                    return RedirectToPage("/Orders/OrderConfirmation", new
                    {
                        orderId = order.Id,
                        orderNumber = order.OrderNumber
                    });
                }
                else
                {
                    // Payment failed
                    return RedirectToPage("/Orders/PaymentFailed", new
                    {
                        orderId = order.Id,
                        errorCode = vnp_ResponseCode
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay return");
                return RedirectToPage("/Error");
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

                _logger.LogInformation(
                    "User returned from MoMo. OrderId: {OrderId}, ResultCode: {Code}",
                    orderId, resultCode
                );

                if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out var orderIdGuid))
                {
                    return RedirectToPage("/Error", new { message = "Invalid order reference" });
                }

                var order = await _orderAppService.GetAsync(orderIdGuid);

                if (order == null)
                {
                    return RedirectToPage("/Error", new { message = "Order not found" });
                }

                if (resultCode == 0)
                {
                    // Payment successful
                    return RedirectToPage("/Orders/OrderConfirmation", new
                    {
                        orderId = order.Id,
                        orderNumber = order.OrderNumber
                    });
                }
                else
                {
                    // Payment failed
                    return RedirectToPage("/Orders/PaymentFailed", new
                    {
                        orderId = order.Id,
                        errorCode = resultCode
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MoMo callback");
                return RedirectToPage("/Error");
            }
        }

        [HttpGet("paypal-success")]
        [Authorize]
        public async Task<IActionResult> PayPalSuccess([FromQuery] string paymentId, [FromQuery] string PayerID, [FromQuery] Guid orderId)
        {
            try
            {
                _logger.LogInformation(
                    "User returned from PayPal. OrderId: {OrderId}, PaymentId: {PaymentId}",
                    orderId, paymentId
                );

                if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
                {
                    return RedirectToPage("/Error", new { message = "Invalid PayPal response" });
                }

                // Confirm the PayPal order
                var order = await _orderAppService.ConfirmPayPalOrderAsync(orderId);

                if (order == null)
                {
                    return RedirectToPage("/Error", new { message = "Order not found" });
                }

                return RedirectToPage("/Orders/OrderConfirmation", new
                {
                    orderId = order.Id,
                    orderNumber = order.OrderNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayPal success callback");
                return RedirectToPage("/Error");
            }
        }

        [HttpGet("paypal-cancel")]
        [Authorize]
        public async Task<IActionResult> PayPalCancel([FromQuery] Guid orderId)
        {
            try
            {
                _logger.LogInformation("User cancelled PayPal payment. OrderId: {OrderId}", orderId);

                var order = await _orderAppService.GetAsync(orderId);

                if (order == null)
                {
                    return RedirectToPage("/Error", new { message = "Order not found" });
                }

                return RedirectToPage("/Orders/PaymentCancelled", new
                {
                    orderId = order.Id,
                    orderNumber = order.OrderNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayPal cancel callback");
                return RedirectToPage("/Error");
            }
        }
    }
}