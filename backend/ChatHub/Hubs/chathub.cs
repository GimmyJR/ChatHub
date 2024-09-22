using ChatHub.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatHub.Hubs
{
    public class chathub:Hub
    {
        public chathub(AppDbContext context)
        {
            this.context = context;
        }
        public static Dictionary<string, Guid> Users = new();
        private readonly AppDbContext context;

        public async Task Connect(Guid userId)
        {
            Users.Add(Context.ConnectionId,userId);
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "online";
                await context.SaveChangesAsync();
                await Clients.All.SendAsync("Users", user);
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Guid userId;
            Users.TryGetValue(Context.ConnectionId, out userId);
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                await context.SaveChangesAsync();
                await Clients.All.SendAsync("Users",user);
            }
        }
    }
}
