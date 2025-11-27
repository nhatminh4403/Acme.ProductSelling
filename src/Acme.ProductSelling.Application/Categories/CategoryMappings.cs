using Acme.ProductSelling.Categories.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Categories;

#region
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class CategoryToCategoryDtoMapper : MapperBase<Category, CategoryDto>
{
    public override partial CategoryDto Map(Category source);
    public override partial void Map(Category source, CategoryDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryDtoToCreateUpdateCategoryDtoMapper : MapperBase<CategoryDto, CreateUpdateCategoryDto>
{
    public override partial CreateUpdateCategoryDto Map(CategoryDto source);
    public override partial void Map(CategoryDto source, CreateUpdateCategoryDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCategoryDtoToCategoryMapper : MapperBase<CreateUpdateCategoryDto, Category>
{
    public override Category Map(CreateUpdateCategoryDto source)
    {
        var entity = new Category(
            Guid.NewGuid(),
            source.Name,
            source.Description,
            source.UrlSlug,
            source.SpecificationType,
            source.CategoryGroup,
            source.DisplayOrder,
            source.IconCssClass
        );

        Map(source, entity);
        return entity;
    }
    [MapperIgnoreTarget(nameof(Category.Id))]
    [MapperIgnoreTarget(nameof(Category.Products))]
    public override partial void Map(CreateUpdateCategoryDto source, Category destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryToCategoryLookupDtoMapper : MapperBase<Category, CategoryLookupDto>
{
    public override partial CategoryLookupDto Map(Category source);
    public override partial void Map(Category source, CategoryLookupDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryToCategoryInGroupDtoMapper : MapperBase<Category, CategoryInGroupDto>
{
    [MapperIgnoreTarget(nameof(CategoryInGroupDto.Manufacturers))]
    public override partial CategoryInGroupDto Map(Category source);

    [MapperIgnoreTarget(nameof(CategoryInGroupDto.Manufacturers))]
    public override partial void Map(Category source, CategoryInGroupDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryToCategoryWithManufacturersDtoMapper : MapperBase<Category, CategoryWithManufacturersDto>
{
    [MapProperty(nameof(Category.Name), nameof(CategoryWithManufacturersDto.CategoryName))]
    [MapProperty(nameof(Category.UrlSlug), nameof(CategoryWithManufacturersDto.CategoryUrlSlug))]
    [MapperIgnoreTarget(nameof(CategoryWithManufacturersDto.Manufacturers))]
    [MapperIgnoreTarget(nameof(CategoryWithManufacturersDto.ManufacturerCount))]
    public override partial CategoryWithManufacturersDto Map(Category source);

    [MapProperty(nameof(Category.Name), nameof(CategoryWithManufacturersDto.CategoryName))]
    [MapProperty(nameof(Category.UrlSlug), nameof(CategoryWithManufacturersDto.CategoryUrlSlug))]
    [MapperIgnoreTarget(nameof(CategoryWithManufacturersDto.Manufacturers))]
    [MapperIgnoreTarget(nameof(CategoryWithManufacturersDto.ManufacturerCount))]
    public override partial void Map(Category source, CategoryWithManufacturersDto destination);
}

#endregion