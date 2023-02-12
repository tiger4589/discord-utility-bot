using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace UtilityBot.Client;

public class BotClient
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public BotClient(DiscordSocketClient client, IConfiguration configuration)
    {
        _configuration = configuration;
        _client = client;
    }

    public async Task StartClient()
    {
        var token = _configuration["BotToken"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}