using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Manufacturers
{
    public interface IManufacturerRepository : IRepository<Manufacturer, Guid>
    {
        Task<List<Manufacturer>> GetListAsync();
        Task<Manufacturer> GetBySlugAsync(string slug);
    }
}
