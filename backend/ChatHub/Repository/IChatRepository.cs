using ChatHub.Dto;
using ChatHub.Models;

namespace ChatHub.Repository
{
    public interface IChatRepository
    {
        Task<List<Chat>> GetChats(Guid userId, Guid toUserId, CancellationToken cancellationToken);
        Task SendMessage(SendMessageDto sendMessageDto, CancellationToken cancellationToken);
        Task<List<User>> GetUsers();
    }
}
