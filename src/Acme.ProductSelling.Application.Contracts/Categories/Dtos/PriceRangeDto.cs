// Location: Acme.ProductSelling.Application.Contracts/Categories/Dtos/PriceRangeDto.cs
using Acme.ProductSelling.Products;

namespace Acme.ProductSelling.Categories.Dtos
{
    /// <summary>
    /// Represents a price range with display information for UI consumption
    /// </summary>
    public class PriceRangeDto
    {
        /// <summary>
        /// The enum value representing this price range
        /// </summary>
        public PriceRangeEnum Range { get; set; }

        /// <summary>
        /// Minimum price in this range (VND)
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// Maximum price in this range (VND)
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// Localized display text for this range (e.g., "Under 1 million VND")
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// URL-friendly value for routing (e.g., "under-1m")
        /// </summary>
        public string UrlValue { get; set; }

        /// <summary>
        /// Localization key for this price range
        /// </summary>
        public string LocalizationKey { get; set; }
    }
}