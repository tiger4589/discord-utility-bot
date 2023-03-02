using Discord;
using Discord.Interactions;

namespace UtilityBot.Services.RumbleServices;

public interface IRumbleService
{
    Task AddMessageConfiguration(SocketInteractionContext context, string message);
    Task AddConfiguration(SocketInteractionContext context, ITextChannel channel, IRole botRole, string emoji, IRole roleToPing, bool isJoinGame);
    Task Subscribe(SocketInteractionContext context);
    Task Unsubscribe(SocketInteractionContext context);
}