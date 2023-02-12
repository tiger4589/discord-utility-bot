using UtilityBot.Contracts;

namespace UtilityBot.Services.CacheService;

public interface ICacheManager
{
    void InitializeCache(Configuration? configuration);
    void UpdateCache(Configuration? configuration);
}