using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Comments
{
    public interface ICommentAppService : IApplicationService
    {
        Task<List<CommentDto>> GetListAsync(CommentListDto input);
        Task<CommentDto> CreateAsync(CreateCommentDto input);
        Task ToggleLikeAsync(Guid commentId);

    }
}
