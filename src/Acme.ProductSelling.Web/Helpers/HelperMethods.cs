using Acme.ProductSelling.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;

namespace Acme.ProductSelling.Web.Helpers
{
    public static class HelperMethods
    {
        public static string GetCategoryLocalizerName(string categoryName, IStringLocalizer<ProductSellingResource> localizer = null)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return localizer["UnknownCategory"];
            }
            string trimmedName = String.Concat(categoryName.Where(c => !Char.IsWhiteSpace(c)));

            if (localizer == null)
            {
                return categoryName;
            }
            var localizedName = localizer["Category:" + trimmedName];
            return localizedName.ResourceNotFound ? categoryName : localizedName;
        }
    }
}
