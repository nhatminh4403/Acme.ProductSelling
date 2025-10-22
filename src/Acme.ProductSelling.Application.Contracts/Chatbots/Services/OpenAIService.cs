using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
namespace Acme.ProductSelling.Chatbots.Services
{
    public class OpenAIService : ITransientDependency
    {
        private readonly ChatClient _chatClient;
        private readonly string _apiKey;
        public OpenAIService(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
            _chatClient = new ChatClient("gpt-3.5-turbo", _apiKey);
        }
        public async Task<string> GenerateResponseAsync(
        string intent,
        string userMessage,
        string contextData)
        {
            var systemPrompt = BuildSystemPrompt(intent, contextData);
            var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userMessage)
        };
            var response = await _chatClient.CompleteChatAsync(messages);
            return response.Value.Content[0].Text;
        }
        private string BuildSystemPrompt(string intent, string contextData)
        {
            return intent switch
            {
                "product_query" => $@"You are a helpful product assistant. 
                Here is our product catalog:
                {contextData}
                Provide detailed, friendly product information based on the user's question.
                Keep responses concise but informative.",
                "order_query" => $@"You are a helpful order tracking assistant.
                Here is the order information:
                {contextData}
                Provide clear order status updates. Be empathetic and professional.",
                "greeting" => @"You are a friendly customer service chatbot. 
                Greet the user warmly and let them know you can help with products, 
                orders, and general inquiries.",
                "support" => @"You are a helpful support assistant. 
                Acknowledge their issue and provide guidance on how to get help.
                Be empathetic and professional.",
                "goodbye" => @"You are a friendly chatbot. 
                Say a warm goodbye and invite them to return.",
                _ => @"You are a helpful assistant. 
                Answer the user's question professionally and concisely."
            };
        }
    }
}
