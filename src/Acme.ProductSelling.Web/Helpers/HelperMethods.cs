using Acme.ProductSelling.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Helpers
{
    public static class HelperMethods
    {
        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
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
        public static string GetProductLocalizerName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                return "Unknown Product";
            }
            return productName.Replace(" ", "_").ToLowerInvariant();
        }

        public static async Task<bool> IsImageUrlValidAsync(string imageUrl)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, imageUrl);
                var response = await _httpClient.SendAsync(request);

                // Trả về true nếu mã trạng thái là 200 OK
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {

                return false;
            }
        }
    }
}
