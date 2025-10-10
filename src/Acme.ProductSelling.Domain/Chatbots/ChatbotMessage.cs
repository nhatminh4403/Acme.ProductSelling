using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Chatbots
{
    public class ChatbotMessage : AuditedAggregateRoot<Guid>
    {
        public string UserMessage { get; set; }
        public string BotResponse { get; set; }
        public string DetectedIntent { get; set; }
        public float IntentConfidence { get; set; }
        public string SessionId { get; set; }
        public Guid? UserId { get; set; }
        public bool IsAdminChat { get; set; }
    }
}
