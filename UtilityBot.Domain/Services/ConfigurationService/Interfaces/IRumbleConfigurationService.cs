using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IRumbleConfigurationService
{
    Task AddConfiguration(RumbleConfiguration configuration);
    Task<RumbleConfiguration?> GetLatestConfiguration();

    Task AddRumbleMessage(RumbleMessageConfiguration  configuration);
    Task<IList<RumbleMessageConfiguration>> GetRumbleMessages();
}