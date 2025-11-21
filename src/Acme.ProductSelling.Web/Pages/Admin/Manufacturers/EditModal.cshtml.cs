using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Manufacturers
{
    [Authorize(ProductSellingPermissions.Categories.Edit)]
    public class EditModalModel : ProductSellingPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        [BindProperty(SupportsGet = true)]
        public CreateUpdateManufacturerDto Manufacturer { get; set; }

        private readonly IManufacturerAppService _manufacturerAppService;

        public EditModalModel(IManufacturerAppService categoryAppService)
        {
            _manufacturerAppService = categoryAppService;
        }
        public async Task OnGet()
        {
            var manuDto = await _manufacturerAppService.GetAsync(Id);
            Manufacturer = ObjectMapper.Map<ManufacturerDto, CreateUpdateManufacturerDto>(manuDto);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _manufacturerAppService.UpdateAsync(Id, Manufacturer);
            return NoContent();
        }
    }
}
