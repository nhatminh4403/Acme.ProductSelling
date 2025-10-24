using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Controllers
{
    [Route("thanh-toan")]
    public class PaymentReturnController : Controller
    {
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger<PaymentReturnController> _logger;

        public PaymentReturnController(
            IOrderAppService orderAppService,
            ILogger<PaymentReturnController> logger)
        {
            _orderAppService = orderAppService;
            _logger = logger;
        }

        /// <summary>
        /// VNPay return URL - where user is redirected after payment
        /// This is DIFFERENT from IPN
        /// </summary>
        [HttpGet("vnpay-return")]
        [Authorize]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {
                var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
                var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();

                _logger.LogInformation(
                    "User returned from VNPay. OrderId: {OrderId}, ResponseCode: {Code}",
                    vnp_TxnRef, vnp_ResponseCode
                );

                if (string.IsNullOrEmpty(vnp_TxnRef) || !Guid.TryParse(vnp_TxnRef, out var orderId))
                {
                    return RedirectToPage("/Error", new { message = "Invalid order reference" });
                }

                var order = await _orderAppService.GetAsync(orderId);

                if (order == null)
                {
                    return RedirectToPage("/Error", new { message = "Order not found" });
                }

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

        /// <summary>
        /// MoMo return URL - where user is redirected after payment
        /// </summary>
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

        /// <summary>
        /// PayPal success callback
        /// </summary>
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

        /// <summary>
        /// PayPal cancel callback
        /// </summary>
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