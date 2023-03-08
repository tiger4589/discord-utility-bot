using Discord.Interactions;
using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Services.EventLogService;

public interface IEventService
{
    Task EnableEvent(SocketInteractionContext context, EEventName eventType);
    Task DisableEvent(SocketInteractionContext context, EEventName eventType);
}