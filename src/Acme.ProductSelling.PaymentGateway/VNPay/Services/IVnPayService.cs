using Acme.ProductSelling.PaymentGateway.VNPay.Dtos;
using Microsoft.AspNetCore.Http;

namespace Acme.ProductSelling.VNPay.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
