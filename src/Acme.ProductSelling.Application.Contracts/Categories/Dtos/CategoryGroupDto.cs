using Acme.ProductSelling.Manufacturers;
using System;
using System.Collections.Generic;

namespace Acme.ProductSelling.Categories.Dtos
{
    public class CategoryGroupDto
    {
        public CategoryGroup GroupType { get; set; }
        public string GroupName { get; set; }
        public string GroupIcon { get; set; }
        public int DisplayOrder { get; set; }
        public List<CategoryInGroupDto> Categories { get; set; }
    }
    public class CategoryInGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public SpecificationType SpecificationType { get; set; }
        public List<ManufacturerDto> Manufacturers { get; set; }
        public List<PriceMenuLinkDto> PriceRanges { get; set; } = new List<PriceMenuLinkDto>();

    }
    public class PriceMenuLinkDto
    {
        public string DisplayText { get; set; } // The localized string (e.g., "Under 15 Million")
        public string UrlValue { get; set; }    // The enum value (e.g., "Low")
    }

    public class GroupedCategoriesResultDto
    {
        public List<CategoryGroupDto> Groups { get; set; }
        public List<CategoryInGroupDto> IndividualCategories { get; set; }
    }
}
