using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products.Dtos
{
    public class RecentlyViewedProductDto 
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string UrlSlug { get; set; }
        [Range(0, 100)]
        public double DiscountPercent { get; set; }
        public bool IsAvailableForPurchase { get; set; }
        public int TotalStockAcrossAllStores { get; set; }
    }
}
