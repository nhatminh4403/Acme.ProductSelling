using System;
using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Comments;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Blogs;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class BlogMapper : MapperBase<Blog, BlogDto>
{
    public override partial BlogDto Map(Blog source);

    public override void Map(Blog source, BlogDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateBlogMapper
{
    // Mimics the 'ConvertUsing' logic from AutoMapper
    // Requires the source manually because the constructor generates ID and sets protected props
    public Blog Map(CreateAndUpdateBlogDto source)
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
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CommentMapper : MapperBase<Comment, CommentDto>
{
    public override partial CommentDto Map(Comment source);

    public override void Map(Comment source, CommentDto destination)
    {
        throw new NotImplementedException();
    }
}