using Acme.ProductSelling.Chatbots.Dtos;
using Acme.ProductSelling.Chatbots.ML;
using Acme.ProductSelling.Chatbots.Services;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Chatbots
{
    public class ChatbotAppService : ApplicationService, IChatbotAppService
    {
        private readonly IntentClassifierService _intentClassifier;
        private readonly OpenAIService _openAIService;
        private readonly IRepository<ChatbotMessage> _messageRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;

        public ChatbotAppService(IntentClassifierService intentClassifier, OpenAIService openAIService, IRepository<ChatbotMessage> messageRepository, IRepository<Product> productRepository, IRepository<Order> orderRepository)
        {
            _intentClassifier = intentClassifier;
            _openAIService = openAIService;
            _messageRepository = messageRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        #region Normal Chatbot
        public async Task<ChatResponseDto> SendMessageAsync(SendMessageInput input)
        {
            var prediction = _intentClassifier.PredictIntent(input.Message);
            var confidence = prediction.Score.Max();

            // 2. Fetch relevant data from database based on intent
            var contextData = await GetContextDataAsync(prediction.Intent, input.Message);

            // 3. Generate response using OpenAI
            var response = await _openAIService.GenerateResponseAsync(
                prediction.Intent,
                input.Message,
                contextData);

            // 4. Save conversation to database
            await SaveConversationAsync(input, prediction, response);

            // 5. Return response
            return new ChatResponseDto
            {
                Message = response,
                Intent = prediction.Intent,
                Confidence = confidence,
                Timestamp = Clock.Now,
                Suggestions = GetSuggestions(prediction.Intent)
            };
        }

        private async Task<string> GetContextDataAsync(string intent, string message)
        {
            return intent switch
            {
                "product_query" => await GetProductContextAsync(message),
                "order_query" => await GetOrderContextAsync(message),
                _ => string.Empty
            };
        }

        private async Task<string> GetProductContextAsync(string message)
        {
            var products = await _productRepository.GetListAsync();

            // Simple filtering based on message content
            var relevantProducts = products
                .Where(p => message.ToLower().Contains(p.ProductName.ToLower()))
                .ToList();

            if (!relevantProducts.Any())
            {
                relevantProducts = products.Take(5).ToList();
            }

            return string.Join("\n", relevantProducts.Select(p =>
                $"- {p.ProductName}: ${p.DiscountedPrice} (Stock: {p.StockCount}, Category: {p.Category.Name})"));
        }

        private async Task<string> GetOrderContextAsync(string message)
        {
            // Extract order ID from message
            var match = System.Text.RegularExpressions.Regex.Match(message, @"\d+");
            var queryable = await _orderRepository.GetQueryableAsync();
            if (match.Success && Guid.TryParse(match.Value, out Guid orderId))
            {

                var order = await queryable
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order != null)
                {
                    return $@"Order #{order.Id}
                    Status: {order.Status}
                    Total: ${order.TotalAmount}
                    Date: {order.CreationTime:MMM dd, yyyy}
                    Items: {string.Join(", ", order.OrderItems.Select(i => i.ProductName))}";
                }
            }

            // If no specific order, get user's recent orders
            if (CurrentUser.IsAuthenticated)
            {
                var recentOrders = await queryable
                    .Where(o => o.CustomerId == CurrentUser.Id)
                    .OrderByDescending(o => o.CreationTime)
                    .Take(3)
                    .ToListAsync();

                return string.Join("\n", recentOrders.Select(o =>
                    $"Order #{o.Id}: {o.Status} - ${o.TotalAmount}"));
            }

            return "No order information available.";
        }

        private async Task SaveConversationAsync(
            SendMessageInput input,
            IntentPrediction prediction,
            string response)
        {
            var message = new ChatbotMessage
            {
                UserMessage = input.Message,
                BotResponse = response,
                DetectedIntent = prediction.Intent,
                IntentConfidence = prediction.Score.Max(),
                SessionId = input.SessionId,
                UserId = CurrentUser.Id,
                IsAdminChat = input.IsAdminChat
            };

            await _messageRepository.InsertAsync(message);
        }

        private List<string> GetSuggestions(string intent)
        {
            return intent switch
            {
                "greeting" => new List<string>
            {
                "Show me products",
                "Track my order",
                "Contact support"
            },
                "product_query" => new List<string>
            {
                "Show more products",
                "Check prices",
                "View categories"
            },
                "order_query" => new List<string>
            {
                "Cancel order",
                "Modify order",
                "Return policy"
            },
                _ => new List<string>()
            };
        }
        #endregion

        #region Admin

        [Authorize(ProductSellingPermissions.Chatbots.Admin)]
        public async Task TrainModelAsync()
        {
            await _intentClassifier.TrainModelAsync();
        }
        [Authorize(ProductSellingPermissions.Chatbots.Admin)]
        public async Task<List<ChatbotMessage>> GetConversationHistoryAsync(int maxCount = 100)
        {
            var queryable = await _messageRepository.GetQueryableAsync();
            return await queryable
            .OrderByDescending(m => m.CreationTime)
            .Take(maxCount)
            .ToListAsync();
        }

        #endregion

    }
}
