using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilityBot.Services.GuildJoinedServices.Interfaces;
using UtilityBot.Services.InteractionServiceManager;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Client;

public class BotClient
{
    private DiscordSocketClient? _client;
    private readonly IServiceProvider _serviceProvider;
    
    private readonly IConfiguration _configuration;
    private readonly IGuildJoinedManager _guildJoinedManager;
    private readonly IInteractionServiceServices _interactionService;
    private readonly ILoggingService _loggingService;

    //private CommandService? _commandService;
    //private CommandHandler _handler;
    public BotClient(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _guildJoinedManager = serviceProvider.GetRequiredService<IGuildJoinedManager>();
        _interactionService = serviceProvider.GetRequiredService<IInteractionServiceServices>();
        _loggingService = serviceProvider.GetRequiredService<ILoggingService>();
    }

    public async Task StartClient()
    {
        var token = _configuration["BotToken"];
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = false
        };
        _client = new DiscordSocketClient(config);

        //_commandService = new CommandService();
        // _handler = new CommandHandler(_client, _commandService, _serviceProvider);
        // await _handler.InstallCommandsAsync();
        
        // _client.ReactionAdded += _client_ReactionAdded;
        // _client.ReactionRemoved += _client_ReactionRemoved;
         _client.Ready += _client_Ready;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async Task _client_Ready()
    {
        await _loggingService.InitializeService(_client!);
        await _guildJoinedManager.InitializeService(_client!);
        await _interactionService.InitializeService(_client!);
    }
}