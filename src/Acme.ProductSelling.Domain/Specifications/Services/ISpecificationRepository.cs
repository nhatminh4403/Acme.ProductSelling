using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Specifications.Services
{
    public interface ISpecificationRepository : IRepository<SpecificationBase, Guid>
    {
    }
}
