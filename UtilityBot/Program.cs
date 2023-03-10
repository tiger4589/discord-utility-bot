using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UtilityBot.Client;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.Mappers;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Domain.Services.ConfigurationService.Services;
using UtilityBot.Domain.Services.UserNoteServices;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.EventLogService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;
using UtilityBot.Services.GuildJoinedServices.Managers;
using UtilityBot.Services.InteractionServiceManager;
using UtilityBot.Services.JokesServices;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.MagicEightBall;
using UtilityBot.Services.MessageHandlers;
using UtilityBot.Services.NoteServices;
using UtilityBot.Services.PlayerServices;
using UtilityBot.Services.RumbleServices;
using UtilityBot.Services.SpamProtectionServices;
using UtilityBot.Services.Uno.Manager;
using UtilityBot.Services.UserJoinedServices;
using ConfigurationService = UtilityBot.Services.ConfigurationServices.ConfigurationService;
using IConfigurationService = UtilityBot.Services.ConfigurationServices.IConfigurationService;

var serviceProvider = BuildServiceProvider();

var context = serviceProvider.GetRequiredService<UtilityBotContext>();
context.Database.Migrate();

InitializeMainComponents();

var client = serviceProvider.GetRequiredService<BotClient>();
await client.StartClient();

IServiceProvider BuildServiceProvider() => new ServiceCollection()
    .AddSingleton<IConfiguration>(sp =>
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json", optional: false);
        return configurationBuilder.Build();
    })
    .AddDbContext<UtilityBotContext>((sp,options) =>
    {
        var conf = sp.GetRequiredService<IConfiguration>();
        options.UseSqlServer(conf["DefaultConnection"]);
    })
    .AddSingleton<DiscordSocketClient>(_ =>
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = false,
            MessageCacheSize = 100
        };

        return new DiscordSocketClient(config);
    })
    .AddSingleton<IGuildJoinedManager, GuildJoinedManager>()
    .AddSingleton<IInteractionServiceServices, InteractionServiceServices>()
    .AddSingleton<ILoggingService, LoggingService>()
    .AddSingleton<IConfigurationService, ConfigurationService>()
    .AddSingleton<IUserJoinedService, UserJoinedService>()
    .AddSingleton<ICacheManager, CacheManager>()
    .AddSingleton<IPlayerService, PlayerService>()
    .AddSingleton<IEmbedMessageBuilder, EmbedMessageBuilder>()
    .AddSingleton<BotClient>()
    .AddSingleton<UtilityBot.Domain.Services.ConfigurationService.Interfaces.IConfigurationService, UtilityBot.Domain.Services.ConfigurationService.Services.ConfigurationService>()
    .AddSingleton<IMessageHandler, MessageHandler>()
    .AddSingleton<IJokeService, JokeService>()
    .AddAutoMapper(typeof(ServerMappingProfile))
    .AddTransient<ILogger>(_ => new LoggerConfiguration().MinimumLevel.Warning().WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Hour,
        shared: true, retainedFileCountLimit: 72).CreateLogger())
    .AddSingleton<IUserNoteService, UserNoteService>()
    .AddSingleton<INoteService, NoteService>()
    .AddSingleton<IRumbleConfigurationService, RumbleConfigurationService>()
    .AddSingleton<IRumbleService, RumbleService>()
    .AddSingleton<ISpamProtectionService, SpamProtectionService>()
    .AddSingleton<IMagicEightBallService, MagicEightBallService>()
    .AddSingleton<IMagicEightBall, MagicEightBall>()
    .AddSingleton<IEventConfiguration, EventConfiguration>()
    .AddSingleton<IEventService, EventService>()
    .AddSingleton<IUnoConfigurationService, UnoConfigurationService>()
    .AddSingleton<IUnoManager, UnoManager>()
    .AddSingleton<IUnoGameManager, UnoGameManager>()
    .BuildServiceProvider();

void InitializeMainComponents()
{
    var loggingService = serviceProvider.GetRequiredService<ILoggingService>();
    loggingService.InitializeService();
    Logger.InitializeLogger(serviceProvider.GetRequiredService<ICacheManager>(), serviceProvider.GetRequiredService<DiscordSocketClient>());

    serviceProvider.GetRequiredService<IGuildJoinedManager>();
    
    serviceProvider.GetRequiredService<IInteractionServiceServices>();
    serviceProvider.GetRequiredService<IUserJoinedService>();
    serviceProvider.GetRequiredService<IJokeService>();
    serviceProvider.GetRequiredService<IRumbleService>();
    serviceProvider.GetRequiredService<ISpamProtectionService>();
    serviceProvider.GetRequiredService<IMagicEightBall>();
    serviceProvider.GetRequiredService<IUnoManager>();
}

