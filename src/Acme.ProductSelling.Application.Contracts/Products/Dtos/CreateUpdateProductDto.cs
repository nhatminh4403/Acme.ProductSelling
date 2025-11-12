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
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Products.Dtos
{
    public class CreateUpdateProductDto
    {
        public Guid Id { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        [Range(0, 100)]
        public double DiscountPercent { get; set; }
        [Required]
        [Range(0, 999999)]
        public int StockCount { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? UrlSlug { get; set; } = string.Empty;
        public IFormFile? ProductImageFile { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid ManufacturerId { get; set; }
        public DateTime? ReleaseDate { get; set; }

        // Existing specifications
        public CreateUpdateMonitorSpecificationDto? MonitorSpecification { get; set; }
        public CreateUpdateMouseSpecificationDto? MouseSpecification { get; set; }
        public CreateUpdateLaptopSpecificationDto? LaptopSpecification { get; set; }
        public CreateUpdateCpuSpecificationDto? CpuSpecification { get; set; }
        public CreateUpdateGpuSpecificationDto? GpuSpecification { get; set; }
        public CreateUpdateRamSpecificationDto? RamSpecification { get; set; }
        public CreateUpdateMotherboardSpecificationDto? MotherboardSpecification { get; set; }
        public CreateUpdateStorageSpecificationDto? StorageSpecification { get; set; }
        public CreateUpdatePsuSpecificationDto? PsuSpecification { get; set; }
        public CreateUpdateCaseSpecificationDto? CaseSpecification { get; set; }
        public CreateUpdateCpuCoolerSpecificationDto? CpuCoolerSpecification { get; set; }
        public CreateUpdateKeyboardSpecificationDto? KeyboardSpecification { get; set; }
        public CreateUpdateHeadsetSpecificationDto? HeadsetSpecification { get; set; }

        // New specifications
        public CreateUpdateSpeakerSpecificationDto? SpeakerSpecification { get; set; }
        public CreateUpdateWebcamSpecificationDto? WebcamSpecification { get; set; }
        public CreateUpdateCableSpecificationDto? CableSpecification { get; set; }
        public CreateUpdateSoftwareSpecificationDto? SoftwareSpecification { get; set; }
        public CreateUpdateCaseFanSpecificationDto? CaseFanSpecification { get; set; }
        public CreateUpdateChairSpecificationDto? ChairSpecification { get; set; }
        public CreateUpdateDeskSpecificationDto? DeskSpecification { get; set; }
        public CreateUpdateChargerSpecificationDto? ChargerSpecification { get; set; }
        public CreateUpdateConsoleSpecificationDto? ConsoleSpecification { get; set; }
        public CreateUpdateHandheldSpecificationDto? HandheldSpecification { get; set; }
        public CreateUpdateHubSpecificationDto? HubSpecification { get; set; }
        public CreateUpdateMemoryCardSpecificationDto? MemoryCardSpecification { get; set; }
        public CreateUpdateMicrophoneSpecificationDto? MicrophoneSpecification { get; set; }
        public CreateUpdateMousePadSpecificationDto? MousepadSpecification { get; set; }
        public CreateUpdateNetworkHardwareSpecificationDto? NetworkHardwareSpecification { get; set; }
        public CreateUpdatePowerBankSpecificationDto? PowerBankSpecification { get; set; }
    }
}