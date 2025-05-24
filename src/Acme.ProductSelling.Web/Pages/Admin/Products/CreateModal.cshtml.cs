using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
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
        public Dictionary<string, string> CategorySpecTypesJson { get; set; }
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
            InitializeProductDto();
        }
        public async Task OnGetAsync()
        {
            InitializeProductDto();

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
        private void InitializeProductDto()
        {
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
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            await _productAppService.CreateAsync(Product);
            return NoContent();

        }
    }
}
