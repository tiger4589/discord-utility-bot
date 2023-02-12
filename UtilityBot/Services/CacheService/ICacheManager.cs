using UtilityBot.Contracts;

namespace UtilityBot.Services.CacheService;

public interface ICacheManager
{
    void InitializeCache(Configuration? configuration);
    void UpdateCache(Configuration? configuration);
    void AddOrUpdate(UserJoinRoleConfiguration userJoinRoleConfiguration);
    void AddOrUpdate(UserJoinMessageConfiguration userJoinMessageConfiguration);

    Configuration? GetGuildOnJoinConfiguration(ulong guildId);
}