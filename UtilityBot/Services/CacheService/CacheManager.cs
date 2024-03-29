﻿using System.Collections;
using UtilityBot.Contracts;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Services.LoggingServices;
using UserJoinConfiguration = UtilityBot.Contracts.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Contracts.UserJoinMessage;
using UserJoinRole = UtilityBot.Contracts.UserJoinRole;
using VerifyConfiguration = UtilityBot.Contracts.VerifyConfiguration;

namespace UtilityBot.Services.CacheService;

public class CacheManager : ICacheManager
{
    private readonly Hashtable _userJoinConfiguration = new();
    private readonly Hashtable _userJoinMessage = new();
    private readonly Hashtable _userJoinRole = new();

    private readonly Hashtable _verifyConfiguration = new();

    public async void InitializeCache(Configuration? configuration, VerifyConfiguration? verifyConfiguration)
    {
        if (configuration == null)
        {
            await Logger.Log("Tried to initialize a null configuration");
            return;
        }

        ClearCache();

        InitializeUserJoinConfiguration(configuration);

        if (verifyConfiguration != null)
        {
            AddOrUpdate(verifyConfiguration);
        }
    }

    public async void UpdateCache(Configuration? configuration)
    {
        if (configuration == null)
        {
            await Logger.Log("Tried to update a null configuration");
            return;
        }

        UpdateUserJoinConfiguration(configuration);
    }

    public void AddOrUpdate(UserJoinRoleConfiguration userJoinRoleConfiguration)
    {
        if (_userJoinConfiguration.ContainsKey(userJoinRoleConfiguration.UserJoinConfiguration.GuildId))
        {
            var userJoinConfiguration = (List<UserJoinConfiguration>?) _userJoinConfiguration[userJoinRoleConfiguration.UserJoinConfiguration.GuildId];
            if (userJoinConfiguration != null)
            {
                if (userJoinConfiguration.All(x => x.Action != userJoinRoleConfiguration.UserJoinConfiguration.Action))
                {
                    userJoinConfiguration.Add(userJoinRoleConfiguration.UserJoinConfiguration);
                }
            }
            else
            {
                _userJoinConfiguration[userJoinRoleConfiguration.UserJoinConfiguration.GuildId] = new List<UserJoinConfiguration> {userJoinRoleConfiguration.UserJoinConfiguration};
            }
        }
        else
        {
            _userJoinConfiguration.Add(userJoinRoleConfiguration.UserJoinConfiguration.GuildId, new List<UserJoinConfiguration>{userJoinRoleConfiguration.UserJoinConfiguration});
        }

        if (_userJoinRole.ContainsKey(userJoinRoleConfiguration.UserJoinRole.GuildId))
        {
            var userJoinRoles = (List<UserJoinRole>?)_userJoinRole[userJoinRoleConfiguration.UserJoinConfiguration.GuildId];
            if (userJoinRoles != null)
            {
                if (userJoinRoles.All(x => x.RoleId != userJoinRoleConfiguration.UserJoinRole.RoleId))
                {
                    userJoinRoles.Add(userJoinRoleConfiguration.UserJoinRole);
                }
            }
            else
            {
                _userJoinRole[userJoinRoleConfiguration.UserJoinConfiguration.GuildId] = new List<UserJoinRole> { userJoinRoleConfiguration.UserJoinRole };
            }
        }
        else
        {
            _userJoinRole.Add(userJoinRoleConfiguration.UserJoinRole.GuildId, new List<UserJoinRole>{userJoinRoleConfiguration.UserJoinRole});
        }
    }

    public void AddOrUpdate(UserJoinMessageConfiguration userJoinMessageConfiguration)
    {
        if (_userJoinConfiguration.ContainsKey(userJoinMessageConfiguration.UserJoinConfiguration.GuildId))
        {
            var userJoinConfiguration = (List<UserJoinConfiguration>?)_userJoinConfiguration[userJoinMessageConfiguration.UserJoinConfiguration.GuildId];
            if (userJoinConfiguration != null)
            {
                if (userJoinConfiguration.All(x => x.Action != userJoinMessageConfiguration.UserJoinConfiguration.Action))
                {
                    userJoinConfiguration.Add(userJoinMessageConfiguration.UserJoinConfiguration);
                }
            }
            else
            {
                _userJoinConfiguration[userJoinMessageConfiguration.UserJoinConfiguration.GuildId] = new List<UserJoinConfiguration> { userJoinMessageConfiguration.UserJoinConfiguration };
            }
        }
        else
        {
            _userJoinConfiguration.Add(userJoinMessageConfiguration.UserJoinConfiguration.GuildId, new List<UserJoinConfiguration> { userJoinMessageConfiguration.UserJoinConfiguration });
        }

        if (_userJoinMessage.ContainsKey(userJoinMessageConfiguration.UserJoinMessage.GuildId))
        {
            var userJoinMessages = (List<UserJoinMessage>?)_userJoinMessage[userJoinMessageConfiguration.UserJoinConfiguration.GuildId];
            if (userJoinMessages != null)
            {
                userJoinMessages.Clear();
                userJoinMessages.Add(new UserJoinMessage(userJoinMessageConfiguration.UserJoinMessage.GuildId,
                    userJoinMessageConfiguration.UserJoinMessage.Message,
                    userJoinMessageConfiguration.UserJoinMessage.IsPrivate,
                    userJoinMessageConfiguration.UserJoinMessage.ChannelId));
            }
            else
            {
                _userJoinMessage[userJoinMessageConfiguration.UserJoinConfiguration.GuildId] = new List<UserJoinMessage> { userJoinMessageConfiguration.UserJoinMessage };
            }
        }
        else
        {
            _userJoinMessage.Add(userJoinMessageConfiguration.UserJoinMessage.GuildId, new List<UserJoinMessage> { userJoinMessageConfiguration.UserJoinMessage });
        }
    }

    public void AddOrUpdate(VerifyConfiguration configuration)
    {
        _verifyConfiguration.Clear();
        _verifyConfiguration.Add("verify", configuration);
    }

    public VerifyConfiguration? GetVerifyConfiguration()
    {
        if (_verifyConfiguration.Count == 0) return null;

        return (VerifyConfiguration?)_verifyConfiguration["verify"];
    }

    public void RemoveMessageConfiguration(ulong guildId)
    {
        _userJoinMessage.Clear();

        var userJoinConfiguration = (List<UserJoinConfiguration>?)_userJoinConfiguration[guildId];
        if (userJoinConfiguration == null)
        {
            return;
        }

        if (userJoinConfiguration.Any(x => x.Action == ActionTypeNames.SendMessage))
        {
            userJoinConfiguration.Remove(userJoinConfiguration.Single(x => x.Action == ActionTypeNames.SendMessage));
        }
    }

    public void RemoveRoleConfiguration(ulong guildId, ulong roleId)
    {
        var userJoinRoles = (List<UserJoinRole>?)_userJoinRole[guildId];

        if (userJoinRoles != null)
        {
            if (userJoinRoles.Count > 1)
            {
                userJoinRoles.Remove(userJoinRoles.Single(x => x.RoleId == roleId));
            }
            else
            {
                _userJoinRole.Clear();
            }
        }

        var userJoinConfiguration = (List<UserJoinConfiguration>?)_userJoinConfiguration[guildId];
        if (userJoinConfiguration == null)
        {
            return;
        }

        if (userJoinConfiguration.Any(x => x.Action == ActionTypeNames.AddRole))
        {
            userJoinConfiguration.Remove(userJoinConfiguration.Single(x => x.Action == ActionTypeNames.AddRole));
        }
    }
    private readonly Hashtable _logConfiguration = new();
    public void AddOrUpdate(LogConfiguration logConfiguration)
    {
        var configuration = (LogConfiguration?)_logConfiguration["log_conf"];
        if (configuration != null)
        {
            _logConfiguration.Clear();
        }

        _logConfiguration.Add("log_conf", logConfiguration);
    }

    public void Remove(LogConfiguration logConfiguration)
    {
        _logConfiguration.Clear();
    }

    public LogConfiguration? GetLogConfiguration()
    {
        return (LogConfiguration?)_logConfiguration["log_conf"];
    }

    private readonly Hashtable _verifyMessageConfiguration = new();
    public VerifyMessageConfiguration? GetVerifyMessageConfiguration()
    {
        return (VerifyMessageConfiguration?)_verifyMessageConfiguration["conf"];
    }

    public void AddOrUpdate(VerifyMessageConfiguration verifyMessageConfiguration)
    {
        _verifyMessageConfiguration.Clear();

        _verifyMessageConfiguration.Add("conf", verifyMessageConfiguration);
    }

    private readonly Hashtable _jokesConfiguration = new();

    public void AddOrUpdate(JokeConfiguration jokeConfiguration)
    {
        var currentConfiguration = (JokeConfiguration?)_jokesConfiguration[jokeConfiguration.JokeType];
        if (currentConfiguration == null)
        {
            _jokesConfiguration.Add(jokeConfiguration.JokeType, jokeConfiguration);
            return;
        }

        currentConfiguration.ChannelId = jokeConfiguration.ChannelId;
        currentConfiguration.IsEnabled = jokeConfiguration.IsEnabled;
    }

    public IList<JokeConfiguration> GetJokeConfigurations()
    {
        var result = new List<JokeConfiguration>();
        foreach (DictionaryEntry entry in _jokesConfiguration)
        {
            if (entry.Value == null)
            {
                continue;
            }

            result.Add((JokeConfiguration)entry.Value);
        }

        return result;
    }

    public JokeConfiguration? GetJokeConfiguration(EJokeType jokeType)
    {
        return (JokeConfiguration?)_jokesConfiguration[jokeType];
    }

    private readonly Hashtable _rumbleConfiguration = new();

    public void AddOrUpdate(RumbleConfiguration configuration)
    {
        if (_rumbleConfiguration.ContainsKey("conf"))
        {
            _rumbleConfiguration.Remove("conf");
        }

        _rumbleConfiguration.Add("conf", configuration);
    }

    public RumbleConfiguration? GetRumbleConfiguration()
    {
        return (RumbleConfiguration?)_rumbleConfiguration["conf"];
    }

    public void Add(RumbleMessageConfiguration configuration)
    {
        List<RumbleMessageConfiguration> messages;
        if (_rumbleConfiguration.ContainsKey("msg_conf"))
        {
            messages = (List<RumbleMessageConfiguration>)_rumbleConfiguration["msg_conf"]!;
            messages.Add(configuration);
        }
        else
        {
            messages = new List<RumbleMessageConfiguration> { configuration };
            _rumbleConfiguration["msg_conf"] = messages;
        }
    }

    public IList<RumbleMessageConfiguration> GetRumbleMessageConfigurations()
    {
        return (List<RumbleMessageConfiguration>)_rumbleConfiguration["msg_conf"]!;
    }

    private readonly Hashtable _capsConfiguration = new();
    public void AddOrUpdate(CapsProtectionConfiguration configuration)
    {
        if (_capsConfiguration.ContainsKey("conf"))
        {
            _capsConfiguration.Remove("conf");
        }

        _capsConfiguration.Add("conf", configuration);
    }

    public CapsProtectionConfiguration? GetCapsProtectionConfiguration()
    {
        return (CapsProtectionConfiguration?)_capsConfiguration["conf"];
    }

    private readonly Hashtable _8BallConfiguration = new();
    public void AddOrUpdate(MagicEightBallConfiguration configuration)
    {
        if (_8BallConfiguration.ContainsKey("conf"))
        {
            var magicEightBallConfigurations = (List<MagicEightBallConfiguration>)_8BallConfiguration["conf"]!;
            if (magicEightBallConfigurations.Any(x => x.ChannelId == configuration.ChannelId))
            {
                magicEightBallConfigurations.Remove(magicEightBallConfigurations.Single(x => x.ChannelId == configuration.ChannelId));
            }
            magicEightBallConfigurations.Add(configuration);
            return;
        }

        _8BallConfiguration.Add("conf", new List<MagicEightBallConfiguration> { configuration });
    }

    public List<MagicEightBallConfiguration>? GetMagicEightBallConfiguration()
    {
        return (List<MagicEightBallConfiguration>?)_8BallConfiguration["conf"];
    }

    public void Add(MagicEightBallResponse response)
    {
        List<MagicEightBallResponse> responses;
        if (_8BallConfiguration.ContainsKey("msg_conf"))
        {
            responses = (List<MagicEightBallResponse>)_8BallConfiguration["msg_conf"]!;
            responses.Add(response);
        }
        else
        {
            responses = new List<MagicEightBallResponse> { response };
            _8BallConfiguration["msg_conf"] = responses;
        }
    }

    public IList<MagicEightBallResponse> GetMagicEightBallResponses()
    {
        return (List<MagicEightBallResponse>)_8BallConfiguration["msg_conf"]!;
    }

    public void EnableMagicEightBall(ulong channelId)
    {
        if (_8BallConfiguration.ContainsKey("conf"))
        {
            List<MagicEightBallConfiguration> conf = (List<MagicEightBallConfiguration>)_8BallConfiguration["conf"]!;
            var magicEightBallConfiguration = conf.SingleOrDefault(x => x.ChannelId == channelId);
            if (magicEightBallConfiguration != null)
            {
                magicEightBallConfiguration.IsEnabled = true;
            }
        }
    }

    public void DisableMagicEightBall(ulong channelId)
    {
        if (_8BallConfiguration.ContainsKey("conf"))
        {
            List<MagicEightBallConfiguration> conf = (List<MagicEightBallConfiguration>)_8BallConfiguration["conf"]!;
            var magicEightBallConfiguration = conf.SingleOrDefault(x => x.ChannelId == channelId);
            if (magicEightBallConfiguration != null)
            {
                magicEightBallConfiguration.IsEnabled = false;
            }
        }
    }

    private readonly Hashtable _eventsConf = new();

    public void LoadEventsConfiguration(IList<EventsConfiguration> configurations)
    {
        _eventsConf.Clear();
        _eventsConf.Add("conf", configurations);
    }

    public EventsConfiguration? GetEventConfiguration(EEventName eventType)
    {
        if (!_eventsConf.ContainsKey("conf"))
        {
            return null;
        }

        var eventsConfigurations = (IList<EventsConfiguration>?)_eventsConf["conf"];
        if (eventsConfigurations == null)
        {
            return null;
        }

        var singleOrDefault = eventsConfigurations.SingleOrDefault(x => x.EventName == $"{eventType}");
        return singleOrDefault;
    }

    public void EnableEvent(EEventName eventType)
    {
        if (!_eventsConf.ContainsKey("conf"))
        {
            return;
        }

        var eventsConfigurations = (IList<EventsConfiguration>?)_eventsConf["conf"];

        var singleOrDefault = eventsConfigurations?.SingleOrDefault(x => x.EventName == $"{eventType}");
        if (singleOrDefault == null)
        {
            return;
        }

        singleOrDefault.IsEnabled = true;
    }

    public void DisableEvent(EEventName eventType)
    {
        if (!_eventsConf.ContainsKey("conf"))
        {
            return;
        }

        var eventsConfigurations = (IList<EventsConfiguration>?)_eventsConf["conf"];

        var singleOrDefault = eventsConfigurations?.SingleOrDefault(x => x.EventName == $"{eventType}");
        if (singleOrDefault == null)
        {
            return;
        }

        singleOrDefault.IsEnabled = false;
    }

    private readonly Hashtable _deletedMessages = new();
    public IList<ulong> GetDeletedMessagesByBot()
    {
        if (!_deletedMessages.ContainsKey("messages"))
        {
            return new List<ulong>();
        }

        var deletedMessage = (List<ulong>?)_deletedMessages["messages"];
        if (deletedMessage == null)
        {
            return new List<ulong>();
        }

        return deletedMessage;
    }

    public void AddDeletedMessageByBot(ulong messageId)
    {
        if (!_deletedMessages.ContainsKey("messages"))
        {
            _deletedMessages.Add("messages", new List<ulong> {messageId});
            return;
        }

        var deletedMessage = (List<ulong>?)_deletedMessages["messages"];
        if (deletedMessage == null)
        {
            _deletedMessages.Clear();
            _deletedMessages.Add("messages", new List<ulong> { messageId });
            return;
        }

        deletedMessage.Add(messageId);
    }

    private readonly Hashtable _coderVerification = new();
    public void AddOrUpdate(CoderRequestVerification requestVerification)
    {
        _coderVerification.Clear();
        _coderVerification.Add("conf", requestVerification);
    }

    public CoderRequestVerification? GetCoderRequestVerification()
    {
        if (!_coderVerification.ContainsKey("conf"))
        {
            return null;
        }

        return (CoderRequestVerification?)_coderVerification["conf"];
    }

    private readonly Hashtable _unoConfiguration  = new();
    private readonly Hashtable _unoRoleConfiguration = new();

    private void AddChannelRoleToPing(ulong channelId, ulong roleId)
    {
        if (_unoRoleConfiguration.ContainsKey(channelId))
        {
            _unoRoleConfiguration.Remove(channelId);
        }

        _unoRoleConfiguration.Add(channelId, roleId);
    }

    public ulong? GetRoleIdForChannel(ulong channelId)
    {
        var value = _unoRoleConfiguration[channelId];
        if (value == null)
        {
            return null;
        }

        return ulong.Parse(value.ToString() ?? string.Empty);
    }

    public void AddUnoConfiguration(ulong channelId, ulong roleId)
    {
        AddChannelRoleToPing(channelId, roleId);
        if (!_unoConfiguration.ContainsKey("conf"))
        {
            List<ulong> list = new List<ulong> {channelId};
            _unoConfiguration.Add("conf", list);
            return;
        }

        var ids = (List<ulong>?)_unoConfiguration["conf"];

        if (ids == null)
        {
            _unoConfiguration.Clear();
            List<ulong> list = new List<ulong> { channelId };
            _unoConfiguration.Add("conf", list);
            return;
        }

        ids.Add(channelId);
    }

    public void RemoveUnoConfiguration(ulong channelId)
    {
        if (!_unoConfiguration.ContainsKey("conf"))
        {
            return;
        }

        var ids = (List<ulong>?)_unoConfiguration["conf"];

        if (ids == null)
        {
            return;
        }

        if (ids.Contains(channelId))
        {
            ids.Remove(channelId);
        }
    }

    public IList<ulong> GetAllUnoConfigurations()
    {
        if (!_unoConfiguration.ContainsKey("conf"))
        {
            return new List<ulong>();
        }

        var ids = (List<ulong>?)_unoConfiguration["conf"];

        if (ids == null)
        {
            return new List<ulong>();
        }

        return ids;
    }

    public Configuration? GetGuildOnJoinConfiguration(ulong guildId)
    {
        if (!_userJoinConfiguration.ContainsKey(guildId))
        {
            return null;
        }

        var userJoinConfigurations = (List<UserJoinConfiguration>?) _userJoinConfiguration[guildId];

        var userJoinMessages = (List<UserJoinMessage>?)_userJoinMessage[guildId];

        var userJoinRoles = (List<UserJoinRole>?)_userJoinRole[guildId];

        return new Configuration(userJoinConfigurations!, userJoinMessages ?? new List<UserJoinMessage>(), userJoinRoles ?? new List<UserJoinRole>());
    }

    private void RemoveKeyIfExist(Hashtable table, ulong key)
    {
        if (table.ContainsKey(key))
        {
            table.Remove(key);
        }
    }

    private void UpdateUserJoinConfiguration(Configuration configuration)
    {
        var keyValuePairs = configuration.UserJoinConfigurations.GroupBy(x => x.GuildId)
            .Select(x =>
                new KeyValuePair<ulong, List<UserJoinConfiguration>>(x.Key, x.Where(y => y.GuildId == x.Key).ToList()));

        foreach (var keyValuePair in keyValuePairs)
        {
            RemoveKeyIfExist(_userJoinConfiguration, keyValuePair.Key);
            _userJoinConfiguration.Add(keyValuePair.Key, keyValuePair.Value);

            if (keyValuePair.Value.Any())
            {
                foreach (var userJoinConfiguration in keyValuePair.Value)
                {
                    if (userJoinConfiguration.Action == ActionTypeNames.SendMessage)
                    {
                        RemoveKeyIfExist(_userJoinMessage, keyValuePair.Key);
                        _userJoinMessage.Add(userJoinConfiguration.GuildId, configuration.UserJoinMessages.Where(x => x.GuildId == userJoinConfiguration.GuildId).ToList());
                    }

                    if (userJoinConfiguration.Action == ActionTypeNames.AddRole)
                    {
                        RemoveKeyIfExist(_userJoinRole, keyValuePair.Key);
                        _userJoinRole.Add(userJoinConfiguration.GuildId, configuration.UserJoinRoles.Where(x => x.GuildId == userJoinConfiguration.GuildId).ToList());
                    }
                }
            }
        }
    }

    private void InitializeUserJoinConfiguration(Configuration configuration)
    {
        var keyValuePairs = configuration.UserJoinConfigurations.GroupBy(x => x.GuildId)
            .Select(x =>
                new KeyValuePair<ulong, List<UserJoinConfiguration>>(x.Key, x.Where(y => y.GuildId == x.Key).ToList()));

        foreach (var keyValuePair in keyValuePairs)
        {
            _userJoinConfiguration.Add(keyValuePair.Key, keyValuePair.Value);

            if (keyValuePair.Value.Any())
            {
                foreach (var userJoinConfiguration in keyValuePair.Value)
                {
                    if (userJoinConfiguration.Action == ActionTypeNames.SendMessage)
                    {
                        _userJoinMessage.Add(userJoinConfiguration.GuildId, configuration.UserJoinMessages.Where(x => x.GuildId == userJoinConfiguration.GuildId).ToList());
                    }

                    if (userJoinConfiguration.Action == ActionTypeNames.AddRole)
                    {
                        _userJoinRole.Add(userJoinConfiguration.GuildId, configuration.UserJoinRoles.Where(x=>x.GuildId == userJoinConfiguration.GuildId).ToList());
                    }
                }
            }
        }
    }

    private void ClearCache()
    {
        _userJoinConfiguration.Clear();
        _userJoinMessage.Clear();
        _userJoinRole.Clear();
        _verifyConfiguration.Clear();
    }
}