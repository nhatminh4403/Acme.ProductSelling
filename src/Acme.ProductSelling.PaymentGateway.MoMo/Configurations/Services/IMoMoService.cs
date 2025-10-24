using Acme.ProductSelling.PaymentGateway.MoMo.Configurations;
using Acme.ProductSelling.PaymentGateway.MoMo.Models;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.PaymentGateway.MoMo.Services
{
    public interface IMoMoService : ITransientDependency
    {

        Task<MoMoPaymentResponse> CreatePaymentAsync(MoMoPaymentRequest request);
        Task<bool> ValidateSignature(string partnerCode, string requestId, string orderId, string orderInfo,
                                    string orderType, long transId, string message,
                                    int resultCode, string payType, long amount,
                                    string extraData, string signatureFromCallback, long responseTime);
        Task<bool> ValidateIPNRequest(MomoIPNRequest request);

        MoMoOption GetMoMoOption();
    }
}
