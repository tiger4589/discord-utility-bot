using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.SpamProtectionServices;

public class SpamProtectionService : ISpamProtectionService
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IConfigurationService _configurationService;
    private readonly IConfiguration _configuration;

    public SpamProtectionService(DiscordSocketClient client, ICacheManager cacheManager, IConfigurationService configurationService, IConfiguration configuration)
    {
        _client = client;
        _cacheManager = cacheManager;
        _configurationService = configurationService;
        _configuration = configuration;
        _client.Ready -= ClientOnReady;
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

        var user = (SocketGuildUser)arg.Author;
        var guild = _client.GetGuild(ulong.Parse(_configuration["ServerId"]!));
        var me = guild.GetUser(_client.CurrentUser.Id);

        if (user.Roles.Max(x => x.Position) >= me.Roles.Max(x => x.Position))
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
            await Logger.Log($"Caps Protection! Warned {arg.Author.Username} for saying {arg.Content} in #{arg.Channel.Name}");
            var myMessage = await arg.Channel.SendMessageAsync($"{arg.Author.Mention} TOO MUCH CAPS! TOO MUCH CAPS! TOO MUCH CAPS!");
            _cacheManager.AddDeletedMessageByBot(arg.Id);
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