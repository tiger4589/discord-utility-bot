﻿using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Database;

public class UtilityBotContext : DbContext
{
    public UtilityBotContext(DbContextOptions<UtilityBotContext> options) : base(options)
    {
    }

    public DbSet<JoinedServer>? JoinedServers { get; set; }
    public DbSet<UserJoinConfiguration>? UserJoinConfigurations { get; set; }
    public DbSet<UserJoinConfigurationAudit>? UserJoinConfigurationsAudit { get; set; }
    public DbSet<UserJoinMessage>? UserJoinMessages { get; set; }
    public DbSet<UserJoinMessageAudit>? UserJoinMessageAudits { get; set; }
    public DbSet<UserJoinRole>? UserJoinRoles { get; set; }
    public DbSet<UserJoinRoleAudit>? UserJoinRoleAudits { get; set; }
    public DbSet<VerifyConfiguration>? VerifyConfigurations { get; set; }
    public DbSet<VerifyConfigurationAudit>? VerifyConfigurationAudits { get; set; }
    public DbSet<LogConfiguration>? LogConfigurations { get; set; }
    public DbSet<LogConfigurationAudit>? LogConfigurationsAudit { get; set; }
    public DbSet<VerifyMessageConfiguration>? VerifyMessageConfigurations { get; set; }
    public DbSet<VerifyMessageConfigurationAudit>? VerifyMessageConfigurationAudits { get; set; }
    public DbSet<JokeConfiguration>? JokeConfigurations { get; set; }
    public DbSet<UserNote>? UserNotes { get; set; }
    public DbSet<RumbleConfiguration>? RumbleConfigurations { get; set; }
    public DbSet<RumbleMessageConfiguration>? RumbleMessageConfigurations { get; set; }
    public DbSet<CapsProtectionConfiguration>? CapsProtectionConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserJoinConfiguration>().HasKey(x => new { x.GuildId, ActionType = x.Action });
        modelBuilder.Entity<UserJoinRole>().HasKey(x => new { x.GuildId, x.RoleId });
        modelBuilder.Entity<RumbleMessageConfiguration>().HasData(new List<RumbleMessageConfiguration>
        {
            new RumbleMessageConfiguration
            {
                Id = 1,
                Message = "A new battle has started!"
            }
        });
    }
}