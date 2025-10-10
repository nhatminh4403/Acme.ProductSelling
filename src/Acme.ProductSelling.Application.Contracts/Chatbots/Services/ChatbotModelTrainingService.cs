using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Chatbots.Services
{

    public class ChatbotModelTrainingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ChatbotModelTrainingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var intentClassifier = scope.ServiceProvider
                .GetRequiredService<IntentClassifierService>();

            await intentClassifier.TrainModelAsync();
        }
    }

}
