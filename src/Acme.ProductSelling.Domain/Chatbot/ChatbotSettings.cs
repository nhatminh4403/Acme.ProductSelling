namespace Acme.ProductSelling.Chatbot
{
    public class ChatbotSettings
    {
        public string ApiKey { get; set; }
        public string Model { get; set; }
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public bool EnableWebSearch { get; set; }
        public int MaxConversationHistory { get; set; }
        public int MaxProductSuggestions { get; set; }
    }
}
