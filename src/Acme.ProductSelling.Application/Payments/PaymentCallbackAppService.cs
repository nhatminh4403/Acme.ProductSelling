using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Models;
using Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Services;
using Acme.ProductSelling.PaymentGateway.VnPay.Dtos;
using Acme.ProductSelling.PaymentGateway.VnPay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Payments
{
    public class PaymentCallbackAppService : ApplicationService, IPaymentCallbackAppService
    {
        private readonly IVnPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderNotificationService _orderNotificationService;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly ILogger<PaymentCallbackAppService> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IOrderHistoryAppService _orderHistoryService;
        public PaymentCallbackAppService(
            IVnPayService vnPayService,
            IOrderRepository orderRepository,
            IOrderNotificationService orderNotificationService,
            ILogger<PaymentCallbackAppService> logger,
            IDistributedEventBus distributedEventBus, IMoMoService moMoService, IUnitOfWorkManager unitOfWorkManager, IOrderHistoryAppService orderHistoryService)
        {
            _vnPayService = vnPayService;
            _orderRepository = orderRepository;
            _orderNotificationService = orderNotificationService;
            _distributedEventBus = distributedEventBus;
            _logger = logger;
            _moMoService = moMoService;
            _unitOfWorkManager = unitOfWorkManager;
            _orderHistoryService = orderHistoryService;
        }
        [UnitOfWork]
        public async Task<VnPaymentResponseModel> ProcessVnPayIpnAsync(IQueryCollection collections)
        {
            var correlationId = Guid.NewGuid().ToString(); // For tracking

            _logger.LogInformation("[VNPay IPN] Starting processing. CorrelationId: {CorrelationId}, QueryParams: {ParamCount}",
                            correlationId, collections.Count);

            try
            {
                var response = _vnPayService.PaymentExecute(collections);

                if (!response.Success)
                {
                    _logger.LogWarning(
                        "[VNPay IPN] Signature validation failed. CorrelationId: {CorrelationId}",
                        correlationId
                    );
                    return new VnPaymentResponseModel
                    {
                        VnPayResponseCode = "97",
                        OrderDescription = "Invalid Signature"
                    };
                }
                _logger.LogInformation(
                    "[VNPay IPN] Signature validated successfully. CorrelationId: {CorrelationId}, OrderRef: {OrderRef}, TransactionNo: {TransactionNo}",
                    correlationId, response.OrderId, response.TransactionId
                );

                var rawOrderId = response.OrderId;
                var cleanOrderIdStr = rawOrderId.Contains("_") ? rawOrderId.Split('_')[0] : rawOrderId;

                if (!Guid.TryParse(cleanOrderIdStr, out var orderId))
                {
                    _logger.LogError("[VNPay IPN] Invalid Guid format: {RawId}. Id: {CorrId}", rawOrderId, correlationId);
                    return new VnPaymentResponseModel { VnPayResponseCode = "01", OrderDescription = "Order not found" };
                }

                var order = await _orderRepository.FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning(
                        "[VNPay IPN] Order not found. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                        correlationId, orderId
                    );
                    return new VnPaymentResponseModel
                    {
                        VnPayResponseCode = "01",
                        OrderDescription = "Order not found"
                    };
                }

                _logger.LogInformation(
                    "[VNPay IPN] Order found. CorrelationId: {CorrelationId}, OrderNumber: {OrderNumber}, CurrentStatus: {Status}, PaymentStatus: {PaymentStatus}",
                    correlationId, order.OrderNumber, order.OrderStatus, order.PaymentStatus
                );

                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    _logger.LogInformation("[VNPay IPN] Order already paid. Id: {OrderId}", orderId);
                    return new VnPaymentResponseModel
                    {
                        OrderId = cleanOrderIdStr,
                        VnPayResponseCode = "00",
                        OrderDescription = "Confirm Success"
                    };
                }

                if (response.VnPayResponseCode == "00")
                {
                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        _logger.LogInformation(
                            "[VNPay IPN] Order already paid. Returning success without update. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                            correlationId, orderId
                        );
                        return new VnPaymentResponseModel
                        {
                            OrderId = orderId.ToString(),
                            VnPayResponseCode = "00",
                            OrderDescription = "Already confirmed"
                        };
                    }

                    if (order.PaymentStatus != PaymentStatus.Pending)
                    {
                        _logger.LogWarning(
                            "[VNPay IPN] Invalid payment status transition. CorrelationId: {CorrelationId}, OrderId: {OrderId}, CurrentStatus: {CurrentStatus}",
                            correlationId, orderId, order.PaymentStatus
                        );
                        return new VnPaymentResponseModel
                        {
                            VnPayResponseCode = "02",
                            OrderDescription = "Invalid order state"
                        };
                    }

                    _logger.LogInformation(
                        "[VNPay IPN] Processing successful payment. CorrelationId: {CorrelationId}, OrderId: {OrderId}, Amount: {Amount} ",
                        correlationId, orderId, response.Amount
                    );

                    var oldStatus = order.OrderStatus;
                    var oldPaymentStatus = order.PaymentStatus;

                    // Update order
                    order.MarkAsPaidOnline();
                    await _orderRepository.UpdateAsync(order, autoSave: true);

                    await _orderHistoryService.LogOrderChangeAsync(
                        orderId,
                        oldStatus,
                        order.OrderStatus,
                        oldPaymentStatus,
                        order.PaymentStatus,
                        $"Payment confirmed via VNPay. TransactionId: {response.TransactionId}"
                    );

                    try
                    {
                        await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogError(
                            notifyEx,
                            "[VNPay IPN] Notification failed but payment processed. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                            correlationId, orderId
                        );
                    }

                    _logger.LogInformation(
                        "[VNPay IPN] Payment processed successfully. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                        correlationId, orderId
                    );

                    return new VnPaymentResponseModel
                    {
                        OrderId = orderId.ToString(),
                        VnPayResponseCode = "00",
                        OrderDescription = "Confirm Success"
                    };
                }
                else
                {
                    _logger.LogWarning(
                        "[VNPay IPN] Transaction failed. CorrelationId: {CorrelationId}, OrderId: {OrderId}, VnPayCode: {Code}, Message: {Message}",
                        correlationId, orderId, response.VnPayResponseCode, response.OrderDescription
                    );

                    if (order.PaymentStatus == PaymentStatus.Pending)
                    {
                        var oldPaymentStatus = order.PaymentStatus;
                        order.SetPaymentStatus(PaymentStatus.Failed);
                        await _orderRepository.UpdateAsync(order, autoSave: true);

                        await _orderHistoryService.LogOrderChangeAsync(
                            orderId,
                            order.OrderStatus,
                            order.OrderStatus,
                            oldPaymentStatus,
                            PaymentStatus.Failed,
                            $"Payment failed via VNPay. Code: {response.VnPayResponseCode}"
                        );
                    }
                    return new VnPaymentResponseModel
                    {
                        VnPayResponseCode = response.VnPayResponseCode,
                        OrderDescription = "Transaction failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                ex,
                "[VNPay IPN] Unexpected error. CorrelationId: {CorrelationId}, Message: {Message}",
                correlationId, ex.Message
                );
                // IMPROVEMENT: Return error code to VNPay so they retry
                return new VnPaymentResponseModel
                {
                    VnPayResponseCode = "99",
                    OrderDescription = "System error"
                };
            }

        }

        [UnitOfWork]
        public async Task ProcessMoMoIpnAsync(MomoIPNRequest request)
        {
            var correlationId = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "[MoMo IPN] Starting processing. CorrelationId: {CorrelationId}, OrderId: {OrderId}, RequestId: {RequestId}",
                correlationId, request.orderId, request.requestId
            );

            try
            {
                // Step 1: Validate signature
                bool isValid = await _moMoService.ValidateIPNRequest(request);

                if (!isValid)
                {
                    _logger.LogWarning(
                        "[MoMo IPN] Signature validation failed. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                        correlationId, request.orderId
                    );
                    throw new UserFriendlyException("Dữ liệu IPN từ MoMo không hợp lệ.");
                }

                _logger.LogInformation(
                    "[MoMo IPN] Signature validated successfully. CorrelationId: {CorrelationId}, OrderId: {OrderId}, TransId: {TransId}",
                    correlationId, request.orderId, request.transId
                );

                // Step 2: Parse and validate OrderId
                if (!Guid.TryParse(request.orderId, out var orderId))
                {
                    _logger.LogError(
                        "[MoMo IPN] Invalid OrderId format. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                        correlationId, request.orderId
                    );
                    throw new UserFriendlyException("Mã tham chiếu không hợp lệ.");
                }

                // Step 3: Get order with details
                var order = await _orderRepository
                    .WithDetailsAsync(o => o.OrderItems)
                    .ContinueWith(async query =>
                    {
                        var queryable = await query;
                        return await AsyncExecuter.FirstOrDefaultAsync(
                            queryable.Where(o => o.Id == orderId)
                        );
                    }).Unwrap();

                if (order == null)
                {
                    _logger.LogWarning(
                        "[MoMo IPN] Order not found. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                        correlationId, orderId
                    );
                    throw new UserFriendlyException("Không tìm thấy đơn hàng.");
                }

                _logger.LogInformation(
                    "[MoMo IPN] Order found. CorrelationId: {CorrelationId}, OrderNumber: {OrderNumber}, CurrentStatus: {Status}, PaymentStatus: {PaymentStatus}",
                    correlationId, order.OrderNumber, order.OrderStatus, order.PaymentStatus
                );

                // Step 4: Process based on result code
                if (request.resultCode == 0)
                {
                    // IMPROVEMENT: Idempotency check
                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        _logger.LogInformation(
                            "[MoMo IPN] Order already paid. Skipping update. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                            correlationId, orderId
                        );
                        return; // Already processed, return success silently
                    }

                    if (order.PaymentStatus != PaymentStatus.Pending)
                    {
                        _logger.LogWarning(
                            "[MoMo IPN] Invalid payment status transition. CorrelationId: {CorrelationId}, OrderId: {OrderId}, CurrentStatus: {CurrentStatus}",
                            correlationId, orderId, order.PaymentStatus
                        );
                        throw new UserFriendlyException("Trạng thái đơn hàng không hợp lệ.");
                    }

                    // IMPROVEMENT: Validate amount
                    if (request.amount != (long)order.TotalAmount)
                    {
                        _logger.LogError(
                            "[MoMo IPN] Amount mismatch. CorrelationId: {CorrelationId}, OrderId: {OrderId}, Expected: {Expected}, Received: {Received}",
                            correlationId, orderId, order.TotalAmount, request.amount
                        );
                        throw new UserFriendlyException("Số tiền không khớp với đơn hàng.");
                    }

                    _logger.LogInformation(
                        "[MoMo IPN] Processing successful payment. CorrelationId: {CorrelationId}, OrderId: {OrderId}, Amount: {Amount}",
                        correlationId, orderId, request.amount
                    );

                    var oldStatus = order.OrderStatus;
                    var oldPaymentStatus = order.PaymentStatus;

                    // Update order
                    order.MarkAsPaidOnline();
                    await _orderRepository.UpdateAsync(order, autoSave: true);

                    // IMPROVEMENT: Log to order history
                    await _orderHistoryService.LogOrderChangeAsync(
                        orderId,
                        oldStatus,
                        order.OrderStatus,
                        oldPaymentStatus,
                        order.PaymentStatus,
                        $"Payment confirmed via MoMo. TransId: {request.transId}"
                    );

                    // Send notifications
                    try
                    {
                        await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogError(
                            notifyEx,
                            "[MoMo IPN] Notification failed but payment processed. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                            correlationId, orderId
                        );
                    }

                    _logger.LogInformation(
                        "[MoMo IPN] Payment processed successfully. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
                        correlationId, orderId
                    );
                }
                else
                {
                    // IMPROVEMENT: Handle failed transactions
                    _logger.LogWarning(
                        "[MoMo IPN] Transaction failed. CorrelationId: {CorrelationId}, OrderId: {OrderId}, ResultCode: {Code}, Message: {Message}",
                        correlationId, orderId, request.resultCode, request.message
                    );

                    if (order.PaymentStatus == PaymentStatus.Pending)
                    {
                        var oldPaymentStatus = order.PaymentStatus;
                        order.SetPaymentStatus(PaymentStatus.Failed);
                        await _orderRepository.UpdateAsync(order, autoSave: true);

                        await _orderHistoryService.LogOrderChangeAsync(
                            orderId,
                            order.OrderStatus,
                            order.OrderStatus,
                            oldPaymentStatus,
                            PaymentStatus.Failed,
                            $"Payment failed via MoMo. ResultCode: {request.resultCode}, Message: {request.message}"
                        );
                    }
                }
            }
            catch (UserFriendlyException)
            {
                throw; // Let ABP handle user-friendly exceptions
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[MoMo IPN] Unexpected error. CorrelationId: {CorrelationId}, OrderId: {OrderId}, Message: {Message}",
                    correlationId, request.orderId, ex.Message
                );
                throw new UserFriendlyException("Đã có lỗi xảy ra khi xử lý IPN từ MoMo.");
            }
        }

    }
}

