using Discord.Commands;
using Discord;
using Discord.WebSocket;

namespace UtilityBot.Services.LoggingServices;

public class LoggingService : ILoggingService
{
    private readonly DiscordSocketClient _client;

    public LoggingService(DiscordSocketClient client)
    {
        _client = client;
        _client.Ready += InitializeService;
    }

    public Task InitializeService()
    {
        _client.Log += LogAsync;
        _client.MessageReceived += MessageReceived;
        return Task.CompletedTask;
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
}