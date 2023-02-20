using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace UtilityBot.Services.MessageHandlers;

public class MessageHandler : IMessageHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    private const string UserName = "USER_NAME";
    private const string UserMention = "USER_MENTION";
    private const string ServerName = "SERVER_NAME";

    public MessageHandler(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public string ReplacePlaceHolders(string message, SocketGuildUser user)
    {
        string result = message;

        result = result
            .Replace(UserName, user.Username)
            .Replace(UserMention, user.Mention)
            .Replace(ServerName, _client.GetGuild(ulong.Parse(_configuration["ServerId"]!)).Name);

        return result;
    }
}