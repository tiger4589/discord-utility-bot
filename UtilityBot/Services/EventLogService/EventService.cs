using Discord.Interactions;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.CacheService;

namespace UtilityBot.Services.EventLogService;

public class EventService : IEventService
{
    private readonly ICacheManager _cacheManager;
    private readonly IEventConfiguration _eventConfiguration;

    public EventService(ICacheManager cacheManager, IEventConfiguration eventConfiguration)
    {
        _cacheManager = cacheManager;
        _eventConfiguration = eventConfiguration;
    }

    public async Task EnableEvent(SocketInteractionContext context, EEventName eventType)
    {
        var eventsConfiguration = _cacheManager.GetEventConfiguration(eventType);
        if (eventsConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"Couldn't find configuration for {eventType} event.. something is weird.");
            return;
        }

        if (eventsConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"Logging for {eventType} is already enabled");
            return;
        }

        await _eventConfiguration.EnableEvent(eventType);
        _cacheManager.EnableEvent(eventType);
        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = $"Enabled logging for {eventType} event...");
    }

    public async Task DisableEvent(SocketInteractionContext context, EEventName eventType)
    {
        var eventsConfiguration = _cacheManager.GetEventConfiguration(eventType);
        if (eventsConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"Couldn't find configuration for {eventType} event.. something is weird.");
            return;
        }

        if (!eventsConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"Logging for {eventType} is already disabled");
            return;
        }

        await _eventConfiguration.DisableEvent(eventType);
        _cacheManager.DisableEvent(eventType);
        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = $"Disabled logging for {eventType} event...");
    }
}