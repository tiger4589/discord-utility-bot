using Discord.Commands;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.EventArguments;
using UtilityBot.Services.CacheService;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace UtilityBot.Services.LoggingServices;

public class LoggingService : ILoggingService
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IConfigurationService _configurationService;
    private readonly IConfiguration _configuration;

    public LoggingService(DiscordSocketClient client, IServiceProvider serviceProvider)
    {
        _client = client;
        _cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        _configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _client.Ready += InitializeService;
    }

    public async Task InitializeService()
    {
        var logConfiguration = await _configurationService.GetLogConfiguration(ulong.Parse(_configuration["ServerId"]!));
        if (logConfiguration != null)
        {
            _cacheManager.AddOrUpdate(logConfiguration);
        }
        _client.Log += LogAsync;
        _client.MessageReceived += MessageReceived;
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
        if (arg.Author.Username == _client?.CurrentUser.Username)
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