using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
namespace Acme.ProductSelling.Products
{
    public class Product : FullAuditedAggregateRoot<Guid>
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        private decimal _originalPrice;
        public decimal OriginalPrice
        {
            get => _originalPrice;
            set
            {
                _originalPrice = value;
                CalculateDiscountedPrice();
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountedPrice { get; set; }

        private double _discountPercent;
        [Range(0, 100)]
        public double DiscountPercent
        {
            get => _discountPercent;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(DiscountPercent), "Phần trăm giảm phải từ 0 đến 100.");

                _discountPercent = value;
                CalculateDiscountedPrice();
            }
        }

        public int StockCount { get; set; }

        [MaxLength(ProductConsts.MaxUrlSlugLength)]
        public string UrlSlug { get; set; }

        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual MonitorSpecification MonitorSpecification { get; set; }
        public virtual MouseSpecification MouseSpecification { get; set; }
        public virtual LaptopSpecification LaptopSpecification { get; set; }
        public virtual CpuSpecification CpuSpecification { get; set; }
        public virtual GpuSpecification GpuSpecification { get; set; }
        public virtual RamSpecification RamSpecification { get; set; }
        public virtual MotherboardSpecification MotherboardSpecification { get; set; }
        public virtual StorageSpecification StorageSpecification { get; set; }
        public virtual PsuSpecification PsuSpecification { get; set; }
        public virtual CaseSpecification CaseSpecification { get; set; }
        public virtual CpuCoolerSpecification CpuCoolerSpecification { get; set; }
        public virtual KeyboardSpecification KeyboardSpecification { get; set; }
        public virtual HeadsetSpecification HeadsetSpecification { get; set; }
        public virtual SpeakerSpecification SpeakerSpecification { get; set; }
        public virtual WebcamSpecification WebcamSpecification { get; set; }
        public virtual CableSpecification CableSpecification { get; set; }
        public virtual SoftwareSpecification SoftwareSpecification { get; set; }
        public virtual CaseFanSpecification CaseFanSpecification { get; set; }
        public virtual ChairSpecification ChairSpecification { get; set; }
        public virtual DeskSpecification DeskSpecification { get; set; }
        public virtual ChargerSpecification ChargerSpecification { get; set; }
        public virtual ConsoleSpecification ConsoleSpecification { get; set; }
        public virtual HandheldSpecification HandheldSpecification { get; set; }
        public virtual HubSpecification HubSpecification { get; set; }
        public virtual MemoryCardSpecification MemoryCardSpecification { get; set; }
        public virtual MicrophoneSpecification MicrophoneSpecification { get; set; }
        public virtual MousePadSpecification mousepadSpecification { get; set; }
        public virtual NetworkHardwareSpecification NetworkHardwareSpecification { get; set; }
        public virtual PowerBankSpecification PowerBankSpecification { get; set; }

        private void CalculateDiscountedPrice()
        {

            var rawPrice = OriginalPrice * (decimal)(1 - DiscountPercent / 100.0);
            DiscountedPrice = RoundToNearestTenThousand(rawPrice);


        }
        private decimal RoundToNearestTenThousand(decimal value)
        {
            return Math.Round(value / 10000m, 0, MidpointRounding.AwayFromZero) * 10000m;
        }
    }
}
