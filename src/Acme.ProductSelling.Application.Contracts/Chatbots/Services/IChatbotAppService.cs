using Acme.ProductSelling.Chatbots.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
namespace Acme.ProductSelling.Chatbots.Services
{
    public interface IChatbotAppService : IApplicationService, ITransientDependency
    {
        Task<ChatResponseDto> SendMessageAsync(SendMessageInput input);
        Task TrainModelAsync();
        Task<List<ChatbotMessage>> GetConversationHistoryAsync(int maxCount);
    }
}
