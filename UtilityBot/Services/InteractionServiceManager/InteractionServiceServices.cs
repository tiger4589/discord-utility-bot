using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using UtilityBot.Services.LoggingServices;
using Microsoft.VisualBasic;

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
        _client.Ready -= InitializeService;
        _client.Ready += InitializeService;
    }

    public async Task InitializeService()
    {
        try
        {
            _interactionService = new InteractionService(_client);
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(_configuration["ServerId"]!));

            _interactionService.SlashCommandExecuted += InteractionServiceOnSlashCommandExecuted;

            _client.InteractionCreated -= ClientOnInteractionCreated;
            _client.InteractionCreated += ClientOnInteractionCreated;

            await Logger.Log("Should have set up my slash commands and ready to listen!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e}");
        }
    }

    private async Task ClientOnInteractionCreated(SocketInteraction arg)
    {
        var ctx = new SocketInteractionContext(_client, arg);

        if (_interactionService == null)
        {
            return;
        }

        await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }

    private async Task InteractionServiceOnSlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    await Logger.Log($"{arg2.User.Username} tried to use {arg1.Name} command with unmet precondition!");
                    await arg2.Interaction.RespondAsync("Not Allowed!", ephemeral: true);
                    break;
                case InteractionCommandError.UnknownCommand:
                    await arg2.Interaction.RespondAsync("Unknown command", ephemeral: true);
                    break;
                case InteractionCommandError.BadArgs:
                    await arg2.Interaction.RespondAsync("Invalid number or arguments", ephemeral: true);
                    break;
                case InteractionCommandError.Exception:
                    await Logger.Log($"{arg2.User.Username} tried to use {arg1.Name} and got an exception: {arg3.ErrorReason}");
                    await arg2.Interaction.RespondAsync("Error occured, mods have been alerted!", ephemeral: true);
                    break;
                case InteractionCommandError.Unsuccessful:
                    await arg2.Interaction.RespondAsync("Command could not be executed", ephemeral: true);
                    break;
                default:
                    await Logger.Log($"{arg2.User.Username} tried to use {arg1.Name} and got an error: {arg3.Error} - {arg3.ErrorReason}");
                    await arg2.Interaction.RespondAsync("Error occured, mods have been alerted!", ephemeral: true);
                    break;
            }
        }
    }
}