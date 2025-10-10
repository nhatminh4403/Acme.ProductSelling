using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Chatbots.Dtos
{
    public class SendMessageInput
    {
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        public string SessionId { get; set; }
        public bool IsAdminChat { get; set; }
    }
}
