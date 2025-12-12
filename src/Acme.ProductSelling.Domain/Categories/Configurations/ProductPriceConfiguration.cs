using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Products
{
    public static class ProductPriceConfiguration
    {
        public record PriceLimits(decimal? Min, decimal? Max);

        public static PriceLimits GetLimits(SpecificationType type, PriceRangeEnum range)
        {
            switch (type)
            {
                // ==========================================
                // 1. HIGH VALUE: Laptops & Desktop Systems
                // ==========================================
                case SpecificationType.Laptop:
                case SpecificationType.Console: // PS5, Xbox, etc.
                case SpecificationType.Handheld: // SteamDeck, ROG Ally
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 15_000_000),             // < 15M
                        PriceRangeEnum.LowToMedium => new(15_000_000, 20_000_000),
                        PriceRangeEnum.Medium => new(20_000_000, 25_000_000),
                        PriceRangeEnum.High => new(25_000_000, 40_000_000),
                        PriceRangeEnum.Premium => new(40_000_000, null),         // > 40M
                        _ => new(null, null)
                    };

                // ==========================================
                // 2. DISPLAYS: Monitors
                // ==========================================
                case SpecificationType.Monitor:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 4_000_000),              // < 4M
                        PriceRangeEnum.LowToMedium => new(4_000_000, 8_000_000),
                        PriceRangeEnum.Medium => new(8_000_000, 15_000_000),
                        PriceRangeEnum.High => new(15_000_000, 25_000_000),
                        PriceRangeEnum.Premium => new(25_000_000, null),
                        _ => new(null, null)
                    };

                // ==========================================
                // 3. CORE COMPONENTS: GPU (High Variance)
                // ==========================================
                case SpecificationType.GPU:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 5_000_000),              // Entry (GTX/RX low)
                        PriceRangeEnum.LowToMedium => new(5_000_000, 10_000_000), // Mid (RTX xx50/60)
                        PriceRangeEnum.Medium => new(10_000_000, 20_000_000),     // High (RTX xx70/80)
                        PriceRangeEnum.High => new(20_000_000, 35_000_000),       // Ultra
                        PriceRangeEnum.Premium => new(35_000_000, null),          // Flagship (4090)
                        _ => new(null, null)
                    };

                // ==========================================
                // 4. COMPONENTS: CPU, Motherboard
                // ==========================================
                case SpecificationType.CPU:
                case SpecificationType.Motherboard:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 3_000_000),               // i3 / Ryzen 3
                        PriceRangeEnum.LowToMedium => new(3_000_000, 6_000_000),  // i5 / Ryzen 5
                        PriceRangeEnum.Medium => new(6_000_000, 10_000_000),      // i7 / Ryzen 7
                        PriceRangeEnum.High => new(10_000_000, null),             // i9 / Ryzen 9
                        _ => new(null, null)
                    };

                // ==========================================
                // 5. STORAGE & RAM
                // ==========================================
                case SpecificationType.RAM:
                case SpecificationType.Storage: // SSD/HDD
                case SpecificationType.MemoryCard:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 1_000_000),
                        PriceRangeEnum.LowToMedium => new(1_000_000, 2_500_000),
                        PriceRangeEnum.Medium => new(2_500_000, 5_000_000),
                        PriceRangeEnum.High => new(5_000_000, null),
                        _ => new(null, null)
                    };

                // ==========================================
                // 6. PERIPHERALS: Keyboard, Headset, Mic
                // ==========================================
                case SpecificationType.Keyboard:
                case SpecificationType.Headset:
                case SpecificationType.Microphone:
                case SpecificationType.Speaker:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 1_000_000),               // Budget
                        PriceRangeEnum.LowToMedium => new(1_000_000, 2_500_000),  // Mid-range
                        PriceRangeEnum.Medium => new(2_500_000, 5_000_000),       // High-end
                        PriceRangeEnum.High => new(5_000_000, null),              // Audiophile / Custom
                        _ => new(null, null)
                    };

                // ==========================================
                // 7. PERIPHERALS: Mouse, Webcam
                // ==========================================
                case SpecificationType.Mouse:
                case SpecificationType.Webcam:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 500_000),
                        PriceRangeEnum.LowToMedium => new(500_000, 1_500_000),
                        PriceRangeEnum.Medium => new(1_500_000, 3_000_000),
                        PriceRangeEnum.High => new(3_000_000, null),
                        _ => new(null, null)
                    };

                // ==========================================
                // 8. FURNITURE: Chairs & Desks
                // ==========================================
                case SpecificationType.Chair:
                case SpecificationType.Desk:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 3_000_000),
                        PriceRangeEnum.LowToMedium => new(3_000_000, 7_000_000),
                        PriceRangeEnum.Medium => new(7_000_000, 15_000_000),
                        PriceRangeEnum.High => new(15_000_000, null), // Herman Miller etc
                        _ => new(null, null)
                    };

                // ==========================================
                // 9. COMPONENTS: Case, PSU, Cooler, Fan
                // ==========================================
                case SpecificationType.Case:
                case SpecificationType.PSU:
                case SpecificationType.CPUCooler:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 1_500_000),
                        PriceRangeEnum.LowToMedium => new(1_500_000, 3_000_000),
                        PriceRangeEnum.Medium => new(3_000_000, 6_000_000),
                        PriceRangeEnum.High => new(6_000_000, null),
                        _ => new(null, null)
                    };

                // ==========================================
                // 10. ACCESSORIES (Cheap Items)
                // ==========================================
                case SpecificationType.CaseFan:
                case SpecificationType.MousePad:
                case SpecificationType.Cable:
                case SpecificationType.Charger:
                case SpecificationType.Hub:
                case SpecificationType.NetworkHardware: // Routers (Home use)
                case SpecificationType.PowerBank:
                case SpecificationType.Software:
                    return range switch
                    {
                        PriceRangeEnum.Low => new(null, 300_000),
                        PriceRangeEnum.LowToMedium => new(300_000, 800_000),
                        PriceRangeEnum.Medium => new(800_000, 2_000_000),
                        PriceRangeEnum.High => new(2_000_000, null),
                        _ => new(null, null)
                    };

                // Default
                default:
                    return new(null, null);
            }
        }

        // Logic to help the Sidebar determine which checkboxes to draw
        public static List<PriceRangeEnum> GetSupportedRanges(SpecificationType type)
        {
            var list = new List<PriceRangeEnum>();
            foreach (PriceRangeEnum r in Enum.GetValues(typeof(PriceRangeEnum)))
            {
                var limit = GetLimits(type, r);
                // If specific limits are defined (not null), include it
                if (limit.Min != null || limit.Max != null) list.Add(r);
            }
            return list;
        }
    }
}