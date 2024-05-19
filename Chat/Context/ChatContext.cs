using Chat.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat.Context;

public class ChatContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<ChatRoom> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}