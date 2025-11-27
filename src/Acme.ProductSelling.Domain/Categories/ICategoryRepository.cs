using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<Category> FindByNameAsync(string name);
        Task<List<Category>> GetListAsync();
        Task<Category> GetByIdAsync(Guid id);
        Task<Category> GetBySlugAsync(string slug);
        Task<List<Category>> GetListByGroupAsync(CategoryGroup categoryGroup);
        Task<Dictionary<CategoryGroup, List<Category>>> GetGroupedCategoriesAsync();
    }
}