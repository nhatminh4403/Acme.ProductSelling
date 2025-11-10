using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Stores
{
    public interface IStoreRepository : IRepository<Store, Guid>
    {
        Task<Store> FindByCodeAsync(string code);
    }
}
