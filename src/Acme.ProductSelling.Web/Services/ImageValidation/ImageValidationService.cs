using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Services.ImageValidation
{
    public class ImageValidationService : IImageValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public ImageValidationService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<bool> IsUrlValidAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return false;
            }

            // Use the URL itself as the cache key
            string cacheKey = $"ImageUrlValid_{imageUrl}";

            // Try to get the result from the cache first
            // If it's not there, the factory delegate will be executed
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                // Set how long the result should be cached
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                try
                {
                    using var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var request = new HttpRequestMessage(HttpMethod.Head, imageUrl);
                    var response = await client.SendAsync(request);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception)
                {
                    // If it fails, cache the failure so we don't retry immediately
                    return false;
                }
            });
        }
    }
}
