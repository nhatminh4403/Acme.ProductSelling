using Acme.ProductSelling.PaymentGateway.VNPay.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Payments
{
    public interface IPaymentCallbackAppService : IApplicationService
    {
        Task<VnPaymentResponseModel> ProcessVnPayIpnAsync(IQueryCollection queryCollections);
    }

}
