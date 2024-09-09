using MeChat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Contexts;

public class ApplicationDataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly IConfiguration _configuration;
    
    public DbSet<ChatGroup> ChatGroups { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<AppStatistic> AppStatistics { get; set; }

    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("Default"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(ent =>
        {
            ent.HasKey(e => e.Id);
            ent.HasOne(e => e.Group)
                .WithMany(e => e.Chats)
                .HasForeignKey(e => e.GroupId);
        });

        modelBuilder.Entity<ChatGroup>(ent =>
        {
            ent.HasKey(e => e.Id);
            ent.HasMany(e => e.Chats)
                .WithOne(e => e.Group)
                .HasForeignKey(e => e.GroupId);
            ent.HasMany(e => e.Users)
                .WithMany(e => e.ChatGroups);
            ent.HasOne(e => e.Owner);
        });
        
        modelBuilder.Entity<User>(ent =>
        {
            ent.HasKey(e => e.Id);
            ent.HasMany(e => e.ChatGroups)
                .WithMany(e => e.Users);
        });

        modelBuilder.Entity<AppStatistic>(ent =>
        {
            ent.HasKey(e => e.Id);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}