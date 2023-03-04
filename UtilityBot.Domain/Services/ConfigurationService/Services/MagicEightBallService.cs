using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.Services.ConfigurationService.Services;

public class MagicEightBallService : IMagicEightBallService
{
    private readonly UtilityBotContext _context;

    public MagicEightBallService(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task AddConfiguration(MagicEightBallConfiguration configuration)
    {
        await _context.MagicEightBallConfigurations!.AddAsync(configuration);
        await _context.SaveChangesAsync();
    }

    public async Task<MagicEightBallConfiguration?> GetLatestConfiguration()
    {
        return await _context.MagicEightBallConfigurations!.AsNoTracking().OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task AddResponse(MagicEightBallResponse response)
    {
        await _context.MagicEightBallResponses!.AddAsync(response);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<MagicEightBallResponse>> GetResponses()
    {
        return await _context.MagicEightBallResponses!.AsNoTracking().ToListAsync();
    }

    public async Task Enable()
    {
        var magicEightBallConfiguration = await _context.MagicEightBallConfigurations!.OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
        if (magicEightBallConfiguration != null)
        {
            magicEightBallConfiguration.IsEnabled = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task Disable()
    {
        var magicEightBallConfiguration = await _context.MagicEightBallConfigurations!.OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
        if (magicEightBallConfiguration != null)
        {
            magicEightBallConfiguration.IsEnabled = false;
            await _context.SaveChangesAsync();
        }
    }
}