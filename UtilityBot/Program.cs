using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilityBot.Client;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.ConfigurationServices;
using UtilityBot.Services.GuildJoinedServices.Interfaces;
using UtilityBot.Services.GuildJoinedServices.Managers;
using UtilityBot.Services.InteractionServiceManager;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.UserJoinedServices;

var serviceProvider = BuildServiceProvider();
InitializeMainComponents(serviceProvider);
var client = serviceProvider.GetRequiredService<BotClient>();
await client.StartClient();

IServiceProvider BuildServiceProvider() => new ServiceCollection()
    .AddSingleton<IConfiguration>(sp =>
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        return configurationBuilder.Build();
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
    .AddSingleton<BotClient>()
    .BuildServiceProvider();

void InitializeMainComponents(IServiceProvider serviceProvider1)
{
    serviceProvider.GetRequiredService<IGuildJoinedManager>();
    serviceProvider.GetRequiredService<ILoggingService>();
    serviceProvider.GetRequiredService<IInteractionServiceServices>();
    serviceProvider.GetRequiredService<IUserJoinedService>();
}