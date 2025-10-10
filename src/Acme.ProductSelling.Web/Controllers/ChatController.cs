using Acme.ProductSelling.Chatbots;
using Acme.ProductSelling.Chatbots.Dtos;
using Acme.ProductSelling.Chatbots.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Controllers
{

    [Route("api/app/chatbot")]
    public class ChatController : AbpController
    {
        private readonly IChatbotAppService _chatbotAppService;

        public ChatController(IChatbotAppService chatbotAppService)
        {
            _chatbotAppService = chatbotAppService;
        }

        [HttpPost("message")]
        public async Task<ChatResponseDto> SendMessage([FromBody] SendMessageInput input)
        {
            return await _chatbotAppService.SendMessageAsync(input);
        }

        [HttpPost("train")]
        [Authorize(ProductSellingPermissions.Chatbots.Default)]
        public async Task TrainModel()
        {
            await _chatbotAppService.TrainModelAsync();
        }

        [HttpGet("history")]
        [Authorize(ProductSellingPermissions.Chatbots.Default)]
        public async Task<List<ChatbotMessage>> GetHistory()
        {
            return await _chatbotAppService.GetConversationHistoryAsync(100);
        }
    }
}
