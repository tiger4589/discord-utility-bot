using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using UtilityBot.Contracts;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.GuildJoinedServices.Interfaces;

namespace UtilityBot.Services.GuildJoinedServices.Managers;

public class GuildJoinedManager : IGuildJoinedManager
{
    private readonly HttpClient _httpClient;
    private readonly ICacheManager _cacheManager;
    private readonly string _apiBaseUrl;

    public GuildJoinedManager(IConfiguration configuration, HttpClient httpClient, ICacheManager cacheManager)
    {
        _httpClient = httpClient;
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

        var response = await _httpClient.PostAsync(
            string.Concat(_apiBaseUrl, "configuration/get-servers-configuration"),
            JsonContent.Create(connectedServers),
            default);
        //todo: try not to block the slash commands from being initialized somehow
        var configuration = await response.Content.ReadFromJsonAsync<Configuration?>();
        _cacheManager.InitializeCache(configuration);
    }

    private async Task GetConfigurationOnJoin(SocketGuild guild)
    {
        var connectedServer = new ConnectedServer(guild.Id, guild.Name);

        var response = await _httpClient.PostAsync(
            string.Concat(_apiBaseUrl, "configuration/get-server-configuration"),
            JsonContent.Create(connectedServer));

        var configuration = await response.Content.ReadFromJsonAsync<Configuration?>();

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
}