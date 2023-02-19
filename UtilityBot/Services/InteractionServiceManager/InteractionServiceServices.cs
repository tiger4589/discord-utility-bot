using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace UtilityBot.Services.InteractionServiceManager;

public class InteractionServiceServices : IInteractionServiceServices
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private InteractionService? _interactionService;

    public InteractionServiceServices(IServiceProvider serviceProvider, DiscordSocketClient client, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _client = client;
        _configuration = configuration;
        _client.Ready += InitializeService;
    }

    public async Task InitializeService()
    {
        _interactionService = new InteractionService(_client);
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(_configuration["ServerId"]!));

        _interactionService.SlashCommandExecuted += InteractionServiceOnSlashCommandExecuted;

        _client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        };
    }

    private async Task InteractionServiceOnSlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}");
                    break;
                case InteractionCommandError.UnknownCommand:
                    await arg2.Interaction.RespondAsync("Unknown command");
                    break;
                case InteractionCommandError.BadArgs:
                    await arg2.Interaction.RespondAsync("Invalid number or arguments");
                    break;
                case InteractionCommandError.Exception:
                    await arg2.Interaction.RespondAsync($"Command exception:{arg3.ErrorReason}");
                    break;
                case InteractionCommandError.Unsuccessful:
                    await arg2.Interaction.RespondAsync("Command could not be executed");
                    break;
                default:
                    await arg2.Interaction.RespondAsync($"Error: {arg3.Error}");
                    break;
            }
        }
    }
}