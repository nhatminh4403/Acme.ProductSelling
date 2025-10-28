using Acme.ProductSelling.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Users
{
    public class AppUserRepository : EfCoreRepository<ProductSellingDbContext, AppUser, Guid>, IAppUserRepository
    {
        public AppUserRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
