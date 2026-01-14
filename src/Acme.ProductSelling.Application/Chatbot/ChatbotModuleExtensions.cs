using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Chatbot
{
    public static class ChatbotModuleExtensions
    {
        public static IServiceCollection AddChatbotServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure settings
            services.Configure<ChatbotSettings>(
                configuration.GetSection("Gemini"));

            // Services are auto-registered via ABP conventions
            // But you can explicitly register them here if needed:
            // services.AddTransient<ChatbotManager>();
            // services.AddTransient<GeminiApiService>();
            // services.AddTransient<IChatbotAppService, ChatbotAppService>();

            return services;
        }
    }
}
