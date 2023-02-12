using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Contracts;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;

namespace UtilityBot.Services.GuildJoinedServices.Managers;

public class GuildJoinedManager : BaseApiCallService, IGuildJoinedManager
{
    private readonly ICacheManager _cacheManager;
    private readonly DiscordSocketClient _client;

    public GuildJoinedManager(IConfiguration configuration, ICacheManager cacheManager, DiscordSocketClient client) : base(configuration)
    {
        _cacheManager = cacheManager;
        _client = client;
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

            ServiceUrl = "configuration/get-servers-configuration";

            var configuration = await RequestApi<Configuration>(connectedServers);

            _cacheManager.InitializeCache(configuration);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task GetConfigurationOnJoin(SocketGuild guild)
    {
        var connectedServer = new ConnectedServer(guild.Id, guild.Name);

        ServiceUrl = "configuration/get-server-configuration";

        var configuration = await RequestApi<Configuration>(connectedServer);

        if (configuration == null)
        {
            return;
        }

        if (!configuration.UserJoinConfigurations.Any())
        {
            return;
        }

        _cacheManager.UpdateCache(configuration);
    }

    public override string? ServiceUrl { get; set; }
}