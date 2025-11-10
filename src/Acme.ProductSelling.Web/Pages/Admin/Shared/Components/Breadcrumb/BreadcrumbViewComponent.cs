using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Shared.Components.Breadcrumb
{
    public class BreadcrumbViewComponent : AbpViewComponent
    {
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

        public BreadcrumbViewComponent(IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var breadcrumbs = new List<BreadcrumbItem>();
            var path = HttpContext.Request.Path.ToString();
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                // No need for breadcrumbs on the root page
                return View(breadcrumbs);
            }

            // --- NEW: Identify and handle the culture prefix ---
            var supportedCultures = _localizationOptions.Value.SupportedCultures?.Select(c => c.Name).ToHashSet();
            string culturePrefix = string.Empty;
            int startIndex = 0;

            if (supportedCultures != null && supportedCultures.Contains(segments[0], StringComparer.OrdinalIgnoreCase))
            {
                culturePrefix = $"/{segments[0]}";
                startIndex = 1; // Start processing segments *after* the culture code
            }

            // The root/dashboard link is always the starting point
            breadcrumbs.Add(new BreadcrumbItem { Text = "Dashboard", Url = $"{culturePrefix}/" });

            // Initialize currentPath with the culture prefix to build correct links
            var currentPath = new StringBuilder(culturePrefix);

            // --- UPDATED: Loop starts from the correct index ---
            for (int i = startIndex; i < segments.Length; i++)
            {
                var segment = segments[i];

                if (Guid.TryParse(segment, out _) || int.TryParse(segment, out _) || IsCommonAction(segment))
                {
                    continue;
                }

                currentPath.Append($"/{segment}");

                // The last segment is the active page
                bool isActive = (i == segments.Length - 1);

                breadcrumbs.Add(new BreadcrumbItem
                {
                    Text = FormatBreadcrumbText(segment),
                    Url = isActive ? null : currentPath.ToString(),
                    IsActive = isActive
                });
            }
            // This is a simple async placeholder; real async logic could go here if needed.
            return View("/Pages/Admin/Shared/Components/Breadcrumb/Default.cshtml", breadcrumbs);
        }

        private string FormatBreadcrumbText(string segment)
        {
            if (string.IsNullOrEmpty(segment)) return string.Empty;
            var text = segment.Replace("-", " ");
            text = Regex.Replace(text, "(?<!^)([A-Z])", " $1");
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
        }

        private bool IsCommonAction(string segment)
        {
            string lowerSegment = segment.ToLowerInvariant();
            var actions = new[] { "edit", "details", "create", "delete", "index", "update", "manage" };
            return Array.Exists(actions, action => action == lowerSegment);
        }
    }

}
