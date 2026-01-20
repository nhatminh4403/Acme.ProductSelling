using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Services;
using Acme.ProductSelling.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Payments
{
    public class MoMoCallbackModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true), FromQuery]
        public string partnerCode { get; set; }

        [FromQuery, BindProperty(SupportsGet = true)]
        public string orderId { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string requestId { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public long amount { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string orderInfo { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string orderType { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public long transId { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public int resultCode { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string message { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string payType { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string extraData { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string signature { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public long responseTime { get; set; }

        public bool IsSuccess { get; private set; }

        private readonly IMoMoService _moMoService;
        public string OutputMessage { get; set; }
        public Guid OrderGuid { get; private set; }
        private readonly IOrderAppService _orderAppService;
        public MoMoCallbackModel(IMoMoService moMoService, IOrderAppService orderAppService)
        {
            _moMoService = moMoService;
            _orderAppService = orderAppService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Logger.LogInformation(
                    "[MoMo Callback] Received - OrderId: {OrderId}, ResultCode: {ResultCode}, TransId: {TransId}",
                    orderId, resultCode, transId
                );

                // Validate orderId
                if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out var orderGuid))
                {
                    Logger.LogError("[MoMo Callback] Invalid OrderId: {OrderId}", orderId);
                    OutputMessage = "Mã đơn hàng không hợp lệ.";
                    IsSuccess = false;
                    return Page();
                }

                OrderGuid = orderGuid;

                // ✅ FIX: Update order status immediately in the callback
                if (resultCode == 0)
                {
                    Logger.LogInformation(
                        "[MoMo Callback] Payment successful - Confirming order {OrderId}",
                        orderId
                    );

                    try
                    {
                        // Get the order
                        var order = await _orderAppService.GetAsync(orderGuid);

                        // If still pending, mark as paid
                        if (order.PaymentStatus == PaymentStatus.Pending)
                        {
                            await _orderAppService.ConfirmPayPalOrderAsync(orderGuid);

                            Logger.LogInformation(
                                "[MoMo Callback] Order confirmed successfully - OrderId: {OrderId}",
                                orderId
                            );
                        }

                        IsSuccess = true;
                        OutputMessage = "Giao dịch đã được thực hiện thành công. Chúng tôi đang xử lý đơn hàng của bạn.";
                        Alerts.Success(OutputMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex,
                            "[MoMo Callback] Failed to confirm order {OrderId}",
                            orderId
                        );

                        // Still show success to user (IPN will handle it)
                        IsSuccess = true;
                        OutputMessage = "Thanh toán thành công. Đơn hàng của bạn đang được xử lý.";
                        Alerts.Success(OutputMessage);
                    }
                }
                else
                {
                    Logger.LogWarning(
                        "[MoMo Callback] Payment failed - OrderId: {OrderId}, ResultCode: {ResultCode}, Message: {Message}",
                        orderId, resultCode, message
                    );

                    IsSuccess = false;
                    OutputMessage = $"Giao dịch không thành công: {message}. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
                    Alerts.Warning(OutputMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[MoMo Callback] Unexpected error");
                IsSuccess = false;
                OutputMessage = "Đã có lỗi xảy ra. Vui lòng liên hệ bộ phận hỗ trợ.";
                Alerts.Danger(OutputMessage);
            }

            return Page();
        }

    }
}
