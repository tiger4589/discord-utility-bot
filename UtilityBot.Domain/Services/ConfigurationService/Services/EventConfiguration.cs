using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.Services.ConfigurationService.Services;

public class EventConfiguration : IEventConfiguration
{
    private readonly UtilityBotContext _context;

    public EventConfiguration(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task<IList<EventsConfiguration>> GetConfigurations()
    {
        return await _context.EventsConfigurations!.AsNoTracking().ToListAsync();
    }

    public async Task EnableEvent(EEventName eventType)
    {
        var eventsConfiguration = await _context.EventsConfigurations!.SingleOrDefaultAsync(x => x.EventName == eventType.ToString());

        if (eventsConfiguration == null)
        {
            return;
        }

        eventsConfiguration.IsEnabled = true;
        await _context.SaveChangesAsync();
    }

    public async Task DisableEvent(EEventName eventType)
    {
        var eventsConfiguration = await _context.EventsConfigurations!.SingleOrDefaultAsync(x => x.EventName == eventType.ToString());

        if (eventsConfiguration == null)
        {
            return;
        }

        eventsConfiguration.IsEnabled = false;
        await _context.SaveChangesAsync();
    }
}