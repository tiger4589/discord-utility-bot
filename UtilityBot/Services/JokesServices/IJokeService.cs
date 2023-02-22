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

    Task InitializeChuckNorrisJokes(SocketInteractionContext context, string channelName);
    Task InitializeChuckNorrisJokes(SocketInteractionContext context, ITextChannel channel);
    Task EnableChuckNorrisJokes(SocketInteractionContext context);
    Task DisableChuckNorrisJokes(SocketInteractionContext context);
    Task GetRandomChuckNorrisJoke(SocketInteractionContext context);

    public event EventHandler<ConfigurationServiceEventArgs> DadJokesInitialized;
    public event EventHandler<ConfigurationServiceEventArgs> DadJokesEnabled;
    public event EventHandler<ConfigurationServiceEventArgs> DadJokesDisabled;

    public event EventHandler<ConfigurationServiceEventArgs> ChuckNorrisJokesInitialized;
    public event EventHandler<ConfigurationServiceEventArgs> ChuckNorrisJokesEnabled;
    public event EventHandler<ConfigurationServiceEventArgs> ChuckNorrisJokesDisabled;
}