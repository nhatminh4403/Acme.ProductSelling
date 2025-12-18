using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.PriceFilter
{
    public class PriceFilterViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke(PriceFilterViewModel model)
        {
            return View("~/Pages/Components/PriceFilter/Default.cshtml", model);
        }
    }
    public class PriceFilterViewModel
    {
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }
        public string CategorySlug { get; set; }
        public string ManufacturerSlug { get; set; }
        public string SearchKeyword { get; set; }
        public int PageSize { get; set; } = 12;
        public bool ShowManufacturerFilter { get; set; } = false;
        public List<ManufacturerFilterDto> Manufacturers { get; set; }
        public Guid? SelectedManufacturerId { get; set; }
    }

    public class ManufacturerFilterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public int ProductCount { get; set; }
    }
}
