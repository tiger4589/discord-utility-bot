using System.Text.RegularExpressions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.KoCMemesServices;

public class KoCMemeService : IKoCMemeService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly ICacheManager _cacheManager;

    public KoCMemeService(DiscordSocketClient client, IConfiguration configuration, ICacheManager cacheManager)
    {
        _client = client;
        _configuration = configuration;
        _cacheManager = cacheManager;
        _client.Ready -= ClientOnReady;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        if (string.IsNullOrWhiteSpace(_configuration["KoCMemeChannelId"]))
        {
            return;
        }

        var channel = await _client.GetChannelAsync(ulong.Parse(_configuration["KoCMemeChannelId"]!));

        if (channel == null)
        {
            return;
        }

        await Logger.Log($"Watching {channel.Name}!!!!");

        _client.MessageReceived -= ClientOnMessageReceived;
        _client.MessageReceived += ClientOnMessageReceived;
    }

    private async Task ClientOnMessageReceived(SocketMessage arg)
    {
        var channelId = ulong.Parse(_configuration["KoCMemeChannelId"]!);

        if (arg.Channel.Id != channelId)
        {
            return;
        }

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

        if (arg.Attachments.Count > 0)
        {
            return;
        }

        string pattern = "([a-zA-Z0-9]+://)?([a-zA-Z0-9_]+:[a-zA-Z0-9_]+@)?([a-zA-Z0-9.-]+\\.[A-Za-z]{2,4})(:[0-9]+)?([^ ])";
        if (Regex.IsMatch(arg.Content, pattern))
        {
            return;
        }

        _cacheManager.AddDeletedMessageByBot(arg.Id);
        await arg.DeleteAsync();

        await Logger.Log($"Deleted message from {user.Mention} in {arg.Channel.Name}");
    }
}