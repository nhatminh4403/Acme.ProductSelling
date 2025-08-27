using Acme.ProductSelling.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using System.Linq.Dynamic.Core;
using Volo.Abp;
using AutoMapper.Internal.Mappers;
namespace Acme.ProductSelling.Comments
{
    public class CommentAppService : ProductSellingAppService, ICommentAppService
    {
        private readonly IRepository<Comment, Guid> _commentRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Likes> _commentLikeRepository;

        public CommentAppService(IRepository<Comment, Guid> commentRepository,
            IIdentityUserRepository userRepository, ICurrentUser currentUser, IRepository<Likes> commentLikeRepository) 
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _commentLikeRepository = commentLikeRepository;
            //ConfigurePolicies();
        }
        /*        private void ConfigurePolicies()
                {
                    GetPolicyName = null;
                    CreatePolicyName = ProductSellingPermissions.Comments.Create;
                    UpdatePolicyName = ProductSellingPermissions.Comments.Edit;
                    DeletePolicyName = ProductSellingPermissions.Comments.Delete;
                    GetListPolicyName = ProductSellingPermissions.Comments.Default;
                }*/

        public async Task<CommentDto> CreateAsync(CreateCommentDto input)
        {
            var guid = Guid.NewGuid();
            var userId = _currentUser.Id;

            var comment = new Comment(
                guid,
                userId.Value,
                DateTime.Now,
                input.Content,
                input.EntityType,
                input.EntityId,
                input.ParentId

                );
            await _commentRepository.InsertAsync(comment);
            var commentDto = ObjectMapper.Map<Comment, CommentDto>(comment);

            return commentDto;
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _commentRepository.GetAsync(id);
            await _commentRepository.DeleteAsync(comment);
        }

        public async Task<CommentDto> GetAsync(Guid id)
        {
            var comment = await _commentRepository.GetAsync(id);
            return ObjectMapper.Map<Comment, CommentDto>(comment);
        }

        public async Task<List<CommentDto>> GetListAsync(CommentListDto input)
        {
            var comments = await _commentRepository.GetListAsync(c => c.EntityType == input.EntityType && c.EntityId == input.EntityId);

            var commentDtos = ObjectMapper.Map<List<Comment>, List<CommentDto>>(comments);
            var commentIds = comments.Select(c => c.Id).ToList();

            var commentLikes = await _commentLikeRepository.GetListAsync(l => commentIds.Contains(l.CommentId));

            var userId = CurrentUser.Id;

            // Fix: Get authorIds from comments, then use GetListByIdsAsync
            var authorIds = comments.Select(c => c.CreatorId).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();
            var authorsList = await _userRepository.GetListByIdsAsync(authorIds);
            var authors = authorsList.ToDictionary(u => u.Id, u => u.UserName);

            foreach (var commentDto in commentDtos)
            {
                commentDto.LikeCount = commentLikes.Count(l => l.CommentId == commentDto.Id);

                // Kiểm tra user hiện tại đã like chưa
                if (userId.HasValue)
                {
                    commentDto.IsLikedByCurrentUser = commentLikes.Any(l => l.CommentId == commentDto.Id && l.UserId == userId.Value);
                }

                // Gán tên tác giả
                if (commentDto.CreatorId.HasValue && authors.ContainsKey(commentDto.CreatorId.Value))
                {
                    commentDto.Commenter = authors[commentDto.CreatorId.Value];
                }
                else
                {
                    commentDto.Commenter = "Unknown";
                }
            }
            return commentDtos.OrderBy(c => c.CreationTime).ToList();
        }
        /*
                public async Task<CommentDto> UpdateAsync(Guid id, CreateCommentDto input)
                {
                    return await _commentRepository.UpdateAsync(, autoSave : true);
                }*/

        public async Task ToggleLikeAsync(Guid commentId)
        {
            var userId = _currentUser.Id.Value;

            var existingLike = await _commentLikeRepository.FindAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

            if (existingLike == null)
            {
                await _commentLikeRepository.InsertAsync(new Likes(commentId, userId));
            }
            else
            {
                // Nếu đã like -> Xóa like (unlike)
                await _commentLikeRepository.DeleteAsync(existingLike);
            }
        }
    }
}
