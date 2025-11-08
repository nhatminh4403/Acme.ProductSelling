using Acme.ProductSelling.Manufacturers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Categories
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
    }


    public class GroupedCategoriesResultDto
    {
        public List<CategoryGroupDto> Groups { get; set; }
        public List<CategoryInGroupDto> IndividualCategories { get; set; }
    }
}
