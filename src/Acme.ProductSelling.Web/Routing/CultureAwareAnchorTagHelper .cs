using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System.Globalization;

namespace Acme.ProductSelling.Web.Routing
{
    [HtmlTargetElement("a", Attributes = "asp-page")]
    [HtmlTargetElement("a", Attributes = "asp-controller")]
    [HtmlTargetElement("a", Attributes = "asp-action")]
    [HtmlTargetElement("a", Attributes = "asp-route-culture")]
    [HtmlTargetElement("a", Attributes = "asp-route-*")]
    [HtmlTargetElement("href", Attributes = "asp-route-*")]
    [HtmlTargetElement("href", Attributes = "asp-page")]
    [HtmlTargetElement("href", Attributes = "asp-controller")]
    [HtmlTargetElement("href", Attributes = "asp-action")]

    public class CultureAwareAnchorTagHelper : TagHelper
    {
       
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            // Nếu chưa có asp-route-culture thì tự động thêm
            if (!output.Attributes.ContainsName("asp-route-culture"))
            {
                output.Attributes.SetAttribute("asp-route-culture", culture);
            }
        }
    }
}
