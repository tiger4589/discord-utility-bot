using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IUnoConfigurationService
{
    Task AddUnoConfiguration(ulong channelId);
    Task RemoveUnoConfiguration(ulong channelId);
    Task<IList<UnoConfiguration>> GetUnoConfigurations();
}