using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Web.Services.ImageValidation
{
    public interface IImageValidationService : ITransientDependency
    {
        Task<bool> IsUrlValidAsync(string imageUrl);
    }
}
