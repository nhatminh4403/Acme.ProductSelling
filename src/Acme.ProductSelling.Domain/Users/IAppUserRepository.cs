using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Users
{
    public interface IAppUserRepository : IRepository<AppUser, Guid>
    {
    }
}
