using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace UtilityBot.Services.Uno.Manager;

public interface IUnoGameManager
{
    Task InitializeGame(SocketInteractionContext context);
    Task JoinGame(SocketInteractionContext context, Guid gameId);
    Task StartGame(SocketInteractionContext context, Guid gameId);

    Task ShowCards(SocketMessageComponent component);
    Task PlayCard(SocketInteractionContext context, Guid cardId);
    Task DrawCard(SocketInteractionContext context);
    Task PlayWildCard(SocketInteractionContext context, string color, Guid cardId);
}