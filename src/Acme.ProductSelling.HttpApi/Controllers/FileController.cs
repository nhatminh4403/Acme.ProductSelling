using Acme.ProductSelling.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Controllers
{
    [Route("api/files")]
    public class FileController : AbpController
    {
        private readonly IFileAppService _fileAppService;

        public FileController(IFileAppService fileAppService)
        {
            _fileAppService = fileAppService;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            try
            {
                var result = await _fileAppService.UploadImageAsync(upload,true);
                return Ok(new { url = result.Url });
            }
            catch (UserFriendlyException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
