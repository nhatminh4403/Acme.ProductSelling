using Acme.ProductSelling.PaymentGateway.VnPay.Configurations;
using Acme.ProductSelling.PaymentGateway.VnPay.Dtos;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.PaymentGateway.VnPay.Services
{
    public interface IVnPayService : ITransientDependency
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);

        VnPayOptions GetVnPayOptions();
    }
}
