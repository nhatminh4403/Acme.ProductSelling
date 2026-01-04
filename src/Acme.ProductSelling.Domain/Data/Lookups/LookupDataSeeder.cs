using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Products.Lookups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Lookups
{
    public class LookupDataSeeder : IDataSeederContributor
    {
        private readonly IRepository<CpuSocket, Guid> _socketRepository;
        private readonly IRepository<Chipset, Guid> _chipsetRepository;
        private readonly IRepository<FormFactor, Guid> _formFactorRepository;
        private readonly IRepository<Material, Guid> _materialRepository;
        private readonly IRepository<PanelType, Guid> _panelTypeRepository;
        private readonly IRepository<RamType, Guid> _ramTypeRepository;
        private readonly IRepository<SwitchType, Guid> _switchTypeRepository;

        public Dictionary<string, CpuSocket> Sockets { get; private set; }
        public Dictionary<string, Chipset> Chipsets { get; private set; }
        public Dictionary<string, FormFactor> FormFactors { get; private set; }
        public Dictionary<string, Material> Materials { get; private set; }
        public Dictionary<string, PanelType> PanelTypes { get; private set; }
        public Dictionary<string, RamType> RamTypes { get; private set; }
        public Dictionary<string, SwitchType> SwitchTypes { get; private set; }

        public LookupDataSeeder(
            IRepository<CpuSocket, Guid> socketRepository,
            IRepository<Chipset, Guid> chipsetRepository,
            IRepository<FormFactor, Guid> formFactorRepository,
            IRepository<Material, Guid> materialRepository,
            IRepository<PanelType, Guid> panelTypeRepository,
            IRepository<RamType, Guid> ramTypeRepository,
            IRepository<SwitchType, Guid> switchTypeRepository)
        {
            _socketRepository = socketRepository;
            _chipsetRepository = chipsetRepository;
            _formFactorRepository = formFactorRepository;
            _materialRepository = materialRepository;
            _panelTypeRepository = panelTypeRepository;
            _ramTypeRepository = ramTypeRepository;
            _switchTypeRepository = switchTypeRepository;

            Sockets = new Dictionary<string, CpuSocket>();
            Chipsets = new Dictionary<string, Chipset>();
            FormFactors = new Dictionary<string, FormFactor>();
            Materials = new Dictionary<string, Material>();
            PanelTypes = new Dictionary<string, PanelType>();
            RamTypes = new Dictionary<string, RamType>();
            SwitchTypes = new Dictionary<string, SwitchType>();
        }

        public async Task SeedAsync()
        {
            await SeedSocketsAsync();
            await SeedChipsetsAsync();
            await SeedFormFactorsAsync();
            await SeedMaterialsAsync();
            await SeedPanelTypesAsync();
            await SeedRamTypesAsync();
            await SeedSwitchTypesAsync();
        }

        private async Task SeedSocketsAsync()
        {
            Sockets["AM4"] = await _socketRepository.InsertAsync(new CpuSocket { Name = "AM4" });
            Sockets["AM5"] = await _socketRepository.InsertAsync(new CpuSocket { Name = "AM5" });
            Sockets["LGA1700"] = await _socketRepository.InsertAsync(new CpuSocket { Name = "LGA1700" });
            Sockets["LGA1851"] = await _socketRepository.InsertAsync(new CpuSocket { Name = "LGA1851" });
            Sockets["LGA1200"] = await _socketRepository.InsertAsync(new CpuSocket { Name = "LGA1200" });
        }

        private async Task SeedChipsetsAsync()
        {
            Chipsets["Z790"] = await _chipsetRepository.InsertAsync(new Chipset { Name = "Intel Z790 Express" });
            Chipsets["Z890"] = await _chipsetRepository.InsertAsync(new Chipset { Name = "Intel Z890" });
            Chipsets["X670E"] = await _chipsetRepository.InsertAsync(new Chipset { Name = "AMD X670E" });
        }

        private async Task SeedFormFactorsAsync()
        {
            FormFactors["ATX"] = await _formFactorRepository.InsertAsync(new FormFactor { Name = "ATX" });
            FormFactors["E-ATX"] = await _formFactorRepository.InsertAsync(new FormFactor { Name = "E-ATX" });
            FormFactors["ATX Mid Tower"] = await _formFactorRepository.InsertAsync(new FormFactor { Name = "ATX Mid Tower" });
        }

        private async Task SeedMaterialsAsync()
        {
            Materials["Steel"] = await _materialRepository.InsertAsync(new Material { Name = "Steel" });
            Materials["Tempered Glass"] = await _materialRepository.InsertAsync(new Material { Name = "Tempered Glass" });
        }

        private async Task SeedPanelTypesAsync()
        {
            PanelTypes["IPS"] = await _panelTypeRepository.InsertAsync(new PanelType { Name = "IPS" });
            PanelTypes["TN"] = await _panelTypeRepository.InsertAsync(new PanelType { Name = "TN" });
            PanelTypes["OLED"] = await _panelTypeRepository.InsertAsync(new PanelType { Name = "OLED" });
            PanelTypes["VA"] = await _panelTypeRepository.InsertAsync(new PanelType { Name = "VA" });
        }

        private async Task SeedRamTypesAsync()
        {
            RamTypes["DDR4"] = await _ramTypeRepository.InsertAsync(new RamType { Name = "DDR4" });
            RamTypes["DDR5"] = await _ramTypeRepository.InsertAsync(new RamType { Name = "DDR5" });
        }

        private async Task SeedSwitchTypesAsync()
        {
            SwitchTypes["Linear Red"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Red (Linear)" });
            SwitchTypes["Linear Black"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Black (Linear)" });
            SwitchTypes["Tactile Brown"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Brown (Tactile)" });
            SwitchTypes["Tactile Clear"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Clear (Tactile)" });
            SwitchTypes["Clicky Blue"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Blue (Clicky)" });
            SwitchTypes["Clicky Green"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Green (Clicky)" });
            SwitchTypes["Silent Red"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Cherry MX Silent Red (Silent Linear)" });
            SwitchTypes["Low Profile"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Kailh Choc (Low Profile)" });
            SwitchTypes["Optical Red"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Razer Optical Red (Optical Linear)" });
            SwitchTypes["Hall Effect"] = await _switchTypeRepository.InsertAsync(
                new SwitchType { Name = "Wooting Lekker (Hall Effect Analog)" });
        }

    }
}
