
using Acme.ProductSelling.Chatbots.ML;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Chatbots.Services
{
    public class IntentClassifierService : ITransientDependency
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<IntentData, IntentPrediction> _predictionEngine;
        private readonly IRepository<ChatbotTrainingData> _trainingDataRepository;
        public IntentClassifierService(IRepository<ChatbotTrainingData> trainingDataRepository)
        {
            _mlContext = new MLContext(seed: 0);
            _trainingDataRepository = trainingDataRepository;
        }
        public async Task TrainModelAsync()
        {
            // Get training data from database
            var queryable = await _trainingDataRepository.GetQueryableAsync();
            var trainingData = await queryable
                .Where(t => t.IsVerified)
                .Select(t => new IntentData
                {
                    Message = t.Message,
                    Intent = t.Intent
                })
                .ToListAsync();

            // Add default training data if database is empty
            if (!trainingData.Any())
            {
                trainingData = GetDefaultTrainingData();
            }

            // Load data
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Split data
            var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            // Build training pipeline
            var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey("Label", "Intent")
                .Append(_mlContext.Transforms.Text.FeaturizeText("Features", "Message"))
                .Append(_mlContext.MulticlassClassification.Trainers
                    .SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Train model
            _model = pipeline.Fit(splitData.TrainSet);

            // Create prediction engine
            _predictionEngine = _mlContext.Model
                .CreatePredictionEngine<IntentData, IntentPrediction>(_model);

            // Evaluate model
            var predictions = _model.Transform(splitData.TestSet);
            var metrics = _mlContext.MulticlassClassification.Evaluate(predictions);

            Console.WriteLine($"Model Accuracy: {metrics.MacroAccuracy:P2}");
        }

        public IntentPrediction PredictIntent(string message)
        {
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("Model not trained. Call TrainModelAsync first.");
            }

            var input = new IntentData { Message = message };
            return _predictionEngine.Predict(input);
        }

        public async Task SaveModelAsync(string path)
        {
            _mlContext.Model.Save(_model, null, path);
        }

        public async Task LoadModelAsync(string path)
        {
            _model = _mlContext.Model.Load(path, out var schema);
            _predictionEngine = _mlContext.Model
                .CreatePredictionEngine<IntentData, IntentPrediction>(_model);
        }

        private List<IntentData> GetDefaultTrainingData()
        {
            return new List<IntentData>
        {
            // Greetings
            new IntentData { Message = "hello", Intent = "greeting" },
            new IntentData { Message = "hi there", Intent = "greeting" },
            new IntentData { Message = "good morning", Intent = "greeting" },
            new IntentData { Message = "hey", Intent = "greeting" },
            
            // Product queries
            new IntentData { Message = "show me products", Intent = "product_query" },
            new IntentData { Message = "what products do you have", Intent = "product_query" },
            new IntentData { Message = "tell me about your items", Intent = "product_query" },
            new IntentData { Message = "product information", Intent = "product_query" },
            new IntentData { Message = "how much does it cost", Intent = "product_query" },
            new IntentData { Message = "price of laptop", Intent = "product_query" },
            
            // Order queries
            new IntentData { Message = "track my order", Intent = "order_query" },
            new IntentData { Message = "where is my order", Intent = "order_query" },
            new IntentData { Message = "order status", Intent = "order_query" },
            new IntentData { Message = "check order 12345", Intent = "order_query" },
            new IntentData { Message = "my purchase history", Intent = "order_query" },
            
            // Support
            new IntentData { Message = "I need help", Intent = "support" },
            new IntentData { Message = "contact support", Intent = "support" },
            new IntentData { Message = "talk to human", Intent = "support" },
            new IntentData { Message = "customer service", Intent = "support" },
            
            // Goodbye
            new IntentData { Message = "bye", Intent = "goodbye" },
            new IntentData { Message = "goodbye", Intent = "goodbye" },
            new IntentData { Message = "see you later", Intent = "goodbye" },
            new IntentData { Message = "thanks bye", Intent = "goodbye" }
        };
        }
    }
}
