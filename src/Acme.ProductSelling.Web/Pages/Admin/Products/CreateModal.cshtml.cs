using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [Authorize(ProductSellingPermissions.Products.Edit)]

    public class CreateModalModel : AbpPageModel
    {
        [BindProperty]
        public CreateUpdateProductDto Product { get; set; }


        // Danh sách Category để đổ vào dropdown
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Manufacturers { get; set; }
        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService; 
        private readonly IManufacturerAppService _manufacturerAppService;
        public CreateModalModel(
           IProductAppService productAppService,
           ICategoryAppService categoryAppService,
           IManufacturerAppService manufacturerAppService)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            _manufacturerAppService = manufacturerAppService;
            Product = new CreateUpdateProductDto();
            Product.MonitorSpecification = new CreateUpdateMonitorSpecificationDto();
            Product.MouseSpecification = new CreateUpdateMouseSpecificationDto();
            Product.LaptopSpecification = new CreateUpdateLaptopSpecificationDto();
            Product.CpuSpecification = new CreateUpdateCpuSpecificationDto();
            Product.GpuSpecification = new CreateUpdateGpuSpecificationDto();
            Product.RamSpecification = new CreateUpdateRamSpecificationDto();
            Product.MotherboardSpecification = new CreateUpdateMotherboardSpecificationDto();
            Product.StorageSpecification = new CreateUpdateStorageSpecificationDto();
            Product.PsuSpecification = new CreateUpdatePsuSpecificationDto();
            Product.CaseSpecification = new CreateUpdateCaseSpecificationDto();
            Product.CpuCoolerSpecification = new CreateUpdateCpuCoolerSpecificationDto();
            Product.KeyboardSpecification = new CreateUpdateKeyboardSpecificationDto();
            Product.HeadsetSpecification = new CreateUpdateHeadsetSpecificationDto();
        }
        public async Task OnGet()
        {
            Product = new CreateUpdateProductDto();
            Product = new CreateUpdateProductDto
            {
                MonitorSpecification = new CreateUpdateMonitorSpecificationDto(),
                MouseSpecification = new CreateUpdateMouseSpecificationDto(),
                LaptopSpecification = new CreateUpdateLaptopSpecificationDto(),
                CpuSpecification = new CreateUpdateCpuSpecificationDto(),
                GpuSpecification = new CreateUpdateGpuSpecificationDto(),
                RamSpecification = new CreateUpdateRamSpecificationDto(),
                MotherboardSpecification = new CreateUpdateMotherboardSpecificationDto(),
                StorageSpecification = new CreateUpdateStorageSpecificationDto(),
                PsuSpecification = new CreateUpdatePsuSpecificationDto(),
                CaseSpecification = new CreateUpdateCaseSpecificationDto(),
                CpuCoolerSpecification = new CreateUpdateCpuCoolerSpecificationDto(),
                KeyboardSpecification = new CreateUpdateKeyboardSpecificationDto(),
                HeadsetSpecification = new CreateUpdateHeadsetSpecificationDto()
            };
            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();



            Categories = categoryLookup.Items
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
            var manufacturerLookup = await _manufacturerAppService.GetManufacturerLookupAsync();
            Manufacturers = manufacturerLookup.Items
               .Select(m => new SelectListItem(m.Name, m.Id.ToString()))
               .ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                await OnGet(); 
                return Page();
            }

            await _productAppService.CreateAsync(Product);
            return NoContent();
            
        }
    }
}
