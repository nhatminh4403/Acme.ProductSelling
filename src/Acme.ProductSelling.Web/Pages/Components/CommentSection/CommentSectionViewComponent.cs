using Acme.ProductSelling.Comments;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.CommentSection
{
    public class CommentSectionViewComponent : AbpViewComponent
    {
        private readonly ICommentAppService _service;
        public CommentSectionViewComponent(ICommentAppService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string entityType, Guid entityId)
        {
            var getListDto = new CommentListDto { EntityType = entityType, EntityId = entityId };

            var comments = await _service.GetListAsync(getListDto);

            var model = new CommentSectionViewModel
            {
                EntityType = entityType,
                EntityId = entityId,
                Comments = comments
            };
            return View(model);
        }

    }
    public class CommentSectionViewModel
    {
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
