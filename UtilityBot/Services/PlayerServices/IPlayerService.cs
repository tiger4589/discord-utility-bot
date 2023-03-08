using Discord.Interactions;

namespace UtilityBot.Services.PlayerServices;

public interface IPlayerService
{
    Task RequestVerification(SocketInteractionContext context, string recruitLink);
    Task RequestCoderStatus(SocketInteractionContext context, string recruitLink, string allianceGameLink, string role);
}