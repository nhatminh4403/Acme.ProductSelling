using Acme.ProductSelling.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Products.Services
{
    public interface IRecentlyViewedProductAppService : IApplicationService
    {
        Task TrackProductViewAsync(Guid productId);
        Task<List<RecentlyViewedProductDto>> GetListAsync(GetRecentlyViewedInputDtos input);
        Task ClearAsync();
        Task SyncGuestHistoryAsync(SyncGuestHistoryInput input);

    }
}
