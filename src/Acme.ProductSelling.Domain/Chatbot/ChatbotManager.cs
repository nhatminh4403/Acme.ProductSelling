using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Acme.ProductSelling.Chatbot
{
    public class ChatbotManager : DomainService
    {
        private readonly IProductRepository _productRepository;

        public ChatbotManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<Product>> SearchProductsAsync(string query, int maxResults = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Product>();
            }

            var queryable = await _productRepository.GetQueryableAsync();

            var searchTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Search in product name, description, category, and manufacturer
            var products = await queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Where(p => searchTerms.Any(term =>
                    p.ProductName.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term) ||
                    p.Manufacturer.Name.ToLower().Contains(term)
                ))
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.StockCount > 0)
                .Take(maxResults)
                .ToListAsync();

            return products;
        }

        public bool IsProductQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;

            var productKeywords = new[]
            {
                "product", "buy", "price", "cost", "laptop", "mouse", "keyboard",
                "monitor", "cpu", "gpu", "ram", "storage", "available", "stock",
                "purchase", "gaming", "computer", "pc", "specifications", "specs"
            };

            var lowerQuery = query.ToLower();
            return productKeywords.Any(keyword => lowerQuery.Contains(keyword));
        }

        public string BuildProductContext(List<Product> products)
        {
            if (!products.Any())
                return string.Empty;

            var context = "Available products in our database:\n";

            foreach (var product in products)
            {
                var price = product.DiscountedPrice ?? product.OriginalPrice;
                var availability = product.IsAvailableForPurchase() ? "In Stock" : "Out of Stock";
                var safeDesc = 
                    product.Description?.Length > 150
                        ? string.Concat(product.Description.AsSpan(0, 47), "...") : product.Description;

                context += $"\n- {product.ProductName}";
                context += $"\n  Category: {product.Category?.Name ?? "N/A"}";
                context += $"\n  Manufacturer: {product.Manufacturer?.Name ?? "N/A"}";
                context += $"\n  Price: {price:C}";
                if (product.DiscountPercent > 0)
                {
                    context += $" (Original: {product.OriginalPrice:C}, {product.DiscountPercent}% off)";
                }
                context += $"\n  Status: {availability}";
                context += $"\n  Description: {safeDesc}";
                context += "\n";
            }

            return context;
        }
    }
}
