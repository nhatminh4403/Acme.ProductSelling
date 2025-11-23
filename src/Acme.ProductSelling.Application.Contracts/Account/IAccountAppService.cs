using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Account
{
    public interface IAccountAppService : IApplicationService
    {
        Task<RolePrefixDto> GetRolePrefixAsync();
    }
}
