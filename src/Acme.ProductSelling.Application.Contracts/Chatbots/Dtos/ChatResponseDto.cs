using System;
using System.Collections.Generic;

namespace Acme.ProductSelling.Chatbots.Dtos
{
    public class ChatResponseDto
    {
        public string Message { get; set; }
        public string Intent { get; set; }
        public float Confidence { get; set; }
        public DateTime Timestamp { get; set; }
        public List<string> Suggestions { get; set; }
    }
}
