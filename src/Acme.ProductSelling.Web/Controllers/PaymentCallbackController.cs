using Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Models;
using Acme.ProductSelling.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentCallbackController : ControllerBase
    {
        private readonly IPaymentCallbackAppService _callbackAppService;
        private readonly ILogger<PaymentCallbackController> _logger;

        public PaymentCallbackController(
            IPaymentCallbackAppService callbackAppService,
            ILogger<PaymentCallbackController> logger)
        {
            _callbackAppService = callbackAppService;
            _logger = logger;
        }


        [HttpGet("vnpay-ipn")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> VnPayIpn()
        {
            var correlationId = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "[VNPay IPN] Received callback. CorrelationId: {CorrelationId}, QueryParams: {ParamCount}",
                    correlationId, Request.Query.Count
                );

                foreach (var param in Request.Query)
                {
                    if (!param.Key.Contains("SecureHash", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogDebug(
                            "[VNPay IPN] {Key} = {Value}",
                            param.Key, param.Value
                        );
                    }
                }

                // Process the IPN
                var result = await _callbackAppService.ProcessVnPayIpnAsync(Request.Query);

                if (result == null)
                {
                    _logger.LogError(
                        "[VNPay IPN] Callback service returned null. CorrelationId: {CorrelationId}",
                        correlationId
                    );
                    return CreateVnPayErrorResponse("99", "System error");
                }

                _logger.LogInformation(
                    "[VNPay IPN] Processing complete. CorrelationId: {CorrelationId}, ResponseCode: {Code}, Success: {Success}",
                    correlationId, result.VnPayResponseCode, result.Success
                );

                if (result.Success && result.VnPayResponseCode == "00")
                {
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                else if (result.VnPayResponseCode == "02")
                {
                    // Order already confirmed
                    return Ok(new { RspCode = "02", Message = "Order already confirmed" });
                }
                else if (result.VnPayResponseCode == "01")
                {
                    // Order not found
                    return Ok(new { RspCode = "01", Message = "Order not found" });
                }
                else if (result.VnPayResponseCode == "97")
                {
                    // Invalid signature
                    return Ok(new { RspCode = "97", Message = "Invalid Signature" });
                }
                else
                {
                    // Other errors
                    return Ok(new { RspCode = result.VnPayResponseCode, Message = result.OrderDescription ?? "Unknown error" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[VNPay IPN] Unexpected error. CorrelationId: {CorrelationId}, Message: {Message}",
                    correlationId, ex.Message
                );

                return CreateVnPayErrorResponse("99", "System error");
            }
        }


        [HttpPost("momo-ipn")] 
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MoMoIpn([FromBody] MomoIPNRequest request)
        {
            var correlationId = Guid.NewGuid().ToString();

            try
            {
                // IMPROVEMENT: Validate request
                if (request == null)
                {
                    _logger.LogWarning(
                        "[MoMo IPN] Null request received. CorrelationId: {CorrelationId}",
                        correlationId
                    );
                    return BadRequest(new { message = "Invalid request" });
                }

                _logger.LogInformation(
                    "[MoMo IPN] Received callback. CorrelationId: {CorrelationId}, OrderId: {OrderId}, TransId: {TransId}, ResultCode: {ResultCode}",
                    correlationId, request.orderId, request.transId, request.resultCode
                );

                // IMPROVEMENT: Log request data (excluding signature)
                _logger.LogDebug(
                    "[MoMo IPN] Request details - PartnerCode: {PartnerCode}, RequestId: {RequestId}, Amount: {Amount}, OrderInfo: {OrderInfo}",
                    request.partnerCode, request.requestId, request.amount, request.orderInfo
                );

                // Process the IPN
                await _callbackAppService.ProcessMoMoIpnAsync(request);

                _logger.LogInformation(
                    "[MoMo IPN] Processing complete. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                    correlationId, request.orderId
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[MoMo IPN] Error processing callback. CorrelationId: {CorrelationId}, OrderId: {OrderId}, Message: {Message}",
                    correlationId, request?.orderId, ex.Message
                );

                return NoContent();
            }
        }

        [HttpPost("paypal-ipn")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> PayPalIpn()
        {
            var correlationId = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "[PayPal IPN] Received callback. CorrelationId: {CorrelationId}",
                    correlationId
                );

                _logger.LogInformation(
                    "[PayPal IPN] Processing complete. CorrelationId: {CorrelationId}",
                    correlationId
                );

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[PayPal IPN] Error processing callback. CorrelationId: {CorrelationId}",
                    correlationId
                );

                return Ok(); // Return 200 to prevent retry
            }
        }

        private IActionResult CreateVnPayErrorResponse(string code, string message)
        {
            return Ok(new
            {
                RspCode = code,
                Message = message
            });
        }
    }
}