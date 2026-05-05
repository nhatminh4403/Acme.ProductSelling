using Acme.ProductSelling.Account.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Account.Services
{
    public interface IProfileAppService : IApplicationService
    {
        //Task<UpdateProfileDto> GetAsync();
        Task<UpdateProfileDto> UpdateAsync(UpdateProfileDto input);
        Task ChangePasswordAsync(ChangePasswordDto input);
        Task UpdateShippingAddressAsync(UpdateShippingAddressDto input);
        Task<CustomerProfileDto> GetAsync();
    }
}
