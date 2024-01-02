using Discord;
using Discord.Interactions;

namespace UtilityBot.Services.MagicEightBall;

public interface IMagicEightBall
{
    Task AddResponse(SocketInteractionContext context, string response);
    Task AddConfiguration(SocketInteractionContext context, IChannel channel);
    Task Enable(SocketInteractionContext context, IChannel channel);
    Task Disable(SocketInteractionContext context, IChannel channel);

    Task Answer(SocketInteractionContext context, string question);
}