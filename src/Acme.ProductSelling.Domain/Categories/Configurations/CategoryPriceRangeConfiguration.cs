
using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.ProductSelling.Categories.Configurations
{
    /// <summary>
    /// Defines price range boundaries for each category that uses price filtering
    /// </summary>
    public static class CategoryPriceRangeConfiguration
    {
        private const decimal OpenEndedMaxValue = 999_999_999m;

        /// <summary>
        /// Maps specification types to their price range definitions
        /// Key: SpecificationType, Value: Dictionary of PriceRangeEnum to (min, max) boundaries
        /// </summary>
        private static readonly Dictionary<SpecificationType, Dictionary<PriceRangeEnum, (decimal Min, decimal Max)>>
            _priceRangesByCategory = new()
            {
                [SpecificationType.Monitor] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 4999999),
                    [PriceRangeEnum.LowToMedium] = (5000000, 9999999),
                    [PriceRangeEnum.Medium] = (10000000, 19999999),
                    [PriceRangeEnum.MediumToHigh] = (20000000, 29999999),
                    [PriceRangeEnum.High] = (30000000, OpenEndedMaxValue)
                    // Premium is ignored (not defined)
                },

                // Laptop: 15M, 20M+
                [SpecificationType.Laptop] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 14999999),
                    [PriceRangeEnum.Medium] = (15000000, 19999999),
                    [PriceRangeEnum.High] = (20000000, OpenEndedMaxValue)
                    // LowToMedium, MediumToHigh, Premium are ignored
                },

                // Mouse: 500K, 1M, 2M, 3M+
                [SpecificationType.Mouse] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 499999),
                    [PriceRangeEnum.LowToMedium] = (500000, 999999),
                    [PriceRangeEnum.Medium] = (1000000, 1999999),
                    [PriceRangeEnum.MediumToHigh] = (2000000, 2999999),
                    [PriceRangeEnum.High] = (3000000, OpenEndedMaxValue)
                },

                // Keyboard: 1M, 2M, 3M, 4M, 5M+
                [SpecificationType.Keyboard] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 999999),
                    [PriceRangeEnum.LowToMedium] = (1000000, 1999999),
                    [PriceRangeEnum.Medium] = (2000000, 2999999),
                    [PriceRangeEnum.MediumToHigh] = (3000000, 3999999),
                    [PriceRangeEnum.High] = (4000000, 4999999),
                    [PriceRangeEnum.Premium] = (5000000, OpenEndedMaxValue)
                },

                // Case: 1M, 2M, 5M+
                [SpecificationType.Case] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 999999),
                    [PriceRangeEnum.Medium] = (1000000, 1999999),
                    [PriceRangeEnum.High] = (2000000, 4999999),
                    [PriceRangeEnum.Premium] = (5000000, OpenEndedMaxValue)
                },

                // Storage: 2M, 5M+
                [SpecificationType.Storage] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 1999999),
                    [PriceRangeEnum.Medium] = (2000000, 4999999),
                    [PriceRangeEnum.High] = (5000000, OpenEndedMaxValue)
                },

                // Chair: 5M, 10M+
                [SpecificationType.Chair] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 4999999),
                    [PriceRangeEnum.Medium] = (5000000, 9999999),
                    [PriceRangeEnum.High] = (10000000, OpenEndedMaxValue)
                },

                // Headset: 1M, 2M, 3M, 4M+
                [SpecificationType.Headset] = new Dictionary<PriceRangeEnum, (decimal, decimal)>
                {
                    [PriceRangeEnum.Low] = (0, 999999),
                    [PriceRangeEnum.LowToMedium] = (1000000, 1999999),
                    [PriceRangeEnum.Medium] = (2000000, 2999999),
                    [PriceRangeEnum.MediumToHigh] = (3000000, 3999999),
                    [PriceRangeEnum.High] = (4000000, OpenEndedMaxValue)
                }
            };


        public static Dictionary<PriceRangeEnum, (decimal Min, decimal Max)> GetPriceRangesForCategory(
            SpecificationType specificationType)
        {
            return _priceRangesByCategory.TryGetValue(specificationType, out var ranges)
                ? ranges
                : new Dictionary<PriceRangeEnum, (decimal Min, decimal Max)>();
        }


        public static bool HasPriceRanges(SpecificationType specificationType)
        {
            return _priceRangesByCategory.ContainsKey(specificationType);
        }
        public static IEnumerable<SpecificationType> GetCategoriesWithPriceRanges()
        {
            return _priceRangesByCategory.Keys;
        }

        /// <summary>
        /// Check if a price range is open-ended (has the max value placeholder)
        /// </summary>
        public static bool IsOpenEndedRange(decimal maxValue)
        {
            return maxValue >= OpenEndedMaxValue;
        }

        /// <summary>
        /// Get the placeholder value used for open-ended ranges
        /// </summary>
        public static decimal GetOpenEndedMaxValue()
        {
            return OpenEndedMaxValue;
        }
    }
}