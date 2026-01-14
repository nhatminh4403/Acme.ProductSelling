using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Chatbot
{
    public class GeminiApiService : ITransientDependency
    {
        private readonly Client _client;
        private readonly ChatbotSettings _settings;


        public GeminiApiService(IOptions<ChatbotSettings> settings)
        {
            _settings = settings.Value;

            // Initialize official Google GenAI client
            _client = new Client(apiKey: _settings.ApiKey);
        }
        public async Task<string> GenerateContentAsync(string userMessage,
                                                       string systemContext = null,
                                                       List<(string role, string content)> conversationHistory = null)
        {
            try
            {
                var tools = new List<Tool>();

                // If Web Search is enabled in settings, give Gemini the "Google Search" tool
                if (_settings.EnableWebSearch)
                {
                    tools.Add(new Tool
                    {
                        GoogleSearch = new GoogleSearch()
                    });
                }
                var contents = new List<Content>();
                if (conversationHistory != null)
                {
                    foreach (var item in conversationHistory)
                    {
                        // Map "user" -> "user", "assistant" -> "model" for Gemini
                        var apiRole = item.role == "user" ? "user" : "model";
                        contents.Add(new Content
                        {
                            Role = apiRole,
                            Parts = new List<Part> { new Part { Text = item.content } }
                        });
                    }
                }

                var finalMessage = userMessage;
                if (!string.IsNullOrEmpty(systemContext))
                {
                    // Prepended system instruction is standard for one-shot RAG
                    finalMessage = $"[SYSTEM INSTRUCTION: {systemContext}]\n\n[USER QUERY]: {userMessage}";
                }
                contents.Add(new Content
                {
                    Role = "user",
                    Parts = new List<Part> { new Part { Text = finalMessage } }
                });

                // Build the full prompt with context

                // Create generation configuration
                var config = new GenerateContentConfig
                {
                    Temperature = _settings.Temperature,
                    MaxOutputTokens = _settings.MaxTokens,
                    Tools = tools.Any() ? tools : null,
                    SystemInstruction = !string.IsNullOrEmpty(systemContext) ? new Content { Parts = new List<Part> { new Part { Text = systemContext } } } : null
                };

                // Generate content using official SDK
                var response = await _client.Models.GenerateContentAsync(
                    model: _settings.Model,
                    contents: contents,
                    config: config
                );

                // Extract text from response
                var textResponse = "";
                if (response.Candidates != null && response.Candidates.Any())
                {
                    var candidate = response.Candidates[0];

                    // Check for Web Search Results in the logs (optional debugging)
                    // var grounding = candidate.GroundingMetadata; 

                    var textParts = candidate.Content.Parts
                            .Where(p => !string.IsNullOrEmpty(p.Text))
                            .Select(p => p.Text);

                    var finalText = string.Join("\n", textParts);

                    // Add a tiny footnote if grounding was used (Optional UI touch)
                    /* 
                    if (candidate.GroundingMetadata?.SearchEntryPoint != null) {
                        finalText += "\n\n(Source: Google Search)";
                    } 
                    */

                    return finalText;
                }

                return "I apologize, but I couldn't generate a response.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Gemini Search Error: {ex.Message}. Check if Model supports tools.", ex);
            }
        }

        /// <summary>
        /// Generate content with streaming support
        /// </summary>
        public async IAsyncEnumerable<string> GenerateContentStreamAsync(
            string userMessage,
            string systemContext = null,
            List<(string role, string content)> conversationHistory = null)
        {
            var fullPrompt = BuildPromptWithContext(userMessage, systemContext, conversationHistory);

            var config = new GenerateContentConfig
            {
                Temperature = _settings.Temperature,
                MaxOutputTokens = _settings.MaxTokens
            };

            // Use streaming API
            await foreach (var chunk in _client.Models.GenerateContentStreamAsync(
                model: _settings.Model,
                contents: fullPrompt,
                config: config))
            {
                if (chunk.Candidates != null && chunk.Candidates.Any())
                {
                    var firstCandidate = chunk.Candidates[0];
                    if (firstCandidate.Content?.Parts != null && firstCandidate.Content.Parts.Any())
                    {
                        foreach (var part in firstCandidate.Content.Parts)
                        {
                            if (!string.IsNullOrEmpty(part.Text))
                            {
                                yield return part.Text;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Search the web using Gemini (if needed)
        /// </summary>
        public async Task<string> SearchWebAsync(string query)
        {
            if (!_settings.EnableWebSearch)
            {
                return "Web search is currently disabled.";
            }

            var searchPrompt = $"Please search for and provide accurate information about: {query}";

            return await GenerateContentAsync(searchPrompt);
        }

        /// <summary>
        /// Build prompt with conversation history and context
        /// </summary>
        private string BuildPromptWithContext(
            string userMessage,
            string systemContext,
            List<(string role, string content)> conversationHistory)
        {
            var promptParts = new List<string>();

            // Add system context if provided
            if (!string.IsNullOrWhiteSpace(systemContext))
            {
                promptParts.Add($"System Instructions: {systemContext}\n");
            }

            // Add conversation history if provided
            if (conversationHistory != null && conversationHistory.Any())
            {
                var historyText = string.Join("\n", conversationHistory
                    .Take(_settings.MaxConversationHistory)
                    .Select(h => $"{(h.role == "user" ? "User" : "Assistant")}: {h.content}"));

                if (!string.IsNullOrWhiteSpace(historyText))
                {
                    promptParts.Add($"Conversation History:\n{historyText}\n");
                }
            }

            // Add current user message
            promptParts.Add($"Current Question: {userMessage}");

            return string.Join("\n", promptParts);
        }

        /// <summary>
        /// Get available models
        /// </summary>
        public async Task<List<string>> GetAvailableModelsAsync()
        {
            try
            {
                var models = await _client.Models.ListAsync();
                return await models.Select(m => m.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve models: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Count tokens in a prompt (useful for staying under limits)
        /// </summary>
        public async Task<int> CountTokensAsync(string text)
        {
            try
            {
                var response = await _client.Models.CountTokensAsync(
                    model: _settings.Model,
                    contents: text
                );

                return (int)response.TotalTokens;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to count tokens: {ex.Message}", ex);
            }
        }
    }
}

