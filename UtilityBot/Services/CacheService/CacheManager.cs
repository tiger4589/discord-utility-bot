using System.Collections;
using UtilityBot.Contracts;

namespace UtilityBot.Services.CacheService;

public class CacheManager : ICacheManager
{
    private readonly Hashtable _userJoinConfiguration = new Hashtable();
    private readonly Hashtable _userJoinMessage = new Hashtable();
    private readonly Hashtable _userJoinRole = new Hashtable();

    private bool _isLoaded = false;

    public void InitializeCache(Configuration? configuration)
    {
        if (configuration == null)
        {
            //ToDo: Send a message, let the owner know.
            return;
        }

        ClearCache();

        InitializeUserJoinConfiguration(configuration);

        _isLoaded = true;
    }

    public void UpdateCache(Configuration? configuration)
    {
        if (configuration == null)
        {
            //ToDo: Send a message, let the owner know.
            return;
        }

        //Todo: if _isLoaded = false, something is wrong.. do something about it

        UpdateUserJoinConfiguration(configuration);
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
    }
}