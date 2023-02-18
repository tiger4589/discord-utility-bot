using UtilityBot.Contracts;

namespace UtilityBot.Domain.Services.ConfigurationService.Interfaces;

public interface IConfigurationService
{
    Task<Configuration> GetConfigurationsOfConnectedServers(IList<ConnectedServer> connectedServers);
    Task<Configuration> GetConfigurationsOfConnectedServer(ConnectedServer connectedServer);
    Task<VerifyConfiguration?> GetVerifyConfiguration();

    Task AddUserJoinRoleConfiguration(UserJoinConfiguration userJoinConfiguration, UserJoinRole userJoinRole);
    Task AddUserJoinMessageConfiguration(UserJoinConfiguration userJoinConfiguration, UserJoinMessage userJoinMessage);
    Task AddVerifyConfiguration(VerifyConfiguration verifyConfiguration);

    Task RemoveUserJoinMessageConfiguration(ulong guildId);
    Task RemoveUserJoinRoleConfiguration(ulong guildId, ulong roleId);
}