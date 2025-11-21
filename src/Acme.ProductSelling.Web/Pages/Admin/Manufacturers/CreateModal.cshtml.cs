using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Manufacturers
{
    [Authorize(ProductSellingPermissions.Manufacturers.Create)]
    public class CreateModalModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        [BindProperty]
        public CreateUpdateManufacturerDto Manufacturer { get; set; }
        private readonly IManufacturerAppService _manufacturerAppService;

        public CreateModalModel(IManufacturerAppService manufacturerAppService)
        {
            _manufacturerAppService = manufacturerAppService;
        }

        public void OnGet()
        {
            Manufacturer = new CreateUpdateManufacturerDto();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _manufacturerAppService.CreateAsync(Manufacturer);
            return NoContent();
        }
    }
}
