using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Folder;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Cable;
using Acme.ProductSelling.Specifications.CaseFan;
using Acme.ProductSelling.Specifications.Chair;
using Acme.ProductSelling.Specifications.Charger;
using Acme.ProductSelling.Specifications.Console;
using Acme.ProductSelling.Specifications.Desk;
using Acme.ProductSelling.Specifications.Handheld;
using Acme.ProductSelling.Specifications.Hub;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using Acme.ProductSelling.Specifications.MemoryCard;
using Acme.ProductSelling.Specifications.Microphone;
using Acme.ProductSelling.Specifications.MousePad;
using Acme.ProductSelling.Specifications.NetworkHardware;
using Acme.ProductSelling.Specifications.PowerBank;
using Acme.ProductSelling.Specifications.Software;
using Acme.ProductSelling.Specifications.Speaker;
using Acme.ProductSelling.Specifications.Webcam;
using Acme.ProductSelling.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.Web.Pages.Admin.Products
{
    [Authorize(ProductSellingPermissions.Products.Edit)]
    public class EditModel : ProductSellingPageModel
    {
        [BindProperty]
        public CreateUpdateProductDto Product { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public Dictionary<string, string> CategorySpecTypesJson { get; set; }

        // Existing lists
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Manufacturers { get; set; }
        public List<SelectListItem> CpuSockets { get; set; }
        public List<SelectListItem> Chipsets { get; set; }
        public List<SelectListItem> FormFactors { get; set; }
        public List<SelectListItem> RamTypes { get; set; }
        public List<SelectListItem> PanelTypes { get; set; }
        public List<SelectListItem> SwitchTypes { get; set; }
        public List<SelectListItem> Materials { get; set; }

        // Enum-based dropdowns for new specs
        public List<SelectListItem> BearingTypes { get; set; }
        public List<SelectListItem> CardTypes { get; set; }
        public List<SelectListItem> SpeakerTypes { get; set; }
        public List<SelectListItem> MicrophoneTypes { get; set; }
        public List<SelectListItem> FocusTypes { get; set; }
        public List<SelectListItem> MousePadMaterials { get; set; }
        public List<SelectListItem> SurfaceTypes { get; set; }
        public List<SelectListItem> ChairMaterials { get; set; }
        public List<SelectListItem> ArmrestTypes { get; set; }
        public List<SelectListItem> DeskMaterials { get; set; }
        public List<SelectListItem> SoftwareTypes { get; set; }
        public List<SelectListItem> LicenseTypes { get; set; }
        public List<SelectListItem> Platforms { get; set; }
        public List<SelectListItem> NetworkDeviceTypes { get; set; }
        public List<SelectListItem> WifiStandards { get; set; }
        public List<SelectListItem> HubTypes { get; set; }
        public List<SelectListItem> CableTypes { get; set; }
        public List<SelectListItem> ChargerTypes { get; set; }
        public List<SelectListItem> OpticalDriveTypes { get; set; }

        // Existing enums
        public List<SelectListItem> ConnectivityTypes { get; set; }
        public List<SelectListItem> KeyboardLayouts { get; set; }
        public List<SelectListItem> PsuModularities { get; set; }
        public List<SelectListItem> StorageFormFactors { get; set; }
        public List<SelectListItem> StorageTypes { get; set; }

        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IManufacturerAppService _manufacturerAppService;
        private readonly IPanelTypeAppService _panelTypeAppService;
        private readonly IRamTypeAppService _ramTypeAppService;
        private readonly IMaterialAppService _materialAppService;
        private readonly IChipsetAppService _chipsetAppService;
        private readonly ICpuSocketAppService _cpuSocketAppService;
        private readonly ISwitchTypeAppService _switchTypeAppService;
        private readonly IFormFactorAppSerivce _formFactorAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;

        private const string ProductImageUpload = FolderConsts.ImageFolder + "product-images";

        public EditModel(
            IProductAppService productAppService,
            ICategoryAppService categoryAppService,
            IManufacturerAppService manufacturerAppService,
            IWebHostEnvironment webHostEnvironment,
            IStringLocalizer<ProductSellingResource> localizer,
            IPanelTypeAppService panelTypeAppService,
            IRamTypeAppService ramTypeAppService,
            IMaterialAppService materialAppService,
            IChipsetAppService chipsetAppService,
            ICpuSocketAppService cpuSocketAppService,
            ISwitchTypeAppService switchTypeAppService,
            IFormFactorAppSerivce formFactorAppService)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            _manufacturerAppService = manufacturerAppService;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
            _panelTypeAppService = panelTypeAppService;
            _ramTypeAppService = ramTypeAppService;
            _materialAppService = materialAppService;
            _chipsetAppService = chipsetAppService;
            _cpuSocketAppService = cpuSocketAppService;
            _switchTypeAppService = switchTypeAppService;
            _formFactorAppService = formFactorAppService;
        }

        public async Task OnGetAsync()
        {
            await LoadProductAsync();
            await LoadDropdownDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var imageSourceType = Request.Form["imageSourceType"].ToString();

            ValidateImageInput(imageSourceType);

            if (!ModelState.IsValid)
            {
                Logger.LogWarning("ModelState is invalid");
                LogModelStateErrors();
                await LoadDropdownDataAsync();
                return Page();
            }

            if (!await HandleImageUploadAsync(imageSourceType))
            {
                await LoadDropdownDataAsync();
                return Page();
            }

            await ResetUnusedSpecificationsAsync();

            try
            {
                await _productAppService.UpdateAsync(Id, Product);

                TempData["UserMessage"] = _localizer["Product:UpdateSuccessMessage", Product.ProductName];
                TempData["MessageType"] = "success";

                return Redirect("/admin/products");
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User-friendly error updating product");
                TempData["UserMessage"] = _localizer["Product:UpdateFailureMessageWithReason", Product.ProductName, ex.Message];
                TempData["MessageType"] = "danger";
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating product");
                TempData["UserMessage"] = _localizer["Product:UpdateFailureMessageGeneral", Product.ProductName];
                TempData["MessageType"] = "danger";
                ModelState.AddModelError(string.Empty, _localizer["Product:UpdateErrorGeneral"]);
            }

            await LoadDropdownDataAsync();
            return Page();
        }

        // ============================================
        // PRIVATE HELPER METHODS
        // ============================================

        private async Task LoadProductAsync()
        {
            var productDto = await _productAppService.GetAsync(Id);
            Product = ObjectMapper.Map<ProductDto, CreateUpdateProductDto>(productDto);

            // Initialize null specifications
            InitializeNullSpecifications();
        }

        private void InitializeNullSpecifications()
        {
            Product.MonitorSpecification ??= new CreateUpdateMonitorSpecificationDto();
            Product.MouseSpecification ??= new CreateUpdateMouseSpecificationDto();
            Product.LaptopSpecification ??= new CreateUpdateLaptopSpecificationDto();
            Product.CpuSpecification ??= new CreateUpdateCpuSpecificationDto();
            Product.GpuSpecification ??= new CreateUpdateGpuSpecificationDto();
            Product.RamSpecification ??= new CreateUpdateRamSpecificationDto();
            Product.MotherboardSpecification ??= new CreateUpdateMotherboardSpecificationDto();
            Product.StorageSpecification ??= new CreateUpdateStorageSpecificationDto();
            Product.PsuSpecification ??= new CreateUpdatePsuSpecificationDto();
            Product.CaseSpecification ??= new CreateUpdateCaseSpecificationDto();
            Product.CpuCoolerSpecification ??= new CreateUpdateCpuCoolerSpecificationDto();
            Product.KeyboardSpecification ??= new CreateUpdateKeyboardSpecificationDto();
            Product.HeadsetSpecification ??= new CreateUpdateHeadsetSpecificationDto();

            // New specs
            Product.CaseFanSpecification ??= new CreateUpdateCaseFanSpecificationDto();
            Product.MemoryCardSpecification ??= new CreateUpdateMemoryCardSpecificationDto();
            Product.SpeakerSpecification ??= new CreateUpdateSpeakerSpecificationDto();
            Product.MicrophoneSpecification ??= new CreateUpdateMicrophoneSpecificationDto();
            Product.WebcamSpecification ??= new CreateUpdateWebcamSpecificationDto();
            Product.MousepadSpecification ??= new CreateUpdateMousePadSpecificationDto();
            Product.ChairSpecification ??= new CreateUpdateChairSpecificationDto();
            Product.DeskSpecification ??= new CreateUpdateDeskSpecificationDto();
            Product.SoftwareSpecification ??= new CreateUpdateSoftwareSpecificationDto();
            Product.NetworkHardwareSpecification ??= new CreateUpdateNetworkHardwareSpecificationDto();
            Product.HandheldSpecification ??= new CreateUpdateHandheldSpecificationDto();
            Product.ConsoleSpecification ??= new CreateUpdateConsoleSpecificationDto();
            Product.HubSpecification ??= new CreateUpdateHubSpecificationDto();
            Product.CableSpecification ??= new CreateUpdateCableSpecificationDto();
            Product.ChargerSpecification ??= new CreateUpdateChargerSpecificationDto();
            Product.PowerBankSpecification ??= new CreateUpdatePowerBankSpecificationDto();
        }

        private async Task LoadDropdownDataAsync()
        {
            var categoryTask = LoadCategoriesAsync();
            var manufacturerTask = LoadManufacturersAsync();
            var lookupTask = LoadLookupDataAsync();
            var enumTask = Task.Run(() => LoadEnumBasedDropdowns());

            await Task.WhenAll(categoryTask, manufacturerTask, lookupTask, enumTask);
        }

        private async Task LoadCategoriesAsync()
        {
            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();

            Categories = new List<SelectListItem>
            {
                new SelectListItem(_localizer["Dropdown:SelectCategory"], "", true)
            };
            Categories.AddRange(categoryLookup.Items
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())));

            CategorySpecTypesJson = categoryLookup.Items
                .ToDictionary(
                    c => c.Id.ToString(),
                    c => c.SpecificationType.ToString() ?? SpecificationType.None.ToString()
                );
        }

        private async Task LoadManufacturersAsync()
        {
            var manufacturerLookup = await _manufacturerAppService.GetManufacturerLookupAsync();

            Manufacturers = new List<SelectListItem>
            {
                new SelectListItem(_localizer["Dropdown:SelectManufacturer"], "", true)
            };
            Manufacturers.AddRange(manufacturerLookup.Items
                .Select(m => new SelectListItem(m.Name, m.Id.ToString())));
        }

        private async Task LoadLookupDataAsync()
        {
            var tasks = new[]
            {
                LoadCpuSocketsAsync(),
                LoadMaterialsAsync(),
                LoadChipsetsAsync(),
                LoadPanelTypesAsync(),
                LoadRamTypesAsync(),
                LoadFormFactorsAsync(),
                LoadSwitchTypesAsync()
            };

            await Task.WhenAll(tasks);
        }

        private async Task LoadCpuSocketsAsync()
        {
            var lookup = await _cpuSocketAppService.GetLookupAsync();
            CpuSockets = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private async Task LoadMaterialsAsync()
        {
            var lookup = await _materialAppService.GetLookupAsync();
            Materials = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private async Task LoadChipsetsAsync()
        {
            var lookup = await _chipsetAppService.GetLookupAsync();
            Chipsets = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private async Task LoadPanelTypesAsync()
        {
            var lookup = await _panelTypeAppService.GetLookupAsync();
            PanelTypes = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private async Task LoadRamTypesAsync()
        {
            var lookup = await _ramTypeAppService.GetLookupAsync();
            RamTypes = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private async Task LoadFormFactorsAsync()
        {
            var lookup = await _formFactorAppService.GetLookupAsync();
            FormFactors = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private async Task LoadSwitchTypesAsync()
        {
            var lookup = await _switchTypeAppService.GetLookupAsync();
            SwitchTypes = lookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();
        }

        private void LoadEnumBasedDropdowns()
        {
            // New specs enums
            BearingTypes = _localizer.ToLocalizedSelectList<BearingType>();
            CardTypes = _localizer.ToLocalizedSelectList<CardType>();
            SpeakerTypes = _localizer.ToLocalizedSelectList<SpeakerType>();
            MicrophoneTypes = _localizer.ToLocalizedSelectList<MicrophoneType>();
            FocusTypes = _localizer.ToLocalizedSelectList<FocusType>();
            MousePadMaterials = _localizer.ToLocalizedSelectList<MousePadMaterial>();
            SurfaceTypes = _localizer.ToLocalizedSelectList<SurfaceType>();
            ChairMaterials = _localizer.ToLocalizedSelectList<ChairMaterial>();
            ArmrestTypes = _localizer.ToLocalizedSelectList<ArmrestType>();
            DeskMaterials = _localizer.ToLocalizedSelectList<DeskMaterial>();
            SoftwareTypes = _localizer.ToLocalizedSelectList<SoftwareType>();
            LicenseTypes = _localizer.ToLocalizedSelectList<LicenseType>();
            Platforms = _localizer.ToLocalizedSelectList<Platform>();
            NetworkDeviceTypes = _localizer.ToLocalizedSelectList<NetworkDeviceType>();
            WifiStandards = _localizer.ToLocalizedSelectList<WifiStandard>();
            HubTypes = _localizer.ToLocalizedSelectList<HubType>();
            CableTypes = _localizer.ToLocalizedSelectList<CableType>();
            ChargerTypes = _localizer.ToLocalizedSelectList<ChargerType>();
            OpticalDriveTypes = _localizer.ToLocalizedSelectList<OpticalDriveType>();

            // Existing enums
            ConnectivityTypes = _localizer.ToLocalizedSelectList<ConnectivityType>();
            KeyboardLayouts = _localizer.ToLocalizedSelectList<KeyboardLayout>();
            PsuModularities = _localizer.ToLocalizedSelectList<PsuModularity>();
            StorageFormFactors = _localizer.ToLocalizedSelectList<StorageFormFactor>();
            StorageTypes = _localizer.ToLocalizedSelectList<StorageType>();
        }

        private void ValidateImageInput(string imageSourceType)
        {
            if (imageSourceType == "url" && string.IsNullOrWhiteSpace(Product.ImageUrl))
            {
                ModelState.AddModelError("Product.ImageUrl", _localizer["Validation:ImageUrlRequired"]);
            }
            else if (imageSourceType == "upload" &&
                     (Product.ProductImageFile == null || Product.ProductImageFile.Length == 0) &&
                     string.IsNullOrWhiteSpace(Product.ImageUrl))
            {
                // Allow keeping existing image if no new file is uploaded
                // This is different from Create page
            }
        }

        private async Task<bool> HandleImageUploadAsync(string imageSourceType)
        {
            if (imageSourceType == "upload" && Product.ProductImageFile != null && Product.ProductImageFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, ProductImageUpload);
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Product.ProductImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Product.ProductImageFile.CopyToAsync(stream);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrWhiteSpace(Product.ImageUrl))
                    {
                        DeleteOldImage(Product.ImageUrl);
                    }

                    Product.ImageUrl = $"/product-images/{uniqueFileName}";
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error uploading product image");
                    ModelState.AddModelError(string.Empty, _localizer["Product:ImageUploadError"]);
                    return false;
                }
            }
            else if (imageSourceType == "url")
            {
                Product.ProductImageFile = null;
                return true;
            }
            else
            {
                // Keep existing image if no changes
                return true;
            }
        }

        private void DeleteOldImage(string imageUrl)
        {
            try
            {
                if (imageUrl.StartsWith("/product-images/"))
                {
                    var fileName = Path.GetFileName(imageUrl);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, ProductImageUpload, fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Could not delete old image: {ImageUrl}", imageUrl);
                // Don't fail the update if we can't delete the old image
            }
        }

        private async Task ResetUnusedSpecificationsAsync()
        {
            if (Product.CategoryId != Guid.Empty)
            {
                var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();
                var selectedCategory = categoryLookup.Items.FirstOrDefault(c => c.Id == Product.CategoryId);

                ResetUnusedSpecifications(selectedCategory?.SpecificationType ?? SpecificationType.None);
            }
            else
            {
                ResetUnusedSpecifications(SpecificationType.None);
            }
        }

        private void ResetUnusedSpecifications(SpecificationType activeSpecType)
        {
            // Existing specs
            if (activeSpecType != SpecificationType.Monitor) Product.MonitorSpecification = null;
            if (activeSpecType != SpecificationType.Mouse) Product.MouseSpecification = null;
            if (activeSpecType != SpecificationType.Laptop) Product.LaptopSpecification = null;
            if (activeSpecType != SpecificationType.CPU) Product.CpuSpecification = null;
            if (activeSpecType != SpecificationType.GPU) Product.GpuSpecification = null;
            if (activeSpecType != SpecificationType.RAM) Product.RamSpecification = null;
            if (activeSpecType != SpecificationType.Motherboard) Product.MotherboardSpecification = null;
            if (activeSpecType != SpecificationType.Storage) Product.StorageSpecification = null;
            if (activeSpecType != SpecificationType.PSU) Product.PsuSpecification = null;
            if (activeSpecType != SpecificationType.Case) Product.CaseSpecification = null;
            if (activeSpecType != SpecificationType.CPUCooler) Product.CpuCoolerSpecification = null;
            if (activeSpecType != SpecificationType.Keyboard) Product.KeyboardSpecification = null;
            if (activeSpecType != SpecificationType.Headset) Product.HeadsetSpecification = null;

            // New specs
            if (activeSpecType != SpecificationType.CaseFan) Product.CaseFanSpecification = null;
            if (activeSpecType != SpecificationType.MemoryCard) Product.MemoryCardSpecification = null;
            if (activeSpecType != SpecificationType.Speaker) Product.SpeakerSpecification = null;
            if (activeSpecType != SpecificationType.Microphone) Product.MicrophoneSpecification = null;
            if (activeSpecType != SpecificationType.Webcam) Product.WebcamSpecification = null;
            if (activeSpecType != SpecificationType.MousePad) Product.MousepadSpecification = null;
            if (activeSpecType != SpecificationType.Chair) Product.ChairSpecification = null;
            if (activeSpecType != SpecificationType.Desk) Product.DeskSpecification = null;
            if (activeSpecType != SpecificationType.Software) Product.SoftwareSpecification = null;
            if (activeSpecType != SpecificationType.NetworkHardware) Product.NetworkHardwareSpecification = null;
            if (activeSpecType != SpecificationType.Handheld) Product.HandheldSpecification = null;
            if (activeSpecType != SpecificationType.Console) Product.ConsoleSpecification = null;
            if (activeSpecType != SpecificationType.Hub) Product.HubSpecification = null;
            if (activeSpecType != SpecificationType.Cable) Product.CableSpecification = null;
            if (activeSpecType != SpecificationType.Charger) Product.ChargerSpecification = null;
            if (activeSpecType != SpecificationType.PowerBank) Product.PowerBankSpecification = null;
        }

        private void LogModelStateErrors()
        {
            foreach (var error in ModelState)
            {
                Logger.LogWarning(
                    "ModelState Error - Key: {Key}, Errors: {Errors}",
                    error.Key,
                    string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))
                );
            }
        }
    }
}