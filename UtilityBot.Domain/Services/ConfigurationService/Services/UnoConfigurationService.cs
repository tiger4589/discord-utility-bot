using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.Services.ConfigurationService.Services;

public class UnoConfigurationService : IUnoConfigurationService
{
    private readonly UtilityBotContext _context;

    public UnoConfigurationService(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task AddUnoConfiguration(ulong channelId, ulong roleId)
    {
        await _context.UnoConfigurations!.AddAsync(new UnoConfiguration
        {
            ChannelId = channelId,
            RoleId = roleId
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveUnoConfiguration(ulong channelId)
    {
        var unoConfiguration = await _context.UnoConfigurations!.SingleOrDefaultAsync(x => x.ChannelId == channelId);
        if (unoConfiguration == null)
        {
            return;
        }

        _context.UnoConfigurations!.Remove(unoConfiguration);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<UnoConfiguration>> GetUnoConfigurations()
    {
        return await _context.UnoConfigurations!.AsNoTracking().ToListAsync();
    }
}