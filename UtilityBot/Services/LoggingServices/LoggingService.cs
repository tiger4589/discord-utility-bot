using Discord.Commands;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.EventArguments;
using UtilityBot.Services.CacheService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace UtilityBot.Services.LoggingServices;

public class LoggingService : ILoggingService
{
    private readonly DiscordSocketClient _client;
    private readonly IEventConfiguration _eventConfiguration;
    private readonly ICacheManager _cacheManager;
    private readonly IConfigurationService _configurationService;
    private readonly IConfiguration _configuration;

    public LoggingService(DiscordSocketClient client, IServiceProvider serviceProvider,
        IEventConfiguration eventConfiguration)
    {
        _client = client;
        _eventConfiguration = eventConfiguration;
        _cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        _configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();

    }

    public async Task InitializeService()
    {
        var logConfiguration =
            await _configurationService.GetLogConfiguration(ulong.Parse(_configuration["ServerId"]!));
        if (logConfiguration != null)
        {
            _cacheManager.AddOrUpdate(logConfiguration);
        }

        _client.Ready -= ClientOnReady;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        _client.Log -= LogAsync;
        _client.Log += LogAsync;
        _client.MessageReceived -= MessageReceived;
        _client.MessageReceived += MessageReceived;

        var eventsConfigurations = await _eventConfiguration.GetConfigurations();
        _cacheManager.LoadEventsConfiguration(eventsConfigurations);

        await Logger.Log($"Loaded {eventsConfigurations.Count} events configurations");

        _client.UserJoined -= ClientOnUserJoined;
        _client.UserJoined += ClientOnUserJoined;

        _client.UserUnbanned -= ClientOnUserUnbanned;
        _client.UserUnbanned += ClientOnUserUnbanned;

        _client.UserLeft -= ClientOnUserLeft;
        _client.UserLeft += ClientOnUserLeft;

        _client.UserBanned -= ClientOnUserBanned;
        _client.UserBanned += ClientOnUserBanned;

        _client.RoleCreated -= ClientOnRoleCreated;
        _client.RoleCreated += ClientOnRoleCreated;

        _client.RoleDeleted -= ClientOnRoleDeleted;
        _client.RoleDeleted += ClientOnRoleDeleted;

        _client.RoleUpdated -= ClientOnRoleUpdated;
        _client.RoleUpdated += ClientOnRoleUpdated;

        _client.GuildMemberUpdated -= ClientOnGuildMemberUpdated;
        _client.GuildMemberUpdated += ClientOnGuildMemberUpdated;

        _client.UserUpdated -= ClientOnUserUpdated;
        _client.UserUpdated += ClientOnUserUpdated;

        _client.MessageUpdated -= ClientOnMessageUpdated;
        _client.MessageUpdated += ClientOnMessageUpdated;

        _client.MessageDeleted -= ClientOnMessageDeleted;
        _client.MessageDeleted += ClientOnMessageDeleted;
    }

    private async Task ClientOnMessageDeleted(Cacheable<IMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
    {
        var message = await arg1.GetOrDownloadAsync();
        var channel = await arg2.GetOrDownloadAsync();

        if (message == null && channel == null)
        {
            return;
        }

        if (message != null)
        {
            if (message.Author.IsBot)
            {
                return;
            }

            var deletedMessagesByBot = _cacheManager.GetDeletedMessagesByBot();
            if (deletedMessagesByBot.Contains(message.Id))
            {
                return;
            }
        }

        if (message != null && channel == null)
        {
            await LogIfAvailable(EEventName.MessageDeleted,
                $"{message.Content} was deleted in #{message.Channel.Name}");
            return;
        }

        if (message == null && channel != null)
        {
            await LogIfAvailable(EEventName.MessageDeleted,
                $"A message has been deleted in {channel.Name} but I couldn't retrieve it.");
            return;
        }

        await LogIfAvailable(EEventName.MessageDeleted,
            $"{message!.Content} was deleted in #{message.Channel.Name}");
    }

    private async Task ClientOnMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
        var originalMessage = await arg1.GetOrDownloadAsync();

        if (arg2.Author.IsBot)
        {
            return;
        }

        if (originalMessage != null)
        {
            await LogIfAvailable(EEventName.MessageUpdated,
                $"A Message has been updated in #{arg3.Name}{Environment.NewLine} New Message: {arg2.Content}{Environment.NewLine} Old Message: {originalMessage.Content}");
            return;
        }

        await LogIfAvailable(EEventName.MessageUpdated,
            $"A Message has been updated in #{arg3.Name}{Environment.NewLine} New Message: {arg2.Content}{Environment.NewLine} **Couldn't retrieve old message**");
    }

    private async Task ClientOnUserUpdated(SocketUser arg1, SocketUser arg2)
    {
        await LogIfAvailable(EEventName.UserUpdated, $"{arg2.Username} has been updated");
    }

    private async Task ClientOnGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser arg2)
    {
        await LogIfAvailable(EEventName.GuildMemberUpdated, $"{arg2.Username} has been updated");
    }

    private async Task ClientOnRoleUpdated(SocketRole arg1, SocketRole arg2)
    {
        await LogIfAvailable(EEventName.RoleUpdated, $"{arg2.Name} role has been updated");
    }

    private async Task ClientOnRoleDeleted(SocketRole arg)
    {
        await LogIfAvailable(EEventName.RoleDeleted, $"{arg.Name} role has been deleted");
    }

    private async Task ClientOnRoleCreated(SocketRole arg)
    {
        await LogIfAvailable(EEventName.RoleCreated, $"{arg.Name} role has been created");
    }

    private async Task ClientOnUserBanned(SocketUser arg1, SocketGuild arg2)
    {
        await LogIfAvailable(EEventName.UserBanned, $"{arg1.Username} has been banned");
    }

    private async Task ClientOnUserLeft(SocketGuild arg1, SocketUser arg2)
    {
        await LogIfAvailable(EEventName.UserLeft, $"{arg2.Username} left the server");
    }

    private async Task ClientOnUserUnbanned(SocketUser arg1, SocketGuild arg2)
    {
        await LogIfAvailable(EEventName.UserUnbanned, $"{arg1.Username} has been unbanned");
    }

    private async Task ClientOnUserJoined(SocketGuildUser arg)
    {
        await LogIfAvailable(EEventName.UserJoined, $"{arg.Username} joined the server");
    }

    private async Task LogIfAvailable(EEventName eventName, string message)
    {
        var eventsConfiguration = _cacheManager.GetEventConfiguration(eventName);

        if (eventsConfiguration == null || !eventsConfiguration.IsEnabled)
        {
            return;
        }

        await Logger.Log(message);
    }

    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else
            Console.WriteLine($"[General/{message.Severity}] {message}");

        return Task.CompletedTask;
    }

    private Task MessageReceived(SocketMessage arg)
    {
        if (arg.Author.Username == _client.CurrentUser.Username)
            return Task.CompletedTask;

        SocketGuildChannel? guildChannel = arg.Channel as SocketGuildChannel;

        Console.WriteLine($"{arg.Timestamp:dd/MM/yyyy HH:mm:ss} - {guildChannel?.Guild.Name ?? "Private"} - {arg.Channel.Name} - {arg.Author.Username}: {arg}");

        return Task.CompletedTask;
    }

    public async Task AddLogConfiguration(SocketInteractionContext context, ITextChannel channel)
    {
        try
        {
            var userMessage = await channel.SendMessageAsync("Test");
            await userMessage.DeleteAsync();
        }
        catch
        {
            RaiseErrorOnLogConfiguration(new ConfigurationServiceEventArgs(context, "I can't send messages to this channel! FIX IT!"));
            return;
        }

        await _configurationService.AddLogConfiguration(context.Guild.Id, channel.Id);
        _cacheManager.AddOrUpdate(new LogConfiguration
        {
            ChannelId = channel.Id,
            GuildId = context.Guild.Id
        });
        RaiseLogAddedEvent(new ConfigurationServiceEventArgs(context, $"Logs will be sent to {channel.Mention}"));
    }

    public async Task RemoveLogConfiguration(SocketInteractionContext context)
    {
        await _configurationService.RemoveLogConfiguration();
        _cacheManager.Remove(new LogConfiguration());
        RaiseLogRemovedEvent(new ConfigurationServiceEventArgs(context, $"Logs will no longer be sent"));
    }

    private protected void RaiseLogRemovedEvent(ConfigurationServiceEventArgs args)
    {
        var handler = LogConfigurationRemoved;
        handler?.Invoke(this, args);
    }

    private protected void RaiseLogAddedEvent(ConfigurationServiceEventArgs args)
    {
        var handler = LogConfigurationAdded;
        handler?.Invoke(this, args);
    }

    private protected void RaiseErrorOnLogConfiguration(ConfigurationServiceEventArgs args)
    {
        var handler = LogConfigurationError;
        handler?.Invoke(this, args);
    }

    public event EventHandler<ConfigurationServiceEventArgs>? LogConfigurationAdded;
    public event EventHandler<ConfigurationServiceEventArgs>? LogConfigurationRemoved;
    public event EventHandler<ConfigurationServiceEventArgs>? LogConfigurationError;
}