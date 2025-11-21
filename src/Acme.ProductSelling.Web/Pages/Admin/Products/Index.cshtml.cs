using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Products
{
    [Authorize(ProductSellingPermissions.Products.Default)]
    public class IndexModel : AdminPageModelBase
    {
        public Dictionary<string, string> CategorySpecTypesJson { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Manufacturers { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IManufacturerAppService _manufacturerAppService;
        public IndexModel(
            IProductAppService productAppService,
            ICategoryAppService categoryAppService,
            IManufacturerAppService manufacturerAppService)
        {
            _productAppService = productAppService; 
            _categoryAppService = categoryAppService;
            _manufacturerAppService = manufacturerAppService;
        }
        public async Task OnGet()
        {

            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect($"/{RoleBasedPrefix}/products");
            }

            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();

            Categories = new List<SelectListItem>
            {
                new SelectListItem("-- Select Category --", "", true)
            };
            Categories.AddRange(categoryLookup.Items
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList());

            CategorySpecTypesJson = categoryLookup.Items
               .ToDictionary(
                   c => c.Id.ToString(),
                   c => c.SpecificationType.ToString() ?? SpecificationType.None.ToString()
               );

            var manufacturerLookup = await _manufacturerAppService.GetManufacturerLookupAsync();
            Manufacturers = new List<SelectListItem>
            {
                new SelectListItem("-- Select Manufacturer --", "", true)
            };
            Manufacturers.AddRange(manufacturerLookup.Items
               .Select(m => new SelectListItem(m.Name, m.Id.ToString()))
               .ToList());

        }
    }
}
