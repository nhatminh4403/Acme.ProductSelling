using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Categories.Configurations
{
    public static class PriceCategoryStrategy
    {
        public static List<PriceRangeEnum> GetRangesForCategory(SpecificationType specType)
        {
            switch (specType)
            {
                case SpecificationType.Laptop:
                case SpecificationType.Monitor:
                    // Expensive items: Show from Low to Premium
                    return new List<PriceRangeEnum> {
                        PriceRangeEnum.Low,
                        PriceRangeEnum.Medium,
                        PriceRangeEnum.High,
                        PriceRangeEnum.Premium
                    };

                case SpecificationType.Mouse:
                case SpecificationType.Keyboard:
                case SpecificationType.Headset:
                    // Cheaper items: Ignore High/Premium, maybe only show Low/Mid
                    return new List<PriceRangeEnum> {
                        PriceRangeEnum.Low,
                        PriceRangeEnum.LowToMedium,
                        PriceRangeEnum.Medium
                    };

                case SpecificationType.Chair:
                    return new List<PriceRangeEnum> {
                        PriceRangeEnum.Medium,
                        PriceRangeEnum.High
                    };

                // CASE: Categories you want to IGNORE (Show no price column)
                case SpecificationType.Software:
                case SpecificationType.Services:
                case SpecificationType.Cable:
                    return new List<PriceRangeEnum>();

                // Default behavior for the other 20+ categories
                default:
                    return new List<PriceRangeEnum>();
            }
        }
    }
}
