using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilityBot.Client;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;
using UtilityBot.Services.GuildJoinedServices.Managers;
using UtilityBot.Services.InteractionServiceManager;
using UtilityBot.Services.LoggingServices;

IServiceProvider BuildServiceProvider() => new ServiceCollection()
    //Transient
    .AddTransient<BotClient>()
    .AddTransient<IConfiguration>(sp =>
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        return configurationBuilder.Build();
    })
    .AddTransient<IGuildJoinedManager, GuildJoinedManager>()
    .AddTransient<IInteractionServiceServices, InteractionServiceServices>()
    .AddTransient<ILoggingService, LoggingService>()
    //Scoped
    .AddScoped(_ => new HttpClient())
    //Singletons
    .AddSingleton<ICacheManager, CacheManager>()
    .BuildServiceProvider();

var serviceProvider = BuildServiceProvider();

var client = serviceProvider.GetRequiredService<BotClient>();

await client.StartClient();