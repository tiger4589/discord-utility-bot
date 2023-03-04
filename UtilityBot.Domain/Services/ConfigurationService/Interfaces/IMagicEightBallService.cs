using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IMagicEightBallService
{
    Task AddConfiguration(MagicEightBallConfiguration configuration);
    Task<MagicEightBallConfiguration?> GetLatestConfiguration();

    Task AddResponse(MagicEightBallResponse response);
    Task<IList<MagicEightBallResponse>> GetResponses();

    Task Enable();
    Task Disable();
}