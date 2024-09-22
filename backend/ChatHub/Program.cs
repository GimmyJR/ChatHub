
using ChatHub.Hubs;
using ChatHub.Models;
using ChatHub.Repository;
using Microsoft.EntityFrameworkCore;

namespace ChatHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IRegisterRepository, RegisterRepository>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddSignalR();
            builder.Services.AddCors(corsoptions => corsoptions.AddPolicy("MyPolicy", corsoptionsbuilder => { corsoptionsbuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseCors("MyPolicy");
            app.MapHub<chathub>("/chat-hub");
            app.MapControllers();

            app.Run();
        }
    }
}
