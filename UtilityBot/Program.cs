using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UtilityBot.Client;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.Mappers;
using UtilityBot.Domain.Services.UserNoteServices;
using UtilityBot.Services.ButtonHandlers;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.ConfigurationServices;
using UtilityBot.Services.GuildJoinedServices.Interfaces;
using UtilityBot.Services.GuildJoinedServices.Managers;
using UtilityBot.Services.InteractionServiceManager;
using UtilityBot.Services.JokesServices;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.MessageHandlers;
using UtilityBot.Services.NoteServices;
using UtilityBot.Services.PlayerServices;
using UtilityBot.Services.UserJoinedServices;

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
            UseInteractionSnowflakeDate = false
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
    .AddSingleton<IButtonHandler, ButtonHandler>()
    .AddSingleton<BotClient>()
    .AddSingleton<UtilityBot.Domain.Services.ConfigurationService.Interfaces.IConfigurationService, UtilityBot.Domain.Services.ConfigurationService.Services.ConfigurationService>()
    .AddSingleton<IMessageHandler, MessageHandler>()
    .AddSingleton<IJokeService, JokeService>()
    .AddAutoMapper(typeof(ServerMappingProfile))
    .AddTransient<ILogger>(_ => new LoggerConfiguration().MinimumLevel.Warning().WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Hour,
        shared: true, retainedFileCountLimit: 72).CreateLogger())
    .AddSingleton<IUserNoteService, UserNoteService>()
    .AddSingleton<INoteService, NoteService>()
    .BuildServiceProvider();

void InitializeMainComponents()
{
    var loggingService = serviceProvider.GetRequiredService<ILoggingService>();
    loggingService.InitializeService();
    Logger.InitializeLogger(serviceProvider.GetRequiredService<ICacheManager>(), serviceProvider.GetRequiredService<DiscordSocketClient>());

    serviceProvider.GetRequiredService<IGuildJoinedManager>();
    
    serviceProvider.GetRequiredService<IInteractionServiceServices>();
    serviceProvider.GetRequiredService<IUserJoinedService>();
    serviceProvider.GetRequiredService<IButtonHandler>();
    serviceProvider.GetRequiredService<IJokeService>();
}

