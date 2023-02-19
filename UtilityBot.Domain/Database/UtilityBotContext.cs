using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Database;

public class UtilityBotContext : DbContext
{
    public UtilityBotContext(DbContextOptions<UtilityBotContext> options) : base(options)
    {
    }

    public DbSet<JoinedServer>? JoinedServers { get; set; }
    public DbSet<ConfiguredAction>? ConfiguredActions { get; set; }
    public DbSet<UserJoinConfiguration>? UserJoinConfigurations { get; set; }
    public DbSet<UserJoinMessage>? UserJoinMessages { get; set; }
    public DbSet<UserJoinRole>? UserJoinRoles { get; set; }
    public DbSet<VerifyConfiguration>? VerifyConfigurations { get; set; }
    public DbSet<LogConfiguration>? LogConfigurations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfiguredAction>().HasKey(x => new { x.GuildId, x.ConfigurationType });
        modelBuilder.Entity<UserJoinConfiguration>().HasKey(x => new { x.GuildId, ActionType = x.Action });
        modelBuilder.Entity<UserJoinRole>().HasKey(x => new { x.GuildId, x.RoleId });
    }
}