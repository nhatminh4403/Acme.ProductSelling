using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Chatbots
{
    public class ChatbotTrainingData : Entity<Guid>
    {
        public string Message { get; set; }
        public string Intent { get; set; }
        public bool IsVerified { get; set; }
    }
}
