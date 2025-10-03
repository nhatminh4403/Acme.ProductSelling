using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Acme.ProductSelling.Products
{

    public static class ProductQueryableExtensions
    {
        public static IQueryable<Product> IncludeAllRelations(this IQueryable<Product> query)
        {
            return query
                .Include(p => p.Category)
                .Include(p => p.MonitorSpecification).ThenInclude(p=> p.PanelType)
                .Include(p => p.MouseSpecification)
                .Include(p => p.LaptopSpecification)
                .Include(p => p.CpuSpecification).ThenInclude(s => s.Socket)
                .Include(p => p.GpuSpecification)
                .Include(p => p.RamSpecification).ThenInclude(ram => ram.RamType)
                .Include(p => p.MotherboardSpecification).ThenInclude(s=>s.Socket)
                .Include(p => p.MotherboardSpecification).ThenInclude(s=>s.FormFactor)
                .Include(p => p.MotherboardSpecification).ThenInclude(s=>s.SupportedRamTypes)
                .Include(p => p.MotherboardSpecification).ThenInclude(s=>s.Chipset)

                .Include(p => p.StorageSpecification)
                .Include(p => p.PsuSpecification).ThenInclude(form => form.FormFactor)
                .Include(p => p.CaseSpecification).ThenInclude(cs => cs.Materials).ThenInclude(cm => cm.Material)
                .Include(p => p.CaseSpecification).ThenInclude(form => form.FormFactor)
                .Include(p => p.CpuCoolerSpecification).ThenInclude(ccs => ccs.SupportedSockets).ThenInclude(css => css.Socket)
                .Include(p => p.KeyboardSpecification)
                .Include(p => p.HeadsetSpecification)
                .Include(p => p.Manufacturer);
        }
    }
}
