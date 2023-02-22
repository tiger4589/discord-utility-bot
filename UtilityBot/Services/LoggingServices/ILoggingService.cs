using Discord;
using Discord.Interactions;
using UtilityBot.EventArguments;

namespace UtilityBot.Services.LoggingServices;

public interface ILoggingService
{
    Task AddLogConfiguration(SocketInteractionContext context, ITextChannel channel);
    Task RemoveLogConfiguration(SocketInteractionContext context);
    Task InitializeService();
    public event EventHandler<ConfigurationServiceEventArgs> LogConfigurationAdded;
    public event EventHandler<ConfigurationServiceEventArgs> LogConfigurationRemoved;
    public event EventHandler<ConfigurationServiceEventArgs> LogConfigurationError;
}