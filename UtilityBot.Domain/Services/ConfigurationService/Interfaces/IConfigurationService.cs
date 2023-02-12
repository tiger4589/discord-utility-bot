using UtilityBot.Contracts;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IConfigurationService
{
    Task<Configuration> GetConfigurationsOfConnectedServers(IList<ConnectedServer> connectedServers);
    Task<Configuration> GetConfigurationsOfConnectedServer(ConnectedServer connectedServer);

    Task AddUserJoinRoleConfiguration(UserJoinConfiguration userJoinConfiguration, UserJoinRole userJoinRole);
    Task AddUserJoinMessageConfiguration(UserJoinConfiguration userJoinConfiguration, UserJoinMessage userJoinMessage);
}