using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using UtilityBot.Contracts;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;

namespace UtilityBot.Services.GuildJoinedServices.Managers;

public class GuildJoinedManager : BaseApiCallService, IGuildJoinedManager
{
    private readonly ICacheManager _cacheManager;
    private readonly string _apiBaseUrl;



    public GuildJoinedManager(IConfiguration configuration, ICacheManager cacheManager) : base(configuration)
    {
        _cacheManager = cacheManager;
        _apiBaseUrl = configuration["ApiBaseUrl"] ?? throw new InvalidOperationException($"API Base URL Can't be found!");
    }

    public async Task InitializeService(DiscordSocketClient client)
    {
        await GetConfigurationOnRun(client.Guilds);
        client.JoinedGuild += ClientOnJoinedGuild;
    }

    private async Task ClientOnJoinedGuild(SocketGuild arg)
    {
        await GetConfigurationOnJoin(arg);
    }

    private async Task GetConfigurationOnRun(IReadOnlyCollection<SocketGuild> guilds)
    {
        var connectedServers = guilds.Select(x => new ConnectedServer(x.Id, x.Name)).ToList();

        ServiceUrl = "configuration/get-servers-configuration";

        var configuration = await RequestApi<Configuration>(connectedServers);

        //todo: try not to block the slash commands from being initialized somehow
        _cacheManager.InitializeCache(configuration);
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