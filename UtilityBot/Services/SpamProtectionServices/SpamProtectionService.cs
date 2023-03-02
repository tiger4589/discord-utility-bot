using Discord.WebSocket;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.SpamProtectionServices;

public class SpamProtectionService : ISpamProtectionService
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IConfigurationService _configurationService;

    public SpamProtectionService(DiscordSocketClient client, ICacheManager cacheManager, IConfigurationService configurationService)
    {
        _client = client;
        _cacheManager = cacheManager;
        _configurationService = configurationService;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        var latestCapsProtectionConfiguration = await _configurationService.GetLatestCapsProtectionConfiguration();
        if (latestCapsProtectionConfiguration == null)
        {
            await Logger.Log($"Caps protection is not set!");
            return;
        }

        _cacheManager.AddOrUpdate(latestCapsProtectionConfiguration);
        await Logger.Log($"Caps protection is loaded!");

        _client.MessageReceived -= ClientOnMessageReceived;
        _client.MessageReceived += ClientOnMessageReceived;
    }

    private async Task ClientOnMessageReceived(SocketMessage arg)
    {

        if (arg.Author.IsBot)
        {
            return;
        }

        var capsProtectionConfiguration = _cacheManager.GetCapsProtectionConfiguration();
        if (capsProtectionConfiguration == null)
        {
            return;
        }

        if (arg.Content.Length < capsProtectionConfiguration.MinimumLength)
        {
            return;
        }

        int count = 0;
        foreach (var s in arg.Content)
        {
            if (char.IsUpper(s)) count++;
        }

        if ((count * 1.0) * 100.0 / (arg.Content.Length * 1.0) > capsProtectionConfiguration.MinimumPercentage)
        {
            var myMessage = await arg.Channel.SendMessageAsync($"{arg.Author.Mention} TOO MUCH CAPS! TOO MUCH CAPS! TOO MUCH CAPS!");
            await arg.DeleteAsync();
            await Task.Delay(1750);
            await myMessage.DeleteAsync();
        }
    }

    public async Task ForceInitialize()
    {
        await ClientOnReady();
    }
}