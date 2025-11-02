using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Account
{
    public interface IProfileAppService : IApplicationService
    {
        Task<UpdateProfileDto> GetAsync();
        Task<UpdateProfileDto> UpdateAsync(UpdateProfileDto input);
        Task ChangePasswordAsync(ChangePasswordDto input);
    }
}
