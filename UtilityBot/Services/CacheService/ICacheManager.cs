using UtilityBot.Contracts;
using UtilityBot.Domain.DomainObjects;
using VerifyConfiguration = UtilityBot.Contracts.VerifyConfiguration;

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
    void AddOrUpdate(LogConfiguration logConfiguration);
    void Remove(LogConfiguration logConfiguration);
    LogConfiguration? GetLogConfiguration();

    VerifyMessageConfiguration? GetVerifyMessageConfiguration();
    void AddOrUpdate(VerifyMessageConfiguration verifyMessageConfiguration);

    void AddOrUpdate(JokeConfiguration jokeConfiguration);
    IList<JokeConfiguration> GetJokeConfigurations();
    JokeConfiguration? GetJokeConfiguration(EJokeType jokeType);

    void AddOrUpdate(RumbleConfiguration configuration);
    RumbleConfiguration? GetRumbleConfiguration();
    void Add(RumbleMessageConfiguration configuration);
    IList<RumbleMessageConfiguration> GetRumbleMessageConfigurations();

    void AddOrUpdate(CapsProtectionConfiguration configuration);
    CapsProtectionConfiguration? GetCapsProtectionConfiguration();

    void AddOrUpdate(MagicEightBallConfiguration configuration);
    MagicEightBallConfiguration? GetMagicEightBallConfiguration();
    void Add(MagicEightBallResponse response);
    IList<MagicEightBallResponse> GetMagicEightBallResponses();
    void EnableMagicEightBall();
    void DisableMagicEightBall();


    void LoadEventsConfiguration(IList<EventsConfiguration> configurations);
    EventsConfiguration? GetEventConfiguration(EEventName eventType);
    void EnableEvent(EEventName eventType);
    void DisableEvent(EEventName eventType);


    IList<ulong> GetDeletedMessagesByBot();
    void AddDeletedMessageByBot(ulong messageId);

    void AddOrUpdate(CoderRequestVerification requestVerification);
    CoderRequestVerification? GetCoderRequestVerification();
}