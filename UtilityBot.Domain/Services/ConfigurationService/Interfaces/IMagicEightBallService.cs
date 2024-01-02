using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IMagicEightBallService
{
    Task AddResponse(MagicEightBallResponse response);
    Task<IList<MagicEightBallResponse>> GetResponses();
    Task AddConfiguration(MagicEightBallConfiguration configuration);
    Task<string> Enable(ulong channelId);
    Task<string> Disable(ulong channelId);
    Task<IEnumerable<MagicEightBallConfiguration>> GetConfigurations();
}