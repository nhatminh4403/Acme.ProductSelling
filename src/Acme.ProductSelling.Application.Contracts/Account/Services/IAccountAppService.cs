using Acme.ProductSelling.Account.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Account.Services
{
    public interface IAccountAppService : IApplicationService
    {
        Task<RolePrefixDto> GetRolePrefixAsync();
    }
}
