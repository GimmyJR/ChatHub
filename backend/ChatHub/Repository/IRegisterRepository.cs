using ChatHub.Dto;
using ChatHub.Models;

namespace ChatHub.Repository
{
    public interface IRegisterRepository
    {
        Task<bool> IsNameExist(string name);
        Task AddToDb(User user, CancellationToken cancellationToken);
        Task<User> IsUserExist(string name, CancellationToken cancellationToken);
        Task Logout(Guid userId);
    }
}
