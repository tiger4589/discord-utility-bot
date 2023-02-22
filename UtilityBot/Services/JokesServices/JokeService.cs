using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.EventArguments;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.JokesServices;

public class JokeService : BaseApiCallService, IJokeService
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IConfigurationService _configurationService;
    private readonly IConfiguration _configuration;

    public JokeService(DiscordSocketClient client, ICacheManager cacheManager, IConfigurationService configurationService, IConfiguration configuration) : base(configuration)
    {
        _client = client;
        _cacheManager = cacheManager;
        _configurationService = configurationService;
        _configuration = configuration;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        var jokeConfigurations = await _configurationService.GetJokeConfigurations();
        if (!jokeConfigurations.Any())
        {
            return;
        }

        foreach (var jokeConfiguration in jokeConfigurations)
        {
            _cacheManager.AddOrUpdate(jokeConfiguration);
        }

        await Logger.Log("Jokes configuration has been loaded");
    }

    public async Task InitializeDadJokes(SocketInteractionContext context, string channelName)
    {
        ulong guildId = ulong.Parse(_configuration["ServerId"]!);

        var guild = _client.GetGuild(guildId);

        var textChannel = await guild.CreateTextChannelAsync(channelName, prop =>
        {
            prop.Topic = "Serve yourself, use /dadjoke for a random dad joke!";
        });

        await InitializeDadJokes(context, textChannel.Id);
    }

    public async Task InitializeDadJokes(SocketInteractionContext context, ITextChannel channel)
    {
        await InitializeDadJokes(context, channel.Id);
    }

    private async Task InitializeDadJokes(SocketInteractionContext context, ulong channelId)
    {
        var jokeConfiguration = _cacheManager.GetJokeConfiguration(EJokeType.DadJoke);
        if (jokeConfiguration != null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration has already been initialized, you either enable or disable them now!");
            return;
        }

        var configuration = new JokeConfiguration
        {
            ChannelId = channelId,
            IsEnabled = true,
            JokeType = EJokeType.DadJoke
        };

        await _configurationService.AddOrUpdateJokeConfiguration(configuration);

        _cacheManager.AddOrUpdate(configuration);

        RaiseEvent(DadJokesInitialized, new ConfigurationServiceEventArgs(context, "Dad Jokes have been initialized"));
    }

    public async Task EnableDadJokes(SocketInteractionContext context)
    {
        var jokeConfiguration = _cacheManager.GetJokeConfiguration(EJokeType.DadJoke);
        if (jokeConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in the first place.");
            return;
        }

        if (jokeConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Dad Jokes are already enabled");
            return;
        }

        jokeConfiguration.IsEnabled = true;

        await _configurationService.AddOrUpdateJokeConfiguration(jokeConfiguration);
        _cacheManager.AddOrUpdate(jokeConfiguration);

        RaiseEvent(DadJokesEnabled, new ConfigurationServiceEventArgs(context, "Dad Jokes have been enabled"));
    }

    public async Task DisableDadJokes(SocketInteractionContext context)
    {
        var jokeConfiguration = _cacheManager.GetJokeConfiguration(EJokeType.DadJoke);
        if (jokeConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Configuration isn't set in the first place.");
            return;
        }

        if (!jokeConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Dad Jokes are already disabled");
            return;
        }

        jokeConfiguration.IsEnabled = false;

        await _configurationService.AddOrUpdateJokeConfiguration(jokeConfiguration);
        _cacheManager.AddOrUpdate(jokeConfiguration);

        RaiseEvent(DadJokesDisabled, new ConfigurationServiceEventArgs(context, "Dad Jokes have been disabled"));
    }

    public async Task GetRandomDadJoke(SocketInteractionContext context)
    {
        var jokeConfiguration = _cacheManager.GetJokeConfiguration(EJokeType.DadJoke);
        if (jokeConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
            {
                prop.Content = "Dad jokes are not yet enabled.";
            });
            return;
        }

        if (!jokeConfiguration.IsEnabled)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
            {
                prop.Content = "Dad jokes are not currently enabled.";
            });
            return;
        }

        if (context.Channel.Id != jokeConfiguration.ChannelId)
        {
            var socketGuildChannel = context.Guild.GetChannel(jokeConfiguration.ChannelId);
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
            {
                prop.Content = $"Dad jokes are only available in #{socketGuildChannel.Name}";
            });
            return;
        }

        var apiUrl = _configuration["DadJokeUrl"];
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
            {
                prop.Content = $"Problem with my configuration.. notifying moderators";
            });
            await Logger.Log($"I couldn't retrieve dad joke api URL from my settings.. fix please");
            return;
        }

        ServiceUrl = apiUrl;

        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "User-Agent", "discord-utility-bot" }
        };

        var dadJoke = await GetApiFromServiceUrl<DadJoke>(headers);

        if (dadJoke == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
            {
                prop.Content = $"Problem getting a dad joke.. notifying moderators";
            });
            await Logger.Log($"I couldn't get a dad joke from api, can you check?");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(context.User.Mention);
        sb.AppendLine(dadJoke.Joke);

        await context.Channel.SendMessageAsync(sb.ToString());
    }

    private protected void RaiseEvent(EventHandler<ConfigurationServiceEventArgs>? handler,
        ConfigurationServiceEventArgs args)
    {
        handler?.Invoke(this, args);
    }

    public event EventHandler<ConfigurationServiceEventArgs>? DadJokesInitialized;
    public event EventHandler<ConfigurationServiceEventArgs>? DadJokesEnabled;
    public event EventHandler<ConfigurationServiceEventArgs>? DadJokesDisabled;

    public override string? ServiceUrl { get; set; }
}

public class DadJoke
{
    public string Id { get; set; } = null!;
    public string Joke { get; set; } = null!;
    public int Status { get; set; }
}