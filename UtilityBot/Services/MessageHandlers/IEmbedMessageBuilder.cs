using Discord;
using Discord.WebSocket;

namespace UtilityBot.Services.MessageHandlers;

public interface IEmbedMessageBuilder
{
    Embed BuildVerificationEmbed(SocketUser user, string recruitLink);
}