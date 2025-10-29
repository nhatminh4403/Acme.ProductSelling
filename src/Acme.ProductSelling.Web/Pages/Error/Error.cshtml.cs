using Acme.ProductSelling.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Error
{
    public class ErrorModel : ProductSellingPageModel
    {
        [BindProperty(SupportsGet = true)]
        public int StatusCode { get; set; }

        public string ErrorTitle { get; private set; }
        public string ErrorMessage { get; private set; }

        public void OnGet()
        {
            ErrorTitle = L[$"Error:{StatusCode}:Title"];
            ErrorMessage = L[$"Error:{StatusCode}:Message"];
        }
    }
}