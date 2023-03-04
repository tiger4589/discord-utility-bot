using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.MagicEightBall;

public class MagicEightBall : IMagicEightBall
{
    private readonly ICacheManager _cacheManager;
    private readonly DiscordSocketClient _client;
    private readonly IMagicEightBallService _eightBallService;

    public MagicEightBall(ICacheManager cacheManager, DiscordSocketClient client, IMagicEightBallService eightBallService)
    {
        _cacheManager = cacheManager;
        _client = client;
        _eightBallService = eightBallService;
        _client.Ready -= ClientOnReady;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        var configuration = await _eightBallService.GetLatestConfiguration();
        if (configuration == null)
        {
            await Logger.Log("Magic Eight Ball isn't configured");
        }
        else
        {
            _cacheManager.AddOrUpdate(configuration);
            await Logger.Log("Magic Eight ball configuration loaded");
        }

        var magicEightBallResponses = await _eightBallService.GetResponses();
        foreach (var magicEightBallResponse in magicEightBallResponses)
        {
            _cacheManager.Add(magicEightBallResponse);
        }

        await Logger.Log("Loaded Magic Eight Ball Responses");
    }

    public async Task AddResponse(SocketInteractionContext context, string response)
    {
        var magicEightBallResponse = new MagicEightBallResponse
        {
            Message = response
        };
        await _eightBallService.AddResponse(magicEightBallResponse);

        _cacheManager.Add(magicEightBallResponse);

        await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"Added: {response}");
    }

    public async Task AddConfiguration(SocketInteractionContext context, IChannel channel)
    {
        var magicEightBallConfiguration = new MagicEightBallConfiguration
        {
            ChannelId = channel.Id,
            IsEnabled = true
        };
        await _eightBallService.AddConfiguration(magicEightBallConfiguration);

        _cacheManager.AddOrUpdate(magicEightBallConfiguration);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = $"Configuration set to reply in #{channel.Name}");
    }

    public async Task Enable(SocketInteractionContext context)
    {
        var magicEightBallConfiguration = _cacheManager.GetMagicEightBallConfiguration();
        if (magicEightBallConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in first place.");
            return;
        }

        if (magicEightBallConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is already enabled");
            return;
        }

        await _eightBallService.Enable();
        _cacheManager.EnableMagicEightBall();

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = "Magic Eight has been enabled");
    }

    public async Task Disable(SocketInteractionContext context)
    {
        var magicEightBallConfiguration = _cacheManager.GetMagicEightBallConfiguration();
        if (magicEightBallConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in first place.");
            return;
        }

        if (!magicEightBallConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is already disabled");
            return;
        }

        await _eightBallService.Disable();
        _cacheManager.DisableMagicEightBall();

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = "Magic Eight has been disabled");
    }

    public async Task Answer(SocketInteractionContext context, string question)
    {
        var magicEightBallConfiguration = _cacheManager.GetMagicEightBallConfiguration();
        if (magicEightBallConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball isn't configured yet");
            return;
        }

        if (!magicEightBallConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is currently disabled");
            return;
        }

        if (magicEightBallConfiguration.ChannelId != context.Channel.Id)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "This isn't where you can ask me a question!");
            return;
        }

        var magicEightBallResponses = _cacheManager.GetMagicEightBallResponses();

        Random rand = new Random();

        var response = magicEightBallResponses[rand.Next(0, magicEightBallResponses.Count)];

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Question: {question}");
        sb.AppendLine($"Answer: {response.Message}");
        await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"{sb}");
    }
}