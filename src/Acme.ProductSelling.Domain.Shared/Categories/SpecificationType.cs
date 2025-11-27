using System.ComponentModel;
using System.Runtime.Serialization;

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


    public enum BearingType
    {
        SleeveSSO,
        SSO2,
        FluidDynamic,
        DoubleBall,
        HydroWave,
        MagLev,
        RifledSSO
    }

    public enum CardType
    {
        SD,
        SDHC,
        SDXC,
        MicroSD,
        MicroSDHC,
        MicroSDXC,
        CompactFlash,
        CFexpress
    }

    public enum SpeakerType
    {
        Stereo_2_0,
        Stereo_2_1_WithSubwoofer,
        Surround_5_1,
        Surround_7_1,
        Soundbar,
        Bookshelf,
        Studio_Monitor
    }

    public enum MicrophoneType
    {
        Dynamic,
        Condenser,
        Ribbon,
        USB,
        XLR,
        Lavalier,
        Shotgun
    }

    public enum PolarPattern
    {
        Cardioid,
        SuperCardioid,
        HyperCardioid,
        Omnidirectional,
        Bidirectional,
        Stereo,
        Multi_Pattern
    }

    public enum FocusType
    {
        FixedFocus,
        AutoFocus,
        ManualFocus,
        HybridAutoFocus
    }

    public enum MousePadMaterial
    {
        Cloth,
        HardPlastic,
        Aluminum,
        Glass,
        Hybrid,
        Rubber,
        Cordura
    }

    public enum SurfaceType
    {
        Speed, // Smooth, fast glide
        Control, // Textured, more control
        Balanced, // Mix of both
        Rough,
        Smooth
    }

    public enum ChairMaterial
    {
        Fabric,
        PULeather,
        GenuineLeather,
        Mesh,
        Hybrid
    }

    public enum ArmrestType
    {
        Fixed,
        Adjustable_1D, // Height only
        Adjustable_2D, // Height + pivot
        Adjustable_3D, // Height + pivot + depth
        Adjustable_4D  // Height + pivot + depth + angle
    }

    public enum DeskMaterial
    {
        Wood,
        MDF,
        ParticleBoard,
        Laminate,
        Tempered_Glass,
        Steel,
        Carbon_Fiber,
        Bamboo
    }

    public enum SoftwareType
    {
        OperatingSystem,
        Productivity,
        Security,
        Creative,
        Gaming,
        Development,
        Utility,
        Antivirus
    }

    public enum LicenseType
    {
        Retail,
        OEM,
        Volume,
        Subscription,
        Perpetual,
        Trial,
        Freeware,
        OpenSource
    }

    public enum Platform
    {
        Windows,
        MacOS,
        Linux,
        Android,
        iOS,
        CrossPlatform,
        Web
    }

    public enum NetworkDeviceType
    {
        Router,
        Switch,
        AccessPoint,
        Modem,
        NetworkAdapter,
        Repeater,
        MeshSystem,
        Firewall
    }

    public enum WifiStandard
    {
        WiFi4_802_11n,
        WiFi5_802_11ac,
        WiFi6_802_11ax,
        WiFi6E_802_11ax_6GHz,
        WiFi7_802_11be
    }

    public enum HubType
    {
        USB_Hub,
        USB_C_Hub,
        Thunderbolt_3_Dock,
        Thunderbolt_4_Dock,
        Universal_Dock,
        Display_Dock
    }

    public enum CableType
    {
        HDMI,
        DisplayPort,
        USB_A_to_USB_B,
        USB_A_to_USB_C,
        USB_C_to_USB_C,
        USB_C_to_Lightning,
        Thunderbolt_3,
        Thunderbolt_4,
        VGA,
        DVI,
        Ethernet_Cat5e,
        Ethernet_Cat6,
        Ethernet_Cat6a,
        Ethernet_Cat7,
        Audio_3_5mm
    }

    public enum ChargerType
    {
        WallCharger,
        CarCharger,
        WirelessCharger,
        DesktopCharger,
        TravelAdapter
    }

    public enum ChargingProtocol
    {
        USB_PD_2_0,
        USB_PD_3_0,
        USB_PD_3_1,
        QualcommQuickCharge_3_0,
        QualcommQuickCharge_4_Plus,
        PPS, // Programmable Power Supply
        AFC, // Adaptive Fast Charging (Samsung)
        SuperVOOC, // Oppo
        DashCharge, // OnePlus
        Proprietary
    }

    public enum OpticalDriveType
    {
        None,
        DVD,
        BluRay,
        UHD_BluRay
    }

    public enum ConnectivityType
    {
        Wired,
        Wireless,
        WiredAndWireless,
        Bluetooth,
        WiredAndBluetooth,
        WirelessAndBluetooth,
        WiredWirelessAndBluetooth,
    }
    public enum KeyboardLayout
    {
        FullSize,
        TKL,
        SeventyFivePercent,
        SixtyFivePercent,
        SixtyPercent
    }
    public enum PsuModularity
    {
        NoneModularity = 0,
        SingleModularity = 1,
        FullModularity = 2,
    }
    public enum RamFormFactor
    {
        [EnumMember(Value = "DIMM")]
        DIMM,

        [EnumMember(Value = "SO-DIMM")]
        SODIMM
    }
    public enum StorageFormFactor
    {
        // HDD
        [Description("3.5-inch HDD")]
        Hdd35Inch,
        [Description("2.5-inch HDD")]
        Hdd25Inch,

        // SSD SATA
        [Description("2.5-inch SSD (SATA)")]
        Ssd25InchSata,
        [Description("mSATA SSD")]
        SsdMSata,

        [Description("M.2 2230")]
        SsdM2_2230,
        [Description("M.2 2242")]
        SsdM2_2242,
        [Description("M.2 2280")]
        SsdM2_2280,
        [Description("M.2 22110")]
        SsdM2_22110,

        [Description("U.2")]
        SsdU2,
        [Description("PCIe Add-in-Card (AIC)")]
        SsdPcieAic,

        [Description("EDSFF E1.S")]
        SsdEdsffE1S,
        [Description("EDSFF E1.L")]
        SsdEdsffE1L,
        [Description("EDSFF E3.S")]
        SsdEdsffE3S,
        [Description("EDSFF E3.L")]
        SsdEdsffE3L
    }
    public enum StorageType
    {
        [Description("Hard Disk Drive (HDD)")]
        HDD,

        [Description("SATA Solid-State Drive (SSD)")]
        SataSsd,

        [Description("NVMe M.2 Solid-State Drive (SSD)")]
        NvmeSsd
    }
}