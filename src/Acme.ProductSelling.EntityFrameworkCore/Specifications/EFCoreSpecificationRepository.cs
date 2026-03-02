using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.Specifications.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Specifications
{
    internal class EFCoreSpecificationRepository : EfCoreRepository<ProductSellingDbContext, SpecificationBase, Guid>, ISpecificationRepository
    {
        public EFCoreSpecificationRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
