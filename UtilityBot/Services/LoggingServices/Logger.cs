using Discord;
using Discord.WebSocket;
using UtilityBot.Services.CacheService;

namespace UtilityBot.Services.LoggingServices;

public static class Logger
{
    private static ICacheManager? _cacheManager;
    private static DiscordSocketClient? _client;
    public static void InitializeLogger(ICacheManager cacheManager, DiscordSocketClient client)
    {
        _cacheManager = cacheManager;
        _client = client;
    }        

    public static async Task Log(string message)
    {
        if (_cacheManager == null)
        {
            Console.WriteLine($"Cache Manager is not initialized, logging here: {message}");
            return;
        }

        if (_client == null)
        {
            Console.WriteLine($"Client is not initialized, logging here: {message}");
            return;
        }

        var logConfiguration = _cacheManager.GetLogConfiguration();
        if (logConfiguration == null)
        {
            return;
        }

        var channel = await _client.GetChannelAsync(logConfiguration.ChannelId) as ITextChannel;
        if (channel == null)
        {
            Console.WriteLine($"I can't find channel with id: {logConfiguration.ChannelId}");
            return;
        }

        await channel.SendMessageAsync(message);
    }
}