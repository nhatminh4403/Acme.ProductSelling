using Acme.ProductSelling.Chatbot.Dtos;
using Acme.ProductSelling.Chatbot.Services;
using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Chatbot
{
    public class ChatbotAppService : ProductSellingAppService, IChatbotAppService
    {
        private readonly ChatbotManager _chatbotManager;
        private readonly GeminiApiService _geminiApiService;
        private readonly ICurrentUser _currentUser;
        public ChatbotAppService(ChatbotManager chatbotManager,
                                 GeminiApiService geminiApiService,
                                 ICurrentUser currentUser)
        {
            _chatbotManager = chatbotManager;
            _geminiApiService = geminiApiService;
            _currentUser = currentUser;
        }
        
        public async Task<ChatMessageOutputDto> SendMessageAsync(ChatMessageInputDto input)
        {
            var userRole = DetermineUserRole();
            var response = new ChatMessageOutputDto
            {
                UserRole = userRole
            };

            // Step 1: Search database first
            var products = await _chatbotManager.SearchProductsAsync(input.Message);
            var hasProductResults = products.Any();

            if (hasProductResults)
            {
                response.RelatedProducts = products.Select(MapToProductDto).ToList();
            }

            // Step 2: Build system context based on role
            var systemContext = BuildSystemContext(userRole, products);

            // Step 3: Prepare conversation history
            var conversationHistory = input.ConversationHistory?
                .Select(h => (h.Role, h.Content))
                .ToList();

            // Step 4: Generate AI response
            if (hasProductResults)
            {
                // Database results found
                response.Response = await _geminiApiService.GenerateContentAsync(
                    input.Message,
                    systemContext,
                    conversationHistory
                );
                response.Source = ResponseSource.Database;
            }
            else if (_chatbotManager.IsProductQuery(input.Message))
            {
                // Product query but no results - search web
                var searchInstructions = $"{systemContext}\n\nIMPORTANT: The internal database found NO products matching this query. Please use your Google Search tool to find current real-world information, prices, and reviews for '{input.Message}'.";
                response.Response = await _geminiApiService.GenerateContentAsync(
                    input.Message,
                    searchInstructions,
                    conversationHistory
                );
                response.Source = ResponseSource.WebSearch;
            }
            else
            {
                // General query - use AI with role context
                response.Response = await _geminiApiService.GenerateContentAsync(
                    input.Message,
                    systemContext,
                    conversationHistory
                );
                response.Source = ResponseSource.WebSearch;
            }

            return response;
        }
        private string DetermineUserRole()
        {
            if (!_currentUser.IsAuthenticated)
            {
                return "anonymous";
            }

            if (_currentUser.IsInRole("admin"))
            {
                return "admin";
            }

            if (_currentUser.IsInRole("staff") || _currentUser.IsInRole("manager"))
            {
                return "staff";
            }

            return "customer";
        }
        private string BuildSystemContext(string userRole, List<Product> products)
        {
            var context = "";

            // Role-specific instructions
            switch (userRole)
            {
                case "admin":
                case "staff":
                    context += "You are an AI assistant for Acme ProductSelling staff members. ";
                    context += "Provide detailed technical information, inventory insights, and administrative guidance. ";
                    context += "You can discuss internal operations, stock management, and pricing strategies. ";
                    break;

                case "customer":
                    context += "You are a helpful shopping assistant for Acme ProductSelling customers. ";
                    context += "Provide friendly product recommendations, answer questions about purchases, and help find the best deals. ";
                    context += "Be encouraging and focus on customer satisfaction. ";
                    break;

                case "anonymous":
                default:
                    context += "You are a friendly AI assistant for Acme ProductSelling. ";
                    context += "Help visitors learn about our products and encourage them to explore our catalog. ";
                    context += "Be welcoming and informative. ";
                    break;
            }

            // Add product context if available
            if (products.Any())
            {
                context += "\n\n" + _chatbotManager.BuildProductContext(products);
                context += "\n\nUse this product information to answer the user's question accurately.";

                if (userRole == "admin" || userRole == "staff")
                {
                    context += " Include stock counts and internal details.";
                }
                else
                {
                    context += " Focus on benefits and features that matter to shoppers.";
                }
            }

            return context;
        }
        private ChatbotProductDto MapToProductDto(Product product)
        {
            return new ChatbotProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                OriginalPrice = product.OriginalPrice,
                DiscountedPrice = product.DiscountedPrice,
                DiscountPercent = product.DiscountPercent,
                UrlSlug = product.UrlSlug,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name,
                ManufacturerName = product.Manufacturer?.Name,
                IsAvailableForPurchase = product.IsAvailableForPurchase(),
                TotalStock = product.StockCount
            };
        }
        public async Task<List<ChatbotProductDto>> SearchProductsAsync(string query)
        {
            var products = await _chatbotManager.SearchProductsAsync(query, maxResults: 10);
            return products.Select(MapToProductDto).ToList();
        }
    }
}
