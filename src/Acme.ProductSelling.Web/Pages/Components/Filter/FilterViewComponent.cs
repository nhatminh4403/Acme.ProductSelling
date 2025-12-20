using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Filter
{
    public class FilterViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke(FilterViewModel model)
        {
            return View("~/Pages/Components/Filter/Default.cshtml", model);
        }
    }
}
