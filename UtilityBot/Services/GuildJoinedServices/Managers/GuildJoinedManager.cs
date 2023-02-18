using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Contracts;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;

namespace UtilityBot.Services.GuildJoinedServices.Managers;

public class GuildJoinedManager : IGuildJoinedManager
{
    private readonly ICacheManager _cacheManager;
    private readonly DiscordSocketClient _client;
    private readonly IConfigurationService _configurationService;

    public GuildJoinedManager(ICacheManager cacheManager, DiscordSocketClient client, IConfigurationService configurationService) 
    {
        _cacheManager = cacheManager;
        _client = client;
        _configurationService = configurationService;
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
        //todo: let me know that I can't connect to the API!
        try
        {
            var connectedServers = guilds.Select(x => new ConnectedServer(x.Id, x.Name)).ToList();

            var configuration = await _configurationService.GetConfigurationsOfConnectedServers(connectedServers);
            var verifyConfiguration = await _configurationService.GetVerifyConfiguration();

            _cacheManager.InitializeCache(configuration, verifyConfiguration);
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