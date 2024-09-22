using Microsoft.EntityFrameworkCore;

namespace ChatHub.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get;  set; }
        public DbSet<Chat> Chats { get; set; }
    }
}
