using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace UtilityBot.Services.InteractionServiceManager;

public class InteractionServiceServices : IInteractionServiceServices
{
    private readonly IServiceProvider _serviceProvider;
    private InteractionService? _interactionService;

    public InteractionServiceServices(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeService(DiscordSocketClient client)
    {
        _interactionService = new InteractionService(client);
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        //todo: register globally on release
        await _interactionService.RegisterCommandsToGuildAsync(686553421280575521);

        _interactionService.SlashCommandExecuted += InteractionServiceOnSlashCommandExecuted;

        client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(client, interaction);
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