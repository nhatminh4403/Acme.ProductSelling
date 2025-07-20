using Acme.ProductSelling.PaymentGateway.VNPay.Dtos;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.VNPay.Services
{
    public interface IVnPayService : ITransientDependency 
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
