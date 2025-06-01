using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products
{

    public static class ProductQueryableExtensions
    {
        public static IQueryable<Product> IncludeAllRelations(this IQueryable<Product> query)
        {
            return query
                .Include(p => p.Category)
                .Include(p => p.MonitorSpecification)
                .Include(p => p.MouseSpecification)
                .Include(p => p.LaptopSpecification)
                .Include(p => p.CpuSpecification)
                .Include(p => p.GpuSpecification)
                .Include(p => p.RamSpecification)
                .Include(p => p.MotherboardSpecification)
                .Include(p => p.StorageSpecification)
                .Include(p => p.PsuSpecification)
                .Include(p => p.CaseSpecification)
                .Include(p => p.CpuCoolerSpecification)
                .Include(p => p.KeyboardSpecification)
                .Include(p => p.HeadsetSpecification)
                .Include(p => p.Manufacturer);
        }
    }
}
