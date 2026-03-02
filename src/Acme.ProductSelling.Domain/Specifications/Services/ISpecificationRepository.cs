using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Specifications.Services
{
    public interface ISpecificationRepository : IRepository<SpecificationBase, Guid>
    {
    }
}
