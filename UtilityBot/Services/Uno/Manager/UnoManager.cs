using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.Uno.Manager;

public class UnoManager : IUnoManager
{
    private readonly ICacheManager _cacheManager;
    private readonly IUnoConfigurationService _unoConfigurationService;
    private readonly DiscordSocketClient _client;

    public UnoManager(ICacheManager cacheManager, IUnoConfigurationService unoConfigurationService, DiscordSocketClient client)
    {
        _cacheManager = cacheManager;
        _unoConfigurationService = unoConfigurationService;
        _client = client;

        _client.Ready -= ClientOnReady;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        var unoConfigurations = await _unoConfigurationService.GetUnoConfigurations();

        if (!unoConfigurations.Any())
        {
            await Logger.Log("No uno configurations has been found");
            return;
        }

        foreach (var unoConfiguration in unoConfigurations)
        {
            _cacheManager.AddUnoConfiguration(unoConfiguration.ChannelId, unoConfiguration.RoleId);
        }

        await Logger.Log($"Loaded {unoConfigurations.Count} uno configurations");
    }

    public async Task EnableUnoInChannel(SocketInteractionContext context, IChannel channel, IRole role)
    {
        var allUnoConfigurations = _cacheManager.GetAllUnoConfigurations();

        if (allUnoConfigurations.Contains(channel.Id))
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"#{channel.Name} is already configured to accept uno games!");
            return;
        }

        await _unoConfigurationService.AddUnoConfiguration(channel.Id, role.Id);
        _cacheManager.AddUnoConfiguration(channel.Id, role.Id);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = $"#{channel.Name} is now configured to accept uno games!");
    }

    public async Task DisableUnoInChannel(SocketInteractionContext context, IChannel channel)
    {
        var allUnoConfigurations = _cacheManager.GetAllUnoConfigurations();

        if (!allUnoConfigurations.Contains(channel.Id))
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"#{channel.Name} is not configured to accept uno games! No Need to disable there!");
            return;
        }

        await _unoConfigurationService.RemoveUnoConfiguration(channel.Id);
        _cacheManager.RemoveUnoConfiguration(channel.Id);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = $"#{channel.Name} is now disabled and no longer initialize uno games!");
    }
}