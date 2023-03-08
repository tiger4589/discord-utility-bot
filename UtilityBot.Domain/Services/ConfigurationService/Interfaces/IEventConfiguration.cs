using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Services;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IEventConfiguration
{
    Task<IList<EventsConfiguration>> GetConfigurations();
    Task EnableEvent(EEventName eventType);
    Task DisableEvent(EEventName eventType);
}