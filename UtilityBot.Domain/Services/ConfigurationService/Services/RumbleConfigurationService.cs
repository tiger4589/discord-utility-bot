using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.Services.ConfigurationService.Services;

public class RumbleConfigurationService : IRumbleConfigurationService
{
    private readonly UtilityBotContext _context;

    public RumbleConfigurationService(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task AddConfiguration(RumbleConfiguration configuration)
    {
        await _context.RumbleConfigurations!.AddAsync(configuration);
        await _context.SaveChangesAsync();
    }

    public async Task<RumbleConfiguration?> GetLatestConfiguration()
    {
        return await _context.RumbleConfigurations!.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    public async Task AddRumbleMessage(RumbleMessageConfiguration configuration)
    {
        await _context.RumbleMessageConfigurations!.AddAsync(configuration);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<RumbleMessageConfiguration>> GetRumbleMessages()
    {
        return await _context.RumbleMessageConfigurations!.AsNoTracking().ToListAsync();
    }
}