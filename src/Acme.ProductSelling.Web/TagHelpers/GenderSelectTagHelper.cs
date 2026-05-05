using Acme.ProductSelling.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Acme.ProductSelling.Web.TagHelpers
{
    [HtmlTargetElement("gender-select", Attributes = "asp-for")]
    public class GenderSelectTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;

            var name = For.Name;
            var id = TagBuilder.CreateSanitizedId(name, "_");
            var currentValue = For.Model?.ToString() ?? UserGender.NONE.ToString();

            output.Attributes.SetAttribute("name", name);
            output.Attributes.SetAttribute("id", id);
            output.Attributes.SetAttribute("class", "form-select");

            var options = new List<(string Value, string Label)>
            {
                (UserGender.NONE.ToString(),   "—"),
                (UserGender.MALE.ToString(),   "Male"),
                (UserGender.FEMALE.ToString(), "Female"),
            };

            var html = new System.Text.StringBuilder();
            foreach (var (value, label) in options)
            {
                var selected = value == currentValue ? " selected" : "";
                html.Append($"<option value=\"{value}\"{selected}>{label}</option>");
            }

            output.Content.SetHtmlContent(html.ToString());
        }
    }
}
