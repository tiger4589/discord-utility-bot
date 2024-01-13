using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

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
    public DbSet<UnoConfiguration>? UnoConfigurations { get; set; }
    public DbSet<Horse>? Horses { get; set; }
    public DbSet<Track>? Tracks { get; set; }
    public DbSet<HorseRace>? HorseRaces { get; set; }
    public DbSet<RaceStanding>? RaceStandings { get; set; }
    public DbSet<UserPrediction>? UserPredictions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserJoinConfiguration>().HasKey(x => new { x.GuildId, ActionType = x.Action });
        modelBuilder.Entity<UserJoinRole>().HasKey(x => new { x.GuildId, x.RoleId });
        modelBuilder.Entity<RaceStanding>().HasKey(x => new { x.RaceId, x.HorseId });
        modelBuilder.Entity<UserPrediction>().HasIndex(x => x.UserId);
        modelBuilder.Entity<RumbleMessageConfiguration>().HasData(new List<RumbleMessageConfiguration>
        {
            new()
            {
                Id = 1,
                Message = "A new battle has started!"
            }
        });

        modelBuilder.Entity<MagicEightBallResponse>().HasData(new List<MagicEightBallResponse>
        {
            new()
            {
                Id = 1,
                Message = "It is certain."
            },
            new()
            {
                Id = 2,
                Message = "It is decidedly so."
            },
            new()
            {
                Id = 3,
                Message = "Without a doubt."
            },
            new()
            {
                Id = 4,
                Message = "Yes definitely."
            },
            new()
            {
                Id = 5,
                Message = "You may rely on it."
            },
            new()
            {
                Id = 6,
                Message = "As I see it, yes."
            },
            new()
            {
                Id = 7,
                Message = "Most likely."
            },
            new()
            {
                Id = 8,
                Message = "Outlook good."
            },
            new()
            {
                Id = 9,
                Message = "Yes."
            },
            new()
            {
                Id = 10,
                Message = "Signs point to yes."
            },
            new()
            {
                Id = 11,
                Message = "Reply hazy, try again."
            },
            new()
            {
                Id = 12,
                Message = "Ask again later."
            },
            new()
            {
                Id = 13,
                Message = "Better not tell you now."
            },
            new()
            {
                Id = 14,
                Message = "Cannot predict now."
            },
            new()
            {
                Id = 15,
                Message = "Concentrate and ask again."
            },
            new()
            {
                Id = 16,
                Message = "Don't count on it."
            },
            new()
            {
                Id = 17,
                Message = "My reply is no."
            },
            new()
            {
                Id = 18,
                Message = "My sources say no."
            },
            new()
            {
                Id = 19,
                Message = "Outlook not so good."
            },
            new()
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
                new()
                {
                    EventName = name,
                    Id = i,
                    IsEnabled = true
                }
            });
            i++;
        }

        modelBuilder.Entity<Horse>().HasData(new List<Horse>
        {
            new() { Id = 1, Name = "Thunderhoof", OddsToOne = 2, AdvantageOn = "Nebula", DisadvantageOn = "Mirage" },
            new() { Id = 2, Name = "Midnight Mirage", OddsToOne = 3, AdvantageOn = "Crystal", DisadvantageOn = "Nebula" },
            new() { Id = 3, Name = "Blaze Lightning", OddsToOne = 4, AdvantageOn = "Inferno", DisadvantageOn = "Crystal" },
            new() { Id = 4, Name = "Mystic Gallop", OddsToOne = 5, AdvantageOn = "Oasis", DisadvantageOn = "Inferno" },
            new() { Id = 5, Name = "Starstrider", OddsToOne = 6, AdvantageOn = "Frostbite", DisadvantageOn = "Oasis" },
            new() { Id = 6, Name = "Shadowfax", OddsToOne = 7, AdvantageOn = "Techno", DisadvantageOn = "Frostbite" },
            new() { Id = 7, Name = "Solar Eclipse", OddsToOne = 8, AdvantageOn = "Jungle", DisadvantageOn = "Techno" },
            new() { Id = 8, Name = "Velvet Victory", OddsToOne = 9, AdvantageOn = "Coral", DisadvantageOn = "Jungle" },
            new() { Id = 9, Name = "Lunar Lullaby", OddsToOne = 10, AdvantageOn = "Serenity", DisadvantageOn = "Coral" },
            new() { Id = 10, Name = "Silver Streak", OddsToOne = 11, AdvantageOn = "Mirage", DisadvantageOn = "Serenity" },
            new() { Id = 11, Name = "Quantum Quicksilver", OddsToOne = 12, AdvantageOn = "Nebula", DisadvantageOn = "Mirage" },
            new() { Id = 12, Name = "Celestial Sprinter", OddsToOne = 2, AdvantageOn = "Crystal", DisadvantageOn = "Nebula" },
            new() { Id = 13, Name = "Ember Echo", OddsToOne = 3, AdvantageOn = "Inferno", DisadvantageOn = "Crystal" },
            new() { Id = 14, Name = "Neptune's Nudge", OddsToOne = 4, AdvantageOn = "Oasis", DisadvantageOn = "Inferno" },
            new() { Id = 15, Name = "Serendipity Stride", OddsToOne = 5, AdvantageOn = "Frostbite", DisadvantageOn = "Oasis" },
            new() { Id = 16, Name = "Dreamweaver", OddsToOne = 6, AdvantageOn = "Techno", DisadvantageOn = "Frostbite" },
            new() { Id = 17, Name = "Galactic Glide", OddsToOne = 7, AdvantageOn = "Jungle", DisadvantageOn = "Techno" },
            new() { Id = 18, Name = "Aurora Borealis", OddsToOne = 8, AdvantageOn = "Coral", DisadvantageOn = "Jungle" },
            new() { Id = 19, Name = "Phoenix Fury", OddsToOne = 9, AdvantageOn = "Serenity", DisadvantageOn = "Coral" },
            new() { Id = 20, Name = "Nebula Nova", OddsToOne = 10, AdvantageOn = "Mirage", DisadvantageOn = "Serenity" },
            new() { Id = 21, Name = "Velvet Vortex", OddsToOne = 11, AdvantageOn = "Nebula", DisadvantageOn = "Mirage" },
            new() { Id = 22, Name = "Whispering Wind", OddsToOne = 12, AdvantageOn = "Crystal", DisadvantageOn = "Nebula" },
            new() { Id = 23, Name = "Radiant Rainstorm", OddsToOne = 2, AdvantageOn = "Inferno", DisadvantageOn = "Crystal" },
            new() { Id = 24, Name = "Spectral Sprint", OddsToOne = 3, AdvantageOn = "Oasis", DisadvantageOn = "Inferno" },
            new() { Id = 25, Name = "Dragonfly Dash", OddsToOne = 4, AdvantageOn = "Frostbite", DisadvantageOn = "Oasis" },
            new() { Id = 26, Name = "Enchanted Equestrian", OddsToOne = 5, AdvantageOn = "Techno", DisadvantageOn = "Frostbite" },
            new() { Id = 27, Name = "Pegasus Prism", OddsToOne = 6, AdvantageOn = "Jungle", DisadvantageOn = "Techno" },
            new() { Id = 28, Name = "Radiant Racer", OddsToOne = 7, AdvantageOn = "Coral", DisadvantageOn = "Jungle" },
            new() { Id = 29, Name = "Zenith Zephyr", OddsToOne = 8, AdvantageOn = "Serenity", DisadvantageOn = "Coral" },
            new() { Id = 30, Name = "Cosmic Cascade", OddsToOne = 9, AdvantageOn = "Mirage", DisadvantageOn = "Serenity" },
            new() { Id = 31, Name = "Twilight Thunderbolt", OddsToOne = 10, AdvantageOn = "Nebula", DisadvantageOn = "Mirage" },
            new() { Id = 32, Name = "Mystic Moonlight", OddsToOne = 11, AdvantageOn = "Crystal", DisadvantageOn = "Nebula" },
            new() { Id = 33, Name = "Nebulous Nectar", OddsToOne = 12, AdvantageOn = "Inferno", DisadvantageOn = "Crystal" },
            new() { Id = 34, Name = "Stardust Sizzle", OddsToOne = 2, AdvantageOn = "Oasis", DisadvantageOn = "Inferno" },
            new() { Id = 35, Name = "Quantum Quasar", OddsToOne = 3, AdvantageOn = "Frostbite", DisadvantageOn = "Oasis" },
            new() { Id = 36, Name = "Shadow Symphony", OddsToOne = 4, AdvantageOn = "Techno", DisadvantageOn = "Frostbite" },
            new() { Id = 37, Name = "Celestial Cyclone", OddsToOne = 5, AdvantageOn = "Jungle", DisadvantageOn = "Techno" },
            new() { Id = 38, Name = "Luminous Lightning", OddsToOne = 6, AdvantageOn = "Coral", DisadvantageOn = "Jungle" },
            new() { Id = 39, Name = "Velvet Vibration", OddsToOne = 7, AdvantageOn = "Serenity", DisadvantageOn = "Coral" },
            new() { Id = 40, Name = "Solar Serenity", OddsToOne = 8, AdvantageOn = "Mirage", DisadvantageOn = "Serenity" },
            new() { Id = 41, Name = "Comet Canter", OddsToOne = 9, AdvantageOn = "Nebula", DisadvantageOn = "Mirage" },
            new() { Id = 42, Name = "Starlight Symphony", OddsToOne = 10, AdvantageOn = "Crystal", DisadvantageOn = "Nebula" },
            new() { Id = 43, Name = "Phoenix Phenomenon", OddsToOne = 11, AdvantageOn = "Inferno", DisadvantageOn = "Crystal" },
            new() { Id = 44, Name = "Midnight Melody", OddsToOne = 12, AdvantageOn = "Oasis", DisadvantageOn = "Inferno" },
            new() { Id = 45, Name = "Silver Seraph", OddsToOne = 2, AdvantageOn = "Frostbite", DisadvantageOn = "Oasis" },
            new() { Id = 46, Name = "Lunar Luminary", OddsToOne = 4, AdvantageOn = "Techno", DisadvantageOn = "Frostbite" },
            new() { Id = 47, Name = "Nebula Nomad", OddsToOne = 6, AdvantageOn = "Jungle", DisadvantageOn = "Techno" },
            new() { Id = 48, Name = "Whispering Whirlwind", OddsToOne = 8, AdvantageOn = "Coral", DisadvantageOn = "Jungle" },
            new() { Id = 49, Name = "Ethereal Elixir", OddsToOne = 10, AdvantageOn = "Serenity", DisadvantageOn = "Coral" },
            new() { Id = 50, Name = "Radiant Roamer", OddsToOne = 12, AdvantageOn = "Mirage", DisadvantageOn = "Serenity" }
        });

        modelBuilder.Entity<Track>().HasData(new List<Track>
        {
            new() { Id = 1, Length = 100, Name = "Galactic Grand Circuit", TimeBetweenRacesInMinutes = 5, Type = "Nebula" },
            new() { Id = 2, Length = 150, Name = "Celestial Speedway", TimeBetweenRacesInMinutes = 10, Type = "Crystal" },
            new() { Id = 3, Length = 200, Name = "Nebula Nook Raceway", TimeBetweenRacesInMinutes = 15, Type = "Inferno" },
            new() { Id = 4, Length = 250, Name = "Lunar Loop Lanes", TimeBetweenRacesInMinutes = 20, Type = "Oasis" },
            new() { Id = 5, Length = 300, Name = "Cosmic Carousel Circuit", TimeBetweenRacesInMinutes = 25, Type = "Frostbite" },
            new() { Id = 6, Length = 350, Name = "Starry Stride Speedway", TimeBetweenRacesInMinutes = 30, Type = "Techno" },
            new() { Id = 7, Length = 400, Name = "Enchanted Equine Expressway", TimeBetweenRacesInMinutes = 35, Type = "Jungle" },
            new() { Id = 8, Length = 450, Name = "Solar Sprints Circuit", TimeBetweenRacesInMinutes = 40, Type = "Coral" },
            new() { Id = 9, Length = 500, Name = "Midnight Mirage Oval", TimeBetweenRacesInMinutes = 45, Type = "Serenity" },
            new() { Id = 10, Length = 1000, Name = "Mystic Meadow Racetrack", TimeBetweenRacesInMinutes = 50, Type = "Mirage" }
        });
    }
}