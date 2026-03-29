using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Comments;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CommentToCommentDtoMapper : MapperBase<Comment, CommentDto>
{
    public override partial CommentDto Map(Comment source);
    public override partial void Map(Comment source, CommentDto destination);
}