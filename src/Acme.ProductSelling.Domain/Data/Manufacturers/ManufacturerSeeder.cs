using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Data.Manufacturers
{
    public class ManufacturerSeeder : IDataSeederContributor
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public Dictionary<string, Manufacturer> SeededManufacturers { get; private set; }

        public ManufacturerSeeder(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            SeededManufacturers = new Dictionary<string, Manufacturer>();
        }

        public async Task SeedAsync()
        {
            if (await _manufacturerRepository.GetCountAsync() > 0)
            {
                return; // Already seeded
            }

            SeededManufacturers["ASUS"] = await CreateManufacturerAsync(
                "ASUS", "asus", "https://www.asus.com/",
                "/images/manufacturers/asus-logo.png",
                "ASUS is a multinational computer hardware and electronics company headquartered in Taipei, Taiwan...");

            SeededManufacturers["Acer"] = await CreateManufacturerAsync(
                "Acer", "acer", "https://www.acer.com/",
                "/images/manufacturers/acer-logo.png",
                "Acer is a Taiwanese multinational hardware and electronics corporation...");

            SeededManufacturers["AMD"] = await CreateManufacturerAsync(
                "AMD", "amd", "https://www.amd.com/",
                "/images/manufacturers/amd-logo.png",
                "AMD (Advanced Micro Devices) is an American multinational semiconductor company...");

            SeededManufacturers["Intel"] = await CreateManufacturerAsync(
                "Intel", "intel", "https://www.intel.com/",
                "/images/manufacturers/intel-logo.png",
                "Intel Corporation is an American multinational corporation and technology company...");

            SeededManufacturers["Logitech"] = await CreateManufacturerAsync(
                "Logitech", "logitech", "https://www.logitech.com/",
                "/images/manufacturers/logitech-logo.png",
                "Logitech is a Swiss company that designs and manufactures computer peripherals and software.");

            SeededManufacturers["Corsair"] = await CreateManufacturerAsync(
                "Corsair", "corsair", "https://www.corsair.com/",
                "/images/manufacturers/corsair-logo.png",
                "Corsair is an American computer peripherals and hardware company...");

            SeededManufacturers["Cooler Master"] = await CreateManufacturerAsync(
                "Cooler Master", "cooler-master", "https://www.coolermaster.com/",
                "/images/manufacturers/cooler-master-logo.png",
                "Cooler Master is a Taiwanese computer hardware manufacturer...");

            SeededManufacturers["MSI"] = await CreateManufacturerAsync(
                "MSI", "msi", "https://www.msi.com/",
                "/images/manufacturers/MSI-Logo.png",
                "MSI (Micro-Star International) is a Taiwanese multinational information technology corporation...");

            SeededManufacturers["Gigabyte"] = await CreateManufacturerAsync(
                "Gigabyte", "gigabyte", "https://www.gigabyte.com/",
                "/images/manufacturers/gigabyte-logo.png",
                "Gigabyte Technology is a Taiwanese manufacturer of computer hardware products...");

            SeededManufacturers["Razer"] = await CreateManufacturerAsync(
                "Razer", "razer", "https://www.razer.com/",
                "/images/manufacturers/Razer-logo.png",
                "Razer Inc. is a global gaming hardware manufacturing company...");

            SeededManufacturers["Samsung"] = await CreateManufacturerAsync(
                "Samsung", "samsung", "https://www.samsung.com/",
                "/images/manufacturers/Samsung-Logo.png",
                "Samsung Electronics is a South Korean multinational electronics company...");

            SeededManufacturers["NVIDIA"] = await CreateManufacturerAsync(
                "NVIDIA", "nvidia", "https://www.nvidia.com/",
                "/images/manufacturers/nvidia-logo.png",
                "NVIDIA Corporation is an American multinational technology company...");

            SeededManufacturers["LG"] = await CreateManufacturerAsync(
                "LG", "lg", "https://www.lg.com/",
                "/images/manufacturers/lg-logo.png",
                "LG Electronics is a South Korean multinational electronics company...");

            SeededManufacturers["NZXT"] = await CreateManufacturerAsync(
                "NZXT", "nzxt", "https://www.nzxt.com/",
                "/images/manufacturers/nzxt-logo.png",
                "NZXT is an American computer hardware company...");

            SeededManufacturers["G.Skill"] = await CreateManufacturerAsync(
                "G.Skill", "g-skill", "https://www.gskill.com/",
                "/images/manufacturers/gskill-logo.png",
                "G.Skill is a Taiwanese manufacturer of computer memory modules...");

            SeededManufacturers["HyperX"] = await CreateManufacturerAsync(
                "HyperX", "hyperx", "https://www.hyperxgaming.com/",
                "/images/manufacturers/hyperx-logo.png",
                "HyperX is a gaming division of Kingston Technology Company, Inc.");

            SeededManufacturers["SteelSeries"] = await CreateManufacturerAsync(
                "SteelSeries", "steelseries", "https://steelseries.com/",
                "/images/manufacturers/SteelSeries-logo.png",
                "SteelSeries is a Danish manufacturer of gaming peripherals and accessories.");

            SeededManufacturers["Dell"] = await CreateManufacturerAsync(
                "Dell", "dell", "https://www.dell.com/",
                "/images/manufacturers/Dell-Logo.png",
                "Dell Technologies is an American multinational technology company...");

            SeededManufacturers["HP"] = await CreateManufacturerAsync(
                "HP", "hp", "https://www.hp.com/",
                "/images/manufacturers/hp-logo.png",
                "HP Inc. is an American multinational information technology company...");

            SeededManufacturers["Phanteks"] = await CreateManufacturerAsync(
                "Phanteks", "phanteks", "https://www.phanteks.com/",
                "/images/manufacturers/phanteks-logo.png",
                "Phanteks is a manufacturer of computer hardware products...");

            SeededManufacturers["EVGA"] = await CreateManufacturerAsync(
                "EVGA", "evga", "https://www.evga.com/",
                "/images/manufacturers/evga-logo.png",
                "EVGA Corporation is an American computer hardware company...");

            SeededManufacturers["WD"] = await CreateManufacturerAsync(
                "WD", "wd", "https://www.westerndigital.com/",
                "images/manufacturers/wd-logo.png",
                "Western Digital Corporation is an American computer data storage company...");

            SeededManufacturers["Noctua"] = await CreateManufacturerAsync(
                "Noctua", "noctua", "https://noctua.at/en/",
                "/images/manufacturers/noctua-logo.png",
                "Noctua is an Austrian manufacturer of computer cooling solutions...");

            SeededManufacturers["ASRock"] = await CreateManufacturerAsync(
                "ASRock", "asrock", "https://www.asrock.com/",
                "/images/manufacturers/ASRock-logo.png",
                "ASRock Inc. is a Taiwanese manufacturer of motherboards, graphics cards...");

            SeededManufacturers["IKEA"] = await CreateManufacturerAsync(
                "IKEA", "ikea", "https://www.ikea.com/",
                "/images/manufacturers/ikea-logo.png",
                "Swedish multinational conglomerate that designs and sells ready-to-assemble furniture...");

            SeededManufacturers["Microsoft"] = await CreateManufacturerAsync(
                "Microsoft", "microsoft", "https://www.microsoft.com/",
                "/images/manufacturers/microsoft-logo.png",
                "American multinational technology corporation known for Windows, Office, Xbox...");

            SeededManufacturers["TP-Link"] = await CreateManufacturerAsync(
                "TP-Link", "tp-link", "https://www.tp-link.com/",
                "/images/manufacturers/tplink-logo.png",
                "Chinese manufacturer of computer networking products...");

            SeededManufacturers["Valve"] = await CreateManufacturerAsync(
                "Valve", "valve", "https://www.valvesoftware.com/",
                "/images/manufacturers/valve-logo.png",
                "American video game developer and digital distribution company...");

            SeededManufacturers["Sony"] = await CreateManufacturerAsync(
                "Sony", "sony", "https://www.sony.com/",
                "/images/manufacturers/sony-logo.png",
                "Japanese multinational conglomerate known for electronics, gaming (PlayStation)...");

            SeededManufacturers["Anker"] = await CreateManufacturerAsync(
                "Anker", "anker", "https://www.anker.com/",
                "/images/manufacturers/anker-logo.png",
                "Chinese electronics company known for charging accessories...");

            SeededManufacturers["UGREEN"] = await CreateManufacturerAsync(
                "UGREEN", "ugreen", "https://www.ugreen.com/",
                "/images/manufacturers/ugreen-logo.png",
                "Chinese manufacturer of computer and mobile accessories...");

            SeededManufacturers["JBL"] = await CreateManufacturerAsync(
                "JBL", "jbl", "https://www.jbl.com/",
                "/images/manufacturers/jbl-logo.png",
                "American audio electronics company known for speakers, headphones and sound systems.");
        }

        private async Task<Manufacturer> CreateManufacturerAsync(
            string name,
            string urlSlug,
            string contactInfo,
            string imageUrl,
            string description)
        {
            return await _manufacturerRepository.InsertAsync(new Manufacturer
            {
                Name = name,
                UrlSlug = urlSlug,
                ContactInfo = contactInfo,
                ManufacturerImage = imageUrl,
                Description = description
            }, autoSave: true);
        }
    }

}
