using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Contracts;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;
using UtilityBot.Services.UserJoinedServices;

namespace UtilityBot.Services.GuildJoinedServices.Managers;

public class GuildJoinedManager : IGuildJoinedManager
{
    private readonly ICacheManager _cacheManager;
    private readonly DiscordSocketClient _client;
    private readonly IConfigurationService _configurationService;
    private readonly IUserJoinedService _userJoinedService;

    public GuildJoinedManager(ICacheManager cacheManager, DiscordSocketClient client, IConfigurationService configurationService, IUserJoinedService userJoinedService) 
    {
        _cacheManager = cacheManager;
        _client = client;
        _configurationService = configurationService;
        _userJoinedService = userJoinedService;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        await GetConfigurationOnRun(_client.Guilds);
        _client.JoinedGuild += ClientOnJoinedGuild;
    }

    private async Task ClientOnJoinedGuild(SocketGuild arg)
    {
        await GetConfigurationOnJoin(arg);
    }

    private async Task GetConfigurationOnRun(IReadOnlyCollection<SocketGuild> guilds)
    {
        try
        {
            var connectedServers = guilds.Select(x => new ConnectedServer(x.Id, x.Name)).ToList();

            var configuration = await _configurationService.GetConfigurationsOfConnectedServers(connectedServers);
            var verifyConfiguration = await _configurationService.GetVerifyConfiguration();

            _cacheManager.InitializeCache(configuration, verifyConfiguration);
            await _userJoinedService.TriggerAfterRestart(configuration);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task GetConfigurationOnJoin(SocketGuild guild)
    {
        var connectedServer = new ConnectedServer(guild.Id, guild.Name);


        var configuration = await _configurationService.GetConfigurationsOfConnectedServer(connectedServer);

        if (!configuration.UserJoinConfigurations.Any())
        {
            return;
        }

        _cacheManager.UpdateCache(configuration);
    }
}