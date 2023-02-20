using System.Collections;
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
    private readonly Hashtable _userJoinConfiguration = new Hashtable();
    private readonly Hashtable _userJoinMessage = new Hashtable();
    private readonly Hashtable _userJoinRole = new Hashtable();

    private readonly Hashtable _verifyConfiguration = new Hashtable();

    private bool _isLoaded = false;

    public async void InitializeCache(Configuration? configuration, VerifyConfiguration? verifyConfiguration)
    {
        if (configuration == null)
        {
            await Logger.Log($"Tried to initialize a null configuration");
            return;
        }

        ClearCache();

        InitializeUserJoinConfiguration(configuration);

        if (verifyConfiguration != null)
        {
            AddOrUpdate(verifyConfiguration);
        }

        _isLoaded = true;
    }

    public async void UpdateCache(Configuration? configuration)
    {
        if (configuration == null)
        {
            await Logger.Log($"Tried to update a null configuration");
            return;
        }

        //Todo: if _isLoaded = false, something is wrong.. do something about it

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
    private readonly Hashtable _logConfiguration = new Hashtable();
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