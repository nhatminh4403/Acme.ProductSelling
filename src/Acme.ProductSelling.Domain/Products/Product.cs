using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
namespace Acme.ProductSelling.Products
{
    public class Product : FullAuditedAggregateRoot<Guid>
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }

        [MaxLength(ProductConsts.MaxUrlSlugLength)]
        public string UrlSlug { get;  set; }

        public string ImageUrl { get; set; } 
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Guid? MonitorSpecificationId { get; set; }
        public MonitorSpecification MonitorSpecification { get; set; }
        public Guid? MouseSpecificationId { get; set; }
        public MouseSpecification MouseSpecification { get; set; }
        public Guid? LaptopSpecificationId { get; set; } // Giả sử có LaptopSpec
        public LaptopSpecification LaptopSpecification { get; set; }
        // --- Thêm mới Spec FKs & Navigations ---
        public Guid? CpuSpecificationId { get; set; }
        public CpuSpecification CpuSpecification { get; set; }
        public Guid? GpuSpecificationId { get; set; }
        public GpuSpecification GpuSpecification { get; set; }
        public Guid? RamSpecificationId { get; set; }
        public RamSpecification RamSpecification { get; set; }
        public Guid? MotherboardSpecificationId { get; set; }
        public MotherboardSpecification MotherboardSpecification { get; set; }
        public Guid? StorageSpecificationId { get; set; }
        public StorageSpecification StorageSpecification { get; set; }
        public Guid? PsuSpecificationId { get; set; }
        public PsuSpecification PsuSpecification { get; set; }
        public Guid? CaseSpecificationId { get; set; }
        public CaseSpecification CaseSpecification { get; set; }
        public Guid? CpuCoolerSpecificationId { get; set; }
        public CpuCoolerSpecification CpuCoolerSpecification { get; set; }
        public Guid? KeyboardSpecificationId { get; set; }
        public KeyboardSpecification KeyboardSpecification { get; set; }
        public Guid? HeadsetSpecificationId { get; set; }
        public HeadsetSpecification HeadsetSpecification { get; set; }
        public virtual void SetProductNameAndSlug(string productName)
        {
            ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: ProductConsts.MaxProductNameLength);
            UrlSlug = UrlHelper.RemoveDiacritics(productName);
        }

        internal virtual void SetSlug(string slug)
        {
            UrlSlug = Check.NotNullOrWhiteSpace(slug, nameof(slug), maxLength: ProductConsts.MaxUrlSlugLength);
        }

    }
}
