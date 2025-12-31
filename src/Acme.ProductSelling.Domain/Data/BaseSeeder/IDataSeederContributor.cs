using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Data.BaseSeeder
{
    public interface IDataSeederContributor : ITransientDependency
    {
        Task SeedAsync();
    }
}
