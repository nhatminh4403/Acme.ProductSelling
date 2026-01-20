using Acme.ProductSelling.Chatbot.Dtos;
using Acme.ProductSelling.Chatbot.Services;
using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Linq;
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
            bool isReportRequest = (userRole == "admin" || userRole == "manager") &&
                          (input.Message.ToLower().Contains("report") ||
                           input.Message.ToLower().Contains("summary") ||
                           input.Message.ToLower().Contains("analysis"));
            if (isReportRequest)
            {
                // 1. Fetch RAW data (The "Fetch")
                var dataJson = await _chatbotManager.GetSalesSummaryJsonAsync();

                // 2. Build Analyst Prompt (The "Think")
                var prompt = $@"
                        ROLE: You are an Executive Data Analyst for Acme Corp.
                        TASK: Interpret the following raw JSON database statistics and write a generic textual report for the {userRole}.
                        CONTEXT: The user asked: '{input.Message}'
                        RAW DATA: {dataJson}
            
                        GUIDANCE:
                        - If revenue is high, be congratulatory.
                        - If stock alerts exist, recommend ordering immediately.
                        - Format with bold headings.
                    ";

                // 3. Send to Gemini
                response.Response = await _geminiApiService.GenerateContentAsync(prompt);
                response.Source = ResponseSource.Database;
                return response;
            }

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

            if (_currentUser.IsInRole(IdentityRoleConsts.Admin)) return "admin";
            if (_currentUser.IsInRole(IdentityRoleConsts.Manager)) return "manager";
            if (_currentUser.IsInRole(IdentityRoleConsts.Blogger)) return "blogger";
            if (_currentUser.IsInRole(IdentityRoleConsts.WarehouseStaff)) return "warehouse";
            if (_currentUser.IsInRole(IdentityRoleConsts.Seller) || _currentUser.IsInRole(IdentityRoleConsts.Cashier)) return "sales_agent";

            return "customer";
        }
        private string BuildSystemContext(string userRole, List<Product> products)
        {
            var baseContext = "You are the 'Acme AI Assistant', an intelligent enterprise tool for Acme ProductSelling.";

            switch (userRole)
            {
                case "admin":
                case "manager":
                    baseContext += " Your role: Senior Supervisor.";
                    baseContext += " You have access to confidential insights. You help with decision making, analyzing sales trends, and spotting operational issues.";
                    baseContext += " Tone: Professional, concise, data-driven.";
                    break;

                case "sales_agent": // Sellers and Cashiers
                    baseContext += " Your role: In-Store POS Assistant.";
                    baseContext += " You assist cashiers and sellers in real-time. Your priorities are speed, checking current stock, applying bulk discounts, and cross-selling.";
                    baseContext += " Tone: Efficient, energetic, customer-focused (proxy for the agent).";
                    break;

                case "warehouse":
                    baseContext += " Your role: Inventory & Logistics Specialist.";
                    baseContext += " You assist warehouse staff with bin locations, restocking levels, and shipment queries. Focus on physical dimensions, weights, and SKU tracking.";
                    baseContext += " Tone: Precise, technical, safety-oriented.";
                    break;

                case "blogger":
                    baseContext += " Your role: Content Marketing Assistant.";
                    baseContext += " You help write catchy product descriptions, social media posts, and SEO tags. Focus on key features, benefits, and 'selling the dream' rather than technical specs.";
                    baseContext += " Tone: Creative, engaging, persuasive.";
                    break;

                case "customer":
                default:
                    baseContext += " Your role: Helpful Shopping Assistant. Help the user find the perfect product.";
                    break;
            }


            baseContext += "\n\nPRODUCT DATA:\n" + _chatbotManager.BuildProductContext(products, userRole);

            return baseContext;
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
