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
        var configuration = (await _eightBallService.GetConfigurations()).ToList();

        if (!configuration.Any())
        {
            await Logger.Log("Magic Eight Ball isn't configured");
        }
        else
        {
            foreach (var magicEightBallConfiguration in configuration)
            {
                _cacheManager.AddOrUpdate(magicEightBallConfiguration);
            }
            
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

    public async Task Enable(SocketInteractionContext context, IChannel channel)
    {
        var magicEightBallConfiguration = _cacheManager.GetMagicEightBallConfiguration();
        if (magicEightBallConfiguration == null || !magicEightBallConfiguration.Any())
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in first place.");
            return;
        }

        var eightBallConfiguration = magicEightBallConfiguration.SingleOrDefault(x => x.ChannelId == channel.Id);
        if (eightBallConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                               prop.Content = "Configuration isn't set in first place for this channel.");
            return;
        }

        if (eightBallConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is already enabled for this channel");
            return;
        }

        await _eightBallService.Enable(channel.Id);
        _cacheManager.EnableMagicEightBall(channel.Id);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = "Magic Eight has been enabled for this channel");
    }

    public async Task Disable(SocketInteractionContext context, IChannel channel)
    {
        var magicEightBallConfiguration = _cacheManager.GetMagicEightBallConfiguration();
        if (magicEightBallConfiguration == null || !magicEightBallConfiguration.Any())
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in first place.");
            return;
        }

        var eightBallConfiguration = magicEightBallConfiguration.SingleOrDefault(x => x.ChannelId == channel.Id);
        if (eightBallConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in first place for this channel.");
            return;
        }

        if (!eightBallConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is already disabled for this channel");
            return;
        }

        await _eightBallService.Disable(channel.Id);
        _cacheManager.DisableMagicEightBall(channel.Id);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = "Magic Eight has been disabled for this channel");
    }

    public async Task Answer(SocketInteractionContext context, string question)
    {
        var magicEightBallConfiguration = _cacheManager.GetMagicEightBallConfiguration();
        if (magicEightBallConfiguration == null || !magicEightBallConfiguration.Any())
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball isn't configured yet");
            return;
        }

        var conf = magicEightBallConfiguration.SingleOrDefault(x => x.ChannelId == context.Channel.Id);
        if (conf == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is currently disabled for this channel");
            return;
        }

        if (!conf.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Magic Eight Ball is currently disabled for this channel");
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