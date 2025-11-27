using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Blogs;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BlogToBlogDtoMapper : MapperBase<Blog, BlogDto>
{
    public override partial BlogDto Map(Blog source);
    public override partial void Map(Blog source, BlogDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateBlogDtoToBlogMapper : MapperBase<CreateAndUpdateBlogDto, Blog>
{
    // ConstructUsing equivalent in Mapperly via direct method implementation
    public override Blog Map(CreateAndUpdateBlogDto source)
    {
        return new Blog(
            Guid.NewGuid(),
            source.Title,
            source.Content,
            source.PublishedDate,
            source.Author,
            source.AuthorId,
            source.UrlSlug,
            source.MainImageUrl,
            source.MainImageId
        );
    }

    // Usually update involves mapping properties to existing entity
    public override partial void Map(CreateAndUpdateBlogDto source, Blog destination);
}