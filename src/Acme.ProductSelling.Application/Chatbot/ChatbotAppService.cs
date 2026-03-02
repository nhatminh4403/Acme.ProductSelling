using Acme.ProductSelling.Chatbot.Dtos;
using Acme.ProductSelling.Chatbot.Services;
using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Products;
using System.Collections.Generic;
using System.Globalization;
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

            //  Search database first
            var products = await _chatbotManager.SearchProductsAsync(input.Message);
            var hasProductResults = products.Any();

            if (hasProductResults)
            {
                response.RelatedProducts = products.Select(MapToProductDto).ToList();
            }

            //  Build system context based on role
            var systemContext = BuildSystemContext(userRole, products);

            //  Prepare conversation history
            var conversationHistory = input.ConversationHistory?
                .Select(h => (h.Role, h.Content))
                .ToList();

            // Generate AI response
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
            if (_currentUser.IsInRole(IdentityRoleConsts.WarehouseStaff)) return "warehouse";
            if (_currentUser.IsInRole(IdentityRoleConsts.Seller) || _currentUser.IsInRole(IdentityRoleConsts.Cashier)) return "sales_agent";

            return "customer";
        }
        private string BuildSystemContext(string userRole, List<Product> products)
        {
            var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var currentUILanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var currentDisplayLanguage = CultureInfo.CurrentCulture.DisplayName;
            var currentUIDisplayLanguage = CultureInfo.CurrentUICulture.DisplayName;

            var selectedLanguage = !string.IsNullOrWhiteSpace(currentLanguage) ? currentLanguage :
                                    !string.IsNullOrWhiteSpace(currentUILanguage) ? currentUILanguage :
                                    !string.IsNullOrWhiteSpace(currentDisplayLanguage) ? currentDisplayLanguage : currentUIDisplayLanguage;

            var sb = new StringBuilder();


            sb.AppendLine($"You are the 'XComputer AI Assistant'. \n" +
                              $"IMPORTANT: You MUST always respond to the user in {selectedLanguage}. " +
                              $"Translate any technical database data into {selectedLanguage} naturally.");

            if (userRole == "customer" || userRole == "anonymous")
            {
                sb.AppendLine("ROLE: You are a friendly, knowledgeable store assistant.");
                sb.AppendLine("IMPORTANT GUIDELINES FOR CUSTOMERS:");
                sb.AppendLine("1. NEVER use words like 'database', 'system record', 'json', 'id', 'slug', or 'fetched'.");
                sb.AppendLine("2. Act as if you are standing in the store looking at the products on the shelf.");
                sb.AppendLine("3. Use the prices provided exactly (in VND/₫). Do not convert currencies.");
                sb.AppendLine("4. DIFFERENTIATION: If the user asks for a 'Mouse', strictly ignore 'Mouse Pads' unless they ask for them.");
                sb.AppendLine("5. DIFFERENTIATION: If a Laptop is marked [GAMING MACHINE], emphasize its GPU. If [OFFICE], emphasize portability.");
            }
            else
            {
                // Admin/Staff logic (Keep previous internal logic)
                sb.AppendLine($"ROLE: Internal System Assistant for Role: {userRole}.");
                sb.AppendLine("You may reference database IDs and stock levels.");
                sb.Append(_chatbotManager.GetRoleKnowledgeBase(userRole)); // Only add secrets for staff
            }
            if (userRole == "customer" && products.Count > 1) // Only if we actually have data
            {
                var distinctBrands = products
                    .Where(p => p.Manufacturer != null)
                    .Select(p => p.Manufacturer.Name)
                    .Distinct()
                    .Take(5) // Limit context
                    .ToList();

                sb.AppendLine("\nGUIDE FOR LISTING PRODUCTS:");
                sb.AppendLine("If the user's request was vague (e.g., just 'Laptop', 'Computer', 'Mouse')...");
                sb.AppendLine("DO NOT dump a list of products immediately.");
                sb.AppendLine($"INSTEAD: 1. Mention the brands we have ({string.Join(", ", distinctBrands)}).");
                sb.AppendLine("         2. Ask if they are looking for 'Budget' (Low), 'Mainstream' (Mid), or 'High-End' options.");
                sb.AppendLine("         3. Ask them about their specific use case (e.g., Gaming vs Office).");
                sb.AppendLine("ONLY list specific products if the user asked for a specific feature, price, or brand.");
            }

            sb.AppendLine("\nCURRENT SHELF INVENTORY (Use this data to answer):");
            sb.Append(_chatbotManager.BuildProductContext(products, userRole));

            return sb.ToString();
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
            var products = await _chatbotManager.SearchProductsAsync(query, maxResults: 3);
            return products.Select(MapToProductDto).ToList();
        }

        public async Task<List<string>> GetAvailableAsync()
        {
            return await _geminiApiService.GetAvailableModelsAsync();
        }
    }
}
