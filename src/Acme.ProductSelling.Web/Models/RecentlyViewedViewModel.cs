using Acme.ProductSelling.Products.Dtos;
using System;
using System.Collections.Generic;

namespace Acme.ProductSelling.Web.Models
{
    public class RecentlyViewedViewModel
    {
        public int MaxCount { get; set; } = 6;
        public Guid? ExcludeProductId { get; set; }
        public string Title { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
