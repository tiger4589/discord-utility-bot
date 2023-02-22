using Discord;
using Discord.Interactions;
using UtilityBot.EventArguments;

namespace UtilityBot.Services.JokesServices;

public interface IJokeService
{
    Task InitializeDadJokes(SocketInteractionContext context, string channelName);
    Task InitializeDadJokes(SocketInteractionContext context, ITextChannel channel);
    Task EnableDadJokes(SocketInteractionContext context);
    Task DisableDadJokes(SocketInteractionContext context);

    Task GetRandomDadJoke(SocketInteractionContext context);

    public event EventHandler<ConfigurationServiceEventArgs> DadJokesInitialized;
    public event EventHandler<ConfigurationServiceEventArgs> DadJokesEnabled;
    public event EventHandler<ConfigurationServiceEventArgs> DadJokesDisabled;
}