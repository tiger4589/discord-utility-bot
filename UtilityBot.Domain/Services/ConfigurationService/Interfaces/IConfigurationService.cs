using UtilityBot.Contracts;
using UtilityBot.Domain.DomainObjects;
using UserJoinConfiguration = UtilityBot.Contracts.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Contracts.UserJoinMessage;
using UserJoinRole = UtilityBot.Contracts.UserJoinRole;
using VerifyConfiguration = UtilityBot.Contracts.VerifyConfiguration;

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

    Task AddLogConfiguration(ulong guildId, ulong channelId);
    Task<LogConfiguration?> GetLogConfiguration(ulong guildId);
    Task RemoveLogConfiguration();

    Task AddOrUpdateVerifyMessageConfiguration(ulong guildId, ulong roleId, string message);
    Task<VerifyMessageConfiguration?> GetVerifyMessageConfiguration();

    Task AddOrUpdateJokeConfiguration(JokeConfiguration jokeConfiguration);
    Task<IList<JokeConfiguration>> GetJokeConfigurations();


    Task AddConfiguration(CapsProtectionConfiguration configuration);
    Task<CapsProtectionConfiguration?> GetLatestCapsProtectionConfiguration();
}