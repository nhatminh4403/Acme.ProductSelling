using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// Mã phản hồi cho VNPay. "00" là thành công.
        /// </summary>
        public string RspCode { get; set; }

        /// <summary>
        /// Thông điệp phản hồi cho VNPay.
        /// </summary>
        public string Message { get; set; }
    }
}
