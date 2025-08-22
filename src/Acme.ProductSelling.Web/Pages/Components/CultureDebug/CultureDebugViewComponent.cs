using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.CultureDebug
{
    public class CultureDebugViewComponent : AbpViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;
        public CultureDebugViewComponent(IHttpContextAccessor httpContextAccessor, IOptions<RequestLocalizationOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizationOptions = options;
        }
        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                // Trả về view trống nếu không có HttpContext
                return View("~/Pages/Components/CultureDebug/Default.cshtml", new CultureDebugModel());
            }

            // Lấy thông tin culture từ feature của request - đây là cách chính xác nhất
            var requestCultureFeature = httpContext.Features.Get<IRequestCultureFeature>();

            var model = new CultureDebugModel
            {
                // Lấy culture cuối cùng được áp dụng cho thread
                CurrentCulture = CultureInfo.CurrentCulture.Name,
                CurrentUICulture = CultureInfo.CurrentUICulture.Name,
                RequestPath = httpContext.Request.Path.Value,

                // Lấy tên của provider đã quyết định culture
                ProviderName = requestCultureFeature?.Provider?.GetType().Name ?? "N/A",

                // Lấy danh sách các culture đã được cấu hình trong hệ thống
                SupportedCultures = string.Join(", ", _localizationOptions.Value.SupportedCultures.Select(c => c.Name)),

                // Lấy tất cả các cookie quan trọng
                RelevantCookies = new Dictionary<string, string>
                {
                    {
                        CookieRequestCultureProvider.DefaultCookieName, // Thường là ".AspNetCore.Culture"
                        httpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName] ?? "Not found"
                    },
                    {
                        "Abp.Localization.CultureName", // Cookie của ABP
                        httpContext.Request.Cookies["Abp.Localization.CultureName"] ?? "Not found"
                    },
                    {
                        "culture", // Cookie tùy chỉnh của bạn
                        httpContext.Request.Cookies["culture"] ?? "Not found"
                    }
                }
            };

            return View("~/Pages/Components/CultureDebug/Default.cshtml", model);
        }
    }
    public class CultureDebugModel
    {
        public string CurrentCulture { get; set; }
        public string CurrentUICulture { get; set; }
        public string RequestPath { get; set; }

        // MỚI: Tên của Provider đã xác định culture (ví dụ: RouteValueRequestCultureProvider)
        public string ProviderName { get; set; }

        // MỚI: Danh sách các cookie liên quan đến culture
        public Dictionary<string, string> RelevantCookies { get; set; }

        // MỚI: Các culture được hỗ trợ trong hệ thống
        public string SupportedCultures { get; set; }
    }
}
