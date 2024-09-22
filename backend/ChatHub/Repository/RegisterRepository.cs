using ChatHub.Dto;
using ChatHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatHub.Repository
{
    public class RegisterRepository:IRegisterRepository
    {
        private readonly AppDbContext context;

        public RegisterRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> IsNameExist (string name)
        {
            return (await context.Users.AnyAsync(u => u.Name == name));
        }

       
        
        public async Task AddToDb(User user,CancellationToken cancellationToken)
        {
            await context.Users.AddAsync(user,cancellationToken);
            await context.SaveChangesAsync();
        }
        public async Task<User> IsUserExist(string name,CancellationToken cancellationToken)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
            if (user != null)
            {
                user.Status = "online";
                await context.SaveChangesAsync();
            }
            return (user);
        }

        public async Task Logout(Guid userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ;
            }

            
            user.Status = "offline";
            await context.SaveChangesAsync();

            return ;
        }

    }
}
