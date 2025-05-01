using Acme.ProductSelling.Specifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products
{
    public class CreateUpdateProductDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        [Range(0, 999999)]
        public int StockCount { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid ManufacturerId { get; set; }
        public CreateUpdateMonitorSpecificationDto MonitorSpecification { get; set; }
        public CreateUpdateMouseSpecificationDto MouseSpecification { get; set; }
        public CreateUpdateLaptopSpecificationDto LaptopSpecification { get; set; } // Giả sử có

        // --- Thêm mới Spec Create/Update DTOs ---
        public CreateUpdateCpuSpecificationDto CpuSpecification { get; set; }
        public CreateUpdateGpuSpecificationDto GpuSpecification { get; set; }
        public CreateUpdateRamSpecificationDto RamSpecification { get; set; }
        public CreateUpdateMotherboardSpecificationDto MotherboardSpecification { get; set; }
        public CreateUpdateStorageSpecificationDto StorageSpecification { get; set; }
        public CreateUpdatePsuSpecificationDto PsuSpecification { get; set; }
        public CreateUpdateCaseSpecificationDto CaseSpecification { get; set; }
        public CreateUpdateCpuCoolerSpecificationDto CpuCoolerSpecification { get; set; }
        public CreateUpdateKeyboardSpecificationDto KeyboardSpecification { get; set; }
        public CreateUpdateHeadsetSpecificationDto HeadsetSpecification { get; set; }
    }
}
