using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Users
{
    public interface IProfileAppService : IApplicationService
    {
        Task<UpdateProfileDto> GetAsync();
        Task<UpdateProfileDto> UpdateAsync(UpdateProfileDto input);
        Task ChangePasswordAsync(ChangePasswordDto input);
    }
}
