using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Cable;
using Acme.ProductSelling.Specifications.CaseFan;
using Acme.ProductSelling.Specifications.Chair;
using Acme.ProductSelling.Specifications.Charger;
using Acme.ProductSelling.Specifications.Console;
using Acme.ProductSelling.Specifications.Desk;
using Acme.ProductSelling.Specifications.Handheld;
using Acme.ProductSelling.Specifications.Hub;
using Acme.ProductSelling.Specifications.MemoryCard;
using Acme.ProductSelling.Specifications.Microphone;
using Acme.ProductSelling.Specifications.MousePad;
using Acme.ProductSelling.Specifications.NetworkHardware;
using Acme.ProductSelling.Specifications.PowerBank;
using Acme.ProductSelling.Specifications.Software;
using Acme.ProductSelling.Specifications.Speaker;
using Acme.ProductSelling.Specifications.Webcam;
using System;
using System.Collections.Generic;
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
        public string UrlSlug { get; set; }
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public SpecificationType CategorySpecificationType { get; set; }
        public List<ProductStoreAvailabilityDto> StoreAvailability { get; set; }
        public int TotalStockAcrossAllStores { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsAvailableForPurchase { get; set; }

        // Existing specifications
        #region Specifications
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

        // New specifications
        public SpeakerSpecificationDto SpeakerSpecification { get; set; }
        public WebcamSpecificationDto WebcamSpecification { get; set; }
        public CableSpecificationDto CableSpecification { get; set; }
        public SoftwareSpecificationDto SoftwareSpecification { get; set; }
        public CaseFanSpecificationDto CaseFanSpecification { get; set; }
        public ChairSpecificationDto ChairSpecification { get; set; }
        public DeskSpecificationDto DeskSpecification { get; set; }
        public ChargerSpecificationDto ChargerSpecification { get; set; }
        public ConsoleSpecificationDto ConsoleSpecification { get; set; }
        public HandheldSpecificationDto HandheldSpecification { get; set; }
        public HubSpecificationDto HubSpecification { get; set; }
        public MemoryCardSpecificationDto MemoryCardSpecification { get; set; }
        public MicrophoneSpecificationDto MicrophoneSpecification { get; set; }
        public MousePadSpecificationDto MousepadSpecification { get; set; }
        public NetworkHardwareSpecificationDto NetworkHardwareSpecification { get; set; }
        public PowerBankSpecificationDto PowerBankSpecification { get; set; }
        #endregion
    }
}