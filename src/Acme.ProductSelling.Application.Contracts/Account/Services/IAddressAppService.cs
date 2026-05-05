using Acme.ProductSelling.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Account.Services
{
    public interface IAddressAppService : IApplicationService
    {
        Task<List<AddressDto>> GetListAsync();
        Task<AddressDto> CreateAsync(CreateAddressDto input);
        Task<AddressDto> UpdateAsync(CreateAddressDto input);
        Task DeleteAsync(Guid id);
        Task SetDefaultAsync(Guid id);
    }
}
