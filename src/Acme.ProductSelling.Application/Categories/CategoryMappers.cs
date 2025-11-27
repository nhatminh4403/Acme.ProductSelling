using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Stores;
using Acme.ProductSelling.Stores.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Categories;

#region Category Mappers

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class CategoryMapper : MapperBase<Category, CategoryDto>
{
    public override partial CategoryDto Map(Category source);

    public override void Map(Category source, CategoryDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryToCreateUpdateMapper : MapperBase<CategoryDto, CreateUpdateCategoryDto>
{
    public override partial CreateUpdateCategoryDto Map(CategoryDto source);

    public override void Map(CategoryDto source, CreateUpdateCategoryDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCategoryMapper : MapperBase<CreateUpdateCategoryDto, Category>
{
    [ObjectFactory]
    Category Create(CreateUpdateCategoryDto dto)
    {
        return new Category(
            Guid.NewGuid(),
            dto.Name,
            dto.Description,
            dto.UrlSlug,
            dto.SpecificationType,
            dto.CategoryGroup,
            dto.DisplayOrder,
            dto.IconCssClass
        );
    }

    // Properties handled by Ctor
    [MapperIgnoreTarget(nameof(Category.Id))]
    [MapperIgnoreTarget(nameof(Category.Name))]
    [MapperIgnoreTarget(nameof(Category.Description))]
    [MapperIgnoreTarget(nameof(Category.UrlSlug))]
    [MapperIgnoreTarget(nameof(Category.SpecificationType))]
    [MapperIgnoreTarget(nameof(Category.CategoryGroup))]
    [MapperIgnoreTarget(nameof(Category.DisplayOrder))]
    [MapperIgnoreTarget(nameof(Category.IconCssClass))]

    [MapperIgnoreTarget(nameof(Category.Products))]
    [MapperIgnoreTarget(nameof(Category.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Category.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Category.CreationTime))]
    [MapperIgnoreTarget(nameof(Category.CreatorId))]
    [MapperIgnoreTarget(nameof(Category.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Category.LastModifierId))]
    [MapperIgnoreTarget(nameof(Category.IsDeleted))]
    [MapperIgnoreTarget(nameof(Category.DeleterId))]
    [MapperIgnoreTarget(nameof(Category.DeletionTime))]
    public override partial Category Map(CreateUpdateCategoryDto source);

    public override void Map(CreateUpdateCategoryDto source, Category destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryLookupMapper : MapperBase<Category, CategoryLookupDto>
{
    public override partial CategoryLookupDto Map(Category source);

    public override void Map(Category source, CategoryLookupDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryInGroupMapper : MapperBase<Category, CategoryInGroupDto>
{
    [MapperIgnoreTarget(nameof(CategoryInGroupDto.Manufacturers))]
    public override partial CategoryInGroupDto Map(Category source);

    public override void Map(Category source, CategoryInGroupDto destination)
    {
        throw new NotImplementedException();
    }
}
#endregion
