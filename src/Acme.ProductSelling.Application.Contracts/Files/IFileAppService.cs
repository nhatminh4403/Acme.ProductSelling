using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Files
{
    public interface IFileAppService : IApplicationService
    {
        Task<FileUploadResultDto> UploadImageAsync(IFormFile file, bool replaceIfExists);
    }
}
