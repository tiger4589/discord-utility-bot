using Discord.WebSocket;

namespace UtilityBot.Services.MessageHandlers;

public interface IMessageHandler
{
    public string ReplacePlaceHolders(string message, SocketGuildUser user);
}