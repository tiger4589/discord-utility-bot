using Microsoft.EntityFrameworkCore;
using UtilityBot.Contracts;
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
        var magicEightBallConfiguration = await _context.MagicEightBallConfigurations!.SingleOrDefaultAsync(x => x.ChannelId == configuration.ChannelId);
        if (magicEightBallConfiguration != null)
        {
            if (!magicEightBallConfiguration.IsEnabled)
            {
                magicEightBallConfiguration.IsEnabled = true;
                await _context.SaveChangesAsync();
            }

            return;
        }

        await _context.MagicEightBallConfigurations!.AddAsync(configuration);
        await _context.SaveChangesAsync();
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

    public async Task<string> Enable(ulong channelId)
    {
        var magicEightBallConfiguration = await _context.MagicEightBallConfigurations!.SingleOrDefaultAsync(x => x.ChannelId == channelId);

        if (magicEightBallConfiguration == null)
        {
            return "Magic Eight Ball isn't configured for this channel. Please add the configuration.";
        }

        if (magicEightBallConfiguration.IsEnabled)
        {
            return "Magic Eight Ball is already enabled for this channel.";
        }

        magicEightBallConfiguration.IsEnabled = true;
        await _context.SaveChangesAsync();
        return "Magic Eight Ball is now enabled for this channel.";
    }

    public async Task<string> Disable(ulong channelId)
    {
        var magicEightBallConfiguration = await _context.MagicEightBallConfigurations!.SingleOrDefaultAsync(x => x.ChannelId == channelId);

        if (magicEightBallConfiguration == null)
        {
            return "Magic Eight Ball isn't configured for this channel. No need to disable it, right?";
        }

        if (!magicEightBallConfiguration.IsEnabled)
        {
            return "Magic Eight Ball is already disabled for this channel.";
        }

        magicEightBallConfiguration.IsEnabled = false;
        await _context.SaveChangesAsync();
        return "Magic Eight Ball is now disabled for this channel.";
    }

    public async Task<IEnumerable<MagicEightBallConfiguration>> GetConfigurations()
    {
        return await _context.MagicEightBallConfigurations!.AsNoTracking().ToListAsync();
    }
}