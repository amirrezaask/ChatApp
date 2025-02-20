using ChatApp.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application;


public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Chat.dat");
    }
}

