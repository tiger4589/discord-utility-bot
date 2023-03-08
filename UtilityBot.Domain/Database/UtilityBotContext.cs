using Microsoft.EntityFrameworkCore;
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
    public DbSet<MagicEightBallConfiguration>? MagicEightBallConfigurations { get; set; }
    public DbSet<MagicEightBallResponse>? MagicEightBallResponses { get; set; }
    public DbSet<EventsConfiguration>? EventsConfigurations { get; set; }
    public DbSet<CoderRequestVerification>? CoderRequestVerifications { get; set; }

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

        modelBuilder.Entity<MagicEightBallResponse>().HasData(new List<MagicEightBallResponse>
        {
            new MagicEightBallResponse
            {
                Id = 1,
                Message = "It is certain."
            },
            new MagicEightBallResponse
            {
                Id = 2,
                Message = "It is decidedly so."
            },
            new MagicEightBallResponse
            {
                Id = 3,
                Message = "Without a doubt."
            },
            new MagicEightBallResponse
            {
                Id = 4,
                Message = "Yes definitely."
            },
            new MagicEightBallResponse
            {
                Id = 5,
                Message = "You may rely on it."
            },
            new MagicEightBallResponse
            {
                Id = 6,
                Message = "As I see it, yes."
            },
            new MagicEightBallResponse
            {
                Id = 7,
                Message = "Most likely."
            },
            new MagicEightBallResponse
            {
                Id = 8,
                Message = "Outlook good."
            },
            new MagicEightBallResponse
            {
                Id = 9,
                Message = "Yes."
            },
            new MagicEightBallResponse
            {
                Id = 10,
                Message = "Signs point to yes."
            },
            new MagicEightBallResponse
            {
                Id = 11,
                Message = "Reply hazy, try again."
            },
            new MagicEightBallResponse
            {
                Id = 12,
                Message = "Ask again later."
            },
            new MagicEightBallResponse
            {
                Id = 13,
                Message = "Better not tell you now."
            },
            new MagicEightBallResponse
            {
                Id = 14,
                Message = "Cannot predict now."
            },
            new MagicEightBallResponse
            {
                Id = 15,
                Message = "Concentrate and ask again."
            },
            new MagicEightBallResponse
            {
                Id = 16,
                Message = "Don't count on it."
            },
            new MagicEightBallResponse
            {
                Id = 17,
                Message = "My reply is no."
            },
            new MagicEightBallResponse
            {
                Id = 18,
                Message = "My sources say no."
            },
            new MagicEightBallResponse
            {
                Id = 19,
                Message = "Outlook not so good."
            },
            new MagicEightBallResponse
            {
                Id = 20,
                Message = "Very doubtful."
            }
        });

        int i = 1;
        foreach (var name in EventsName.EventsNames)
        {
            modelBuilder.Entity<EventsConfiguration>().HasData(new List<EventsConfiguration>()
            {
                new EventsConfiguration
                {
                    EventName = name,
                    Id = i,
                    IsEnabled = true
                }
            });
            i++;
        }
    }
}