namespace Acme.ProductSelling.Categories
{
    public enum SpecificationType
    {
        None = 0,

        // Individual categories
        Laptop = 1,
        Monitor = 2,
        Keyboard = 3,
        Headset = 4,

        // Main, CPU, VGA group
        Motherboard = 10,
        CPU = 11,
        GPU = 12,

        // Case, Nguồn, Tản group
        Case = 20,
        PSU = 21,
        CPUCooler = 22,
        CaseFan = 23,

        // Storage, RAM, Memory group
        Storage = 30,
        RAM = 31,
        MemoryCard = 32,

        // Audio/Video group
        Speaker = 40,
        Microphone = 41,
        Webcam = 42,

        // Mouse group
        Mouse = 50,
        MousePad = 51,

        // Furniture group
        Chair = 60,
        Desk = 61,

        // Software & Network group
        Software = 70,
        NetworkHardware = 71,

        // Handheld & Console group
        Handheld = 80,
        Console = 81,

        // Accessories group
        Hub = 90,
        Cable = 91,
        Charger = 92,
        PowerBank = 93,

        // Services
        Services = 100
    }
}