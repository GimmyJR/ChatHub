using ChatHub.Dto;
using ChatHub.Models;
using ChatHub.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatRepository chatRepository;

        public ChatsController(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetChats(Guid userId,Guid toUserId,CancellationToken cancellationToken)
        {
            List<Chat> chatList = await chatRepository.GetChats(userId,toUserId,cancellationToken);
            return Ok(chatList);
        }
        [HttpGet("/users")]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await chatRepository.GetUsers();
            return Ok(users);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto,CancellationToken cancellationToken)
        {
            chatRepository.SendMessage(sendMessageDto,cancellationToken);
            return Ok();
        }


    }
}
