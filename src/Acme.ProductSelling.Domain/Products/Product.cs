using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.StoreInventories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        public DateTime? ReleaseDate { get; set; }
        public int StockCount { get; set; }

        [MaxLength(ProductConsts.MaxUrlSlugLength)]
        public string UrlSlug { get; set; }
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid ManufacturerId { get; set; }
        public bool IsActive { get; set; }


        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ICollection<StoreInventory> StoreInventories { get; set; } = new HashSet<StoreInventory>();

        #region Specifications

        public virtual SpecificationBase SpecificationBase { get; set; }
        #endregion

        private void CalculateDiscountedPrice()
        {
            var rawPrice = OriginalPrice * (decimal)(1 - DiscountPercent / 100.0);
            DiscountedPrice = RoundToNearestTenThousand(rawPrice);
        }
        private decimal RoundToNearestTenThousand(decimal value)
        {
            return Math.Round(value / 10000m, 0, MidpointRounding.AwayFromZero) * 10000m;
        }
        public int GetStockForStore(Guid storeId)
        {
            var inventory = StoreInventories.FirstOrDefault(si => si.StoreId == storeId);
            return inventory?.Quantity ?? 0;
        }

        public bool IsAvailableInStore(Guid storeId)
        {
            var inventory = StoreInventories.FirstOrDefault(si => si.StoreId == storeId);
            return inventory?.IsAvailableForSale ?? false && inventory?.Quantity > 0;
        }
        public bool IsAvailableForPurchase()
        {
            if (!IsActive)
                return false;

            if (!ReleaseDate.HasValue)
                return true;

            return ReleaseDate.Value <= DateTime.Now;
        }
    }
}
