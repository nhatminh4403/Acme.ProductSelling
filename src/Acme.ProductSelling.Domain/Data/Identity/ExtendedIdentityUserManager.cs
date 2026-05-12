using Acme.ProductSelling.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace Acme.ProductSelling.Data.Identity
{
    public class ExtendedIdentityUserManager : IdentityUserManager
    {
        public ExtendedIdentityUserManager(IdentityUserStore store, IIdentityRoleRepository roleRepository, IIdentityUserRepository userRepository, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Volo.Abp.Identity.IdentityUser> passwordHasher, IEnumerable<IUserValidator<Volo.Abp.Identity.IdentityUser>> userValidators, IEnumerable<IPasswordValidator<Volo.Abp.Identity.IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<IdentityUserManager> logger, ICancellationTokenProvider cancellationTokenProvider, IOrganizationUnitRepository organizationUnitRepository, ISettingProvider settingProvider, IDistributedEventBus distributedEventBus, IIdentityLinkUserRepository identityLinkUserRepository, IDistributedCache<AbpDynamicClaimCacheItem> dynamicClaimCache, IOptions<AbpMultiTenancyOptions> multiTenancyOptions, ICurrentTenant currentTenant, IDataFilter dataFilter) : base(store, roleRepository, userRepository, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, cancellationTokenProvider, organizationUnitRepository, settingProvider, distributedEventBus, identityLinkUserRepository, dynamicClaimCache, multiTenancyOptions, currentTenant, dataFilter)
        {
        }
        public override async Task<IdentityResult> CreateAsync(Volo.Abp.Identity.IdentityUser user)
        {
            var result = await base.CreateAsync(user);

            if (result.Succeeded)
                await TryAddToCommunityRoleAsync(user);

            return result;
        }

        public override async Task<IdentityResult> CreateAsync(Volo.Abp.Identity.IdentityUser user, string password)
        {
            var result = await base.CreateAsync(user, password);

            if (result.Succeeded)
                await TryAddToCommunityRoleAsync(user);

            return result;
        }

        private async Task TryAddToCommunityRoleAsync(Volo.Abp.Identity.IdentityUser user)
        {
            var normalizedName = NormalizeName(ExtendedRoleConsts.Customer);
            var role = await RoleRepository.FindByNormalizedNameAsync(normalizedName);

            if (role == null) return;

            await AddToRoleAsync(user, ExtendedRoleConsts.Customer);
        }
    }
}
