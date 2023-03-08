using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Services.JokesServices;

namespace UtilityBot.Services.MessageHandlers;

public interface IEmbedMessageBuilder
{
    Embed BuildVerificationEmbed(SocketUser user, string recruitLink);
    Embed BuildCoderRequestVerification(SocketUser user, string recruitLink, string allianceLink, string role);

    Embed BuildChuckNorrisFactEmbed(SocketInteractionContext context, ChuckNorrisJoke joke);
}