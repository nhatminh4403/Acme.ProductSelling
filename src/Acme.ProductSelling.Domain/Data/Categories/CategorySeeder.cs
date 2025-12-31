using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Data.Categories
{
    public class CategorySeeder : IDataSeederContributor
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryManager _categoryManager;

        public Dictionary<string, Category> SeededCategories { get; private set; }

        public CategorySeeder(
            ICategoryRepository categoryRepository,
            CategoryManager categoryManager)
        {
            _categoryRepository = categoryRepository;
            _categoryManager = categoryManager;
            SeededCategories = new Dictionary<string, Category>();
        }


        public async Task SeedAsync()
        {
            if (await _categoryRepository.GetCountAsync() > 0)
            {
                return; // Already seeded
            }
            SeededCategories["Laptops"] = await CreateCategoryAsync(
                "Laptops", "Laptops for work and entertainment",
                SpecificationType.Laptop, CategoryGroup.Individual, 1, "fas fa-laptop");

            // Main, CPU, VGA Group
            SeededCategories["CPUs"] = await CreateCategoryAsync(
                "CPUs", "Central Processing Units",
                SpecificationType.CPU, CategoryGroup.MainCpuVga, 2, "fas fa-microchip");

            SeededCategories["GPUs"] = await CreateCategoryAsync(
                "GPUs", "Video Graphics Cards",
                SpecificationType.GPU, CategoryGroup.MainCpuVga, 2, "fas fa-microchip");

            SeededCategories["Motherboards"] = await CreateCategoryAsync(
                "Motherboards", "Main system boards",
                SpecificationType.Motherboard, CategoryGroup.MainCpuVga, 2, "fas fa-microchip");

            // Case, Nguồn, Tản Group
            SeededCategories["Cases"] = await CreateCategoryAsync(
                "Cases", "Computer Chassis",
                SpecificationType.Case, CategoryGroup.CaseAndCooling, 3, "fas fa-box");

            SeededCategories["PSUs"] = await CreateCategoryAsync(
                "PSUs", "Power Supply Units",
                SpecificationType.PSU, CategoryGroup.CaseAndCooling, 3, "fas fa-box");

            SeededCategories["CPU Coolers"] = await CreateCategoryAsync(
                "CPU Coolers", "CPU Cooling Solutions",
                SpecificationType.CPUCooler, CategoryGroup.CaseAndCooling, 3, "fas fa-box");

            SeededCategories["Case Fans"] = await CreateCategoryAsync(
                "Case Fans", "Case Cooling Fans",
                SpecificationType.CaseFan, CategoryGroup.CaseAndCooling, 3, "fas fa-box");

            // Storage, RAM, Memory Group
            SeededCategories["Storages"] = await CreateCategoryAsync(
                "Storages", "Storage Drives",
                SpecificationType.Storage, CategoryGroup.StorageRamMemory, 4, "fas fa-memory");

            SeededCategories["RAMs"] = await CreateCategoryAsync(
                "RAMs", "Random Access Memory",
                SpecificationType.RAM, CategoryGroup.StorageRamMemory, 4, "fas fa-memory");

            SeededCategories["Memory Cards"] = await CreateCategoryAsync(
                "Memory Cards", "SD and MicroSD Cards",
                SpecificationType.MemoryCard, CategoryGroup.StorageRamMemory, 4, "fas fa-memory");

            // Audio/Video Group
            SeededCategories["Speakers"] = await CreateCategoryAsync(
                "Speakers", "Computer Speakers",
                SpecificationType.Speaker, CategoryGroup.AudioVideo, 5, "fas fa-microphone-alt");

            SeededCategories["Microphones"] = await CreateCategoryAsync(
                "Microphones", "Audio Microphones",
                SpecificationType.Microphone, CategoryGroup.AudioVideo, 5, "fas fa-microphone-alt");

            SeededCategories["Webcams"] = await CreateCategoryAsync(
                "Webcams", "Web Cameras",
                SpecificationType.Webcam, CategoryGroup.AudioVideo, 5, "fas fa-microphone-alt");

            // Monitor
            SeededCategories["Monitors"] = await CreateCategoryAsync(
                "Monitors", "Computer Displays",
                SpecificationType.Monitor, CategoryGroup.Individual, 6, "fas fa-tv");

            // Keyboard
            SeededCategories["Keyboards"] = await CreateCategoryAsync(
                "Keyboards", "Mechanical and Membrane Keyboards",
                SpecificationType.Keyboard, CategoryGroup.Individual, 7, "fas fa-keyboard");

            // Mouse & Pad Group
            SeededCategories["Mice"] = await CreateCategoryAsync(
                "Mice", "Computer Mice",
                SpecificationType.Mouse, CategoryGroup.MouseAndPad, 8, "fas fa-mouse");

            SeededCategories["Mouse Pads"] = await CreateCategoryAsync(
                "Mouse Pads", "Gaming and Office Mouse Pads",
                SpecificationType.MousePad, CategoryGroup.MouseAndPad, 8, "fas fa-mouse");

            // Headset
            SeededCategories["Headsets"] = await CreateCategoryAsync(
                "Headsets", "Gaming and Office Headsets",
                SpecificationType.Headset, CategoryGroup.Individual, 9, "fas fa-headphones");

            // Furniture Group
            SeededCategories["Chairs"] = await CreateCategoryAsync(
                "Chairs", "Gaming and Office Chairs",
                SpecificationType.Chair, CategoryGroup.Furniture, 10, "fas fa-chair");

            SeededCategories["Desks"] = await CreateCategoryAsync(
                "Desks", "Work and Gaming Desks",
                SpecificationType.Desk, CategoryGroup.Furniture, 10, "fas fa-chair");

            // Software & Network Group
            SeededCategories["Software"] = await CreateCategoryAsync(
                "Software", "Software and Licenses",
                SpecificationType.Software, CategoryGroup.SoftwareNetwork, 11, "fas fa-network-wired");

            SeededCategories["Networking"] = await CreateCategoryAsync(
                "Networking", "Routers, Switches, Adapters",
                SpecificationType.NetworkHardware, CategoryGroup.SoftwareNetwork, 11, "fas fa-network-wired");

            // Handheld & Console Group
            SeededCategories["Handhelds"] = await CreateCategoryAsync(
                "Handhelds", "Handheld Gaming Consoles",
                SpecificationType.Handheld, CategoryGroup.HandheldConsole, 12, "fas fa-gamepad");

            SeededCategories["Consoles"] = await CreateCategoryAsync(
                "Consoles", "Gaming Consoles",
                SpecificationType.Console, CategoryGroup.HandheldConsole, 12, "fas fa-gamepad");

            // Accessories Group
            SeededCategories["Hubs And Docks"] = await CreateCategoryAsync(
                "Hubs And Docks", "USB Hubs and Docking Stations",
                SpecificationType.Hub, CategoryGroup.Accessories, 13, "fas fa-plug");

            SeededCategories["Cables"] = await CreateCategoryAsync(
                "Cables", "HDMI, USB, and Display Cables",
                SpecificationType.Cable, CategoryGroup.Accessories, 13, "fas fa-plug");

            SeededCategories["Chargers"] = await CreateCategoryAsync(
                "Chargers", "Adapters and Chargers",
                SpecificationType.Charger, CategoryGroup.Accessories, 13, "fas fa-plug");

            SeededCategories["Power Banks"] = await CreateCategoryAsync(
                "Power Banks", "Portable Power Banks",
                SpecificationType.PowerBank, CategoryGroup.Accessories, 13, "fas fa-plug");

            // Services
            SeededCategories["Services"] = await CreateCategoryAsync(
                "Services", "Other Services and Information",
                SpecificationType.Services, CategoryGroup.Individual, 14, "fas fa-info-circle");
        }
        private async Task<Category> CreateCategoryAsync(
            string name,
            string description,
            SpecificationType specificationType,
            CategoryGroup group,
            int sortOrder,
            string icon)
        {
            return await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    name,
                    description,
                    specificationType,
                    group,
                    sortOrder,
                    icon
                )
            );
        }
    }
}