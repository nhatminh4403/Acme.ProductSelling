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
        private static readonly HttpClient _httpClient = new HttpClient();
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
