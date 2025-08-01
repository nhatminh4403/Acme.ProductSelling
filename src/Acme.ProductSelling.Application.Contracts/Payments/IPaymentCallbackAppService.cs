using Acme.ProductSelling.PaymentGateway.MoMo.Configurations;
using Acme.ProductSelling.PaymentGateway.MoMo.Models;
using Acme.ProductSelling.PaymentGateway.VnPay.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Payments
{
    public interface IPaymentCallbackAppService : IApplicationService
    {
        Task<VnPaymentResponseModel> ProcessVnPayIpnAsync(IQueryCollection queryCollections);

        //Task<MoMoPaymentResponse> ProcessMoMoIpnAsync(IQueryCollection queryCollections);

        
    }

}
