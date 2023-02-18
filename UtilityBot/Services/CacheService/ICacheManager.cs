using UtilityBot.Contracts;

namespace UtilityBot.Services.CacheService;

public interface ICacheManager
{
    void InitializeCache(Configuration? configuration, VerifyConfiguration? verifyConfiguration);
    void UpdateCache(Configuration? configuration);
    void AddOrUpdate(UserJoinRoleConfiguration userJoinRoleConfiguration);
    void AddOrUpdate(UserJoinMessageConfiguration userJoinMessageConfiguration);
    void AddOrUpdate(VerifyConfiguration configuration);
    Configuration? GetGuildOnJoinConfiguration(ulong guildId);
    VerifyConfiguration? GetVerifyConfiguration();
    void RemoveMessageConfiguration(ulong guildId);
    void RemoveRoleConfiguration(ulong guildId, ulong roleId);
}