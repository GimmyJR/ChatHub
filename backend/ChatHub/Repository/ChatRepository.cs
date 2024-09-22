using Azure.Core;
using ChatHub.Dto;
using ChatHub.Hubs;
using ChatHub.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatHub.Repository
{
    public class ChatRepository:IChatRepository
    {
        private readonly AppDbContext context;
        private readonly IHubContext<chathub> hubContext;

        public ChatRepository(AppDbContext context,IHubContext<chathub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }
        public async Task<List<Chat>> GetChats(Guid userId,Guid toUserId,CancellationToken cancellationToken)
        {
            List<Chat> chats =await context.Chats.Where(p =>
            p.UserId == userId && p.ToUserId == toUserId ||
            p.ToUserId == userId && p.UserId == toUserId)
            .OrderBy(p => p.Date)
            .ToListAsync(cancellationToken);
            return chats;
        }
        public async Task SendMessage(SendMessageDto sendMessageDto, CancellationToken cancellationToken)
        {
            Chat chat = new()
            {
                UserId = sendMessageDto.UserId,
                ToUserId = sendMessageDto.ToUserId,
                Message = sendMessageDto.Message,
                Date = DateTime.Now
            };

            await context.AddAsync(chat, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            string connectionId = chathub.Users.First(p => p.Value == chat.ToUserId).Key;

            await hubContext.Clients.Client(connectionId).SendAsync("Messages", chat);

        }
        public async Task<List<User>> GetUsers()
        {
            List<User> users= await context.Users.OrderBy(p => p.Name).ToListAsync();
            return users;
        }
    }
}
