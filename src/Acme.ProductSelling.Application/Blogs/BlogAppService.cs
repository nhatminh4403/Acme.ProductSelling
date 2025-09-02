using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Acme.ProductSelling.Blogs
{
    public class BlogAppService : CrudAppService<Blog, BlogDto, Guid, PagedAndSortedResultRequestDto, CreateAndUpdateBlogDto>,
                                    IBlogAppService
    {
        private readonly IRepository<Blog, Guid> _repository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BlogAppService(IRepository<Blog, Guid> repository, IGuidGenerator guidGenerator, IWebHostEnvironment webHostEnvironment) : base(repository)
        {
            _repository = repository;
            _guidGenerator = guidGenerator;
            ConfigurePolicies();
            _webHostEnvironment = webHostEnvironment;
        }

        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            CreatePolicyName = ProductSellingPermissions.Blogs.Create;
            UpdatePolicyName = ProductSellingPermissions.Blogs.Edit;
            DeletePolicyName = ProductSellingPermissions.Blogs.Delete;
            GetListPolicyName = ProductSellingPermissions.Blogs.Default;
        }

        public virtual async Task<BlogDto> GetBlogBySlug(string slug)
        {
            var blog = await _repository.FirstOrDefaultAsync(b => b.UrlSlug == slug);
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }
            return ObjectMapper.Map<Blog, BlogDto>(blog);
        }
/*
        [Authorize(Roles = IdentityRoleConsts.Blogger, Policy = ProductSellingPermissions.Blogs.Create)]
        [Authorize(Roles = IdentityRoleConsts.Admin, Policy = ProductSellingPermissions.Blogs.Create)]*/
        public override async Task<BlogDto> CreateAsync(CreateAndUpdateBlogDto input)
        {
            var blogger = CurrentUser.UserName ?? "Anonymous";
            var blog = new Blog(
                _guidGenerator.Create(),
                input.Title,
                input.Content,
                input.PostedOn,
                blogger,
                input.Slug,
                input.ImageUrl,
                _guidGenerator.Create()
            );

            var newBlog = await _repository.InsertAsync(blog, autoSave: true);
            var result = ObjectMapper.Map<Blog, BlogDto>(newBlog);
            return result;
        }

        public override async Task DeleteAsync(Guid id)
        {
            var blogNeededToBeDeleted = await _repository.GetAsync(id);
            if (blogNeededToBeDeleted == null)
            {
                throw new Exception("Blog not found");
            }
            if (!string.IsNullOrEmpty(blogNeededToBeDeleted.MainImageUrl))
            {
                // BƯỚC 3: Tạo đường dẫn vật lý đầy đủ đến file ảnh.
                // ImageUrl có dạng "/blog-cover-images/file.jpg", cần loại bỏ dấu "/" ở đầu.
                var relativePath = blogNeededToBeDeleted.MainImageUrl.TrimStart('/');
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                // BƯỚC 4: Kiểm tra xem file có tồn tại không và xóa nó.
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            await Repository.DeleteAsync(id, autoSave: true);
        }

        public override async Task<BlogDto> UpdateAsync(Guid id, CreateAndUpdateBlogDto input)
        {
            var blog = await _repository.GetAsync(id);
            blog.Title = input.Title;
            blog.Content = input.Content;
            blog.PublishedDate = input.PostedOn;
            blog.UrlSlug = input.Slug;
            //input.ImageUrl = input.ImageUrl;

            var updatedBlog = await Repository.UpdateAsync(blog, autoSave: true);

            var result = ObjectMapper.Map<Blog, BlogDto>(updatedBlog);
            return result;
        }

    }
}
