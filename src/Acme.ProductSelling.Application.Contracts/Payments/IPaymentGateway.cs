using Acme.ProductSelling.Orders;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
namespace Acme.ProductSelling.Payments
{
    public interface IPaymentGateway : ITransientDependency
    {
        string Name { get; }
        Task<PaymentGatewayResult> ProcessAsync(Order order);
    }
    public class PaymentGatewayResult
    {
        public string RedirectUrl { get; set; }
        public bool Success { get; set; } = true;
        public string TransactionId { get; set; } // Gateway transaction ID
        public decimal? ConvertedAmount { get; set; } // For PayPal
        public string? ConvertedCurrency { get; set; } // For PayPal
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
