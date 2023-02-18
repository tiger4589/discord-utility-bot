using Discord.Interactions;

namespace UtilityBot.Services.PlayerServices;

public interface IPlayerService
{
    Task RequestVerification(SocketInteractionContext context, string recruitLink);
}