using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Specifications;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Products.Dtos
{
    public class ProductDto : AuditedEntityDto<Guid>
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        [Range(0, 100)]
        public double DiscountPercent { get; set; }
        public int StockCount { get; set; }
        public string UrlSlug { get; set; } // Đường dẫn URL thân thiện
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public SpecificationType CategorySpecificationType { get; set; }
        public MonitorSpecificationDto MonitorSpecification { get; set; }
        public MouseSpecificationDto MouseSpecification { get; set; }
        public LaptopSpecificationDto LaptopSpecification { get; set; }
        public CpuSpecificationDto CpuSpecification { get; set; }
        public GpuSpecificationDto GpuSpecification { get; set; }
        public RamSpecificationDto RamSpecification { get; set; }
        public MotherboardSpecificationDto MotherboardSpecification { get; set; }
        public StorageSpecificationDto StorageSpecification { get; set; }
        public PsuSpecificationDto PsuSpecification { get; set; }
        public CaseSpecificationDto CaseSpecification { get; set; }
        public CpuCoolerSpecificationDto CpuCoolerSpecification { get; set; }
        public KeyboardSpecificationDto KeyboardSpecification { get; set; }
        public HeadsetSpecificationDto HeadsetSpecification { get; set; }
    }
}
