using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Payments
{
    public interface IPaymentCallbackAppService : IApplicationService
    {
        Task<VnPayIpnProcessingResult> ProcessVnPayIpnAsync(IQueryCollection queryCollections);
    }
    public class VnPayIpnProcessingResult
    {

        public string RspCode { get; set; }

        public string Message { get; set; }
    }
}
