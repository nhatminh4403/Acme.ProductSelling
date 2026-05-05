using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Specifications;
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

        #region Specifications
        public SpecificationBaseDto SpecificationBase { get; set; }
        #endregion
 
 
    }
}