using Acme.ProductSelling.Chatbot.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Chatbot.Services
{
    public interface IChatbotAppService : IApplicationService
    {
        Task<ChatMessageOutputDto> SendMessageAsync(ChatMessageInputDto input);
        Task<List<ChatbotProductDto>> SearchProductsAsync(string query);
    }
}
