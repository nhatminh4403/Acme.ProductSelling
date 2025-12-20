using Acme.ProductSelling.Manufacturers;
using System;
using System.Collections.Generic;

namespace Acme.ProductSelling.Web.Models
{
    public class FilterViewModel
    {
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }
        public string CategorySlug { get; set; }
        public string ManufacturerSlug { get; set; }
        public string SearchKeyword { get; set; }
        public int PageSize { get; set; } = 12;
        public bool ShowManufacturerFilter { get; set; } = false;
        public List<ManufacturerLookupDto> Manufacturers { get; set; }
        public Guid? SelectedManufacturerId { get; set; }
    }
}
