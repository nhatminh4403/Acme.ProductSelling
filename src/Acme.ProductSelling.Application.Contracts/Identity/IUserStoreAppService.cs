using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Identity
{
    public interface IUserStoreAppService : IApplicationService
    {
        Task AssignUserToStoreAsync(AssignUserToStoreDto input);
        Task<Guid?> GetUserAssignedStoreAsync(Guid userId);
    }
}
