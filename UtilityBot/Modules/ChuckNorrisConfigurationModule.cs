using Discord;
using Discord.Interactions;
using UtilityBot.Services.JokesServices;
using UtilityBot.EventArguments;

namespace UtilityBot.Modules;

[Group("chuck-norris", "Chuck Norris Facts Configuration Module")]
[DefaultMemberPermissions(GuildPermission.ManageGuild)]
public class ChuckNorrisConfigurationModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IJokeService _jokeService;

    public ChuckNorrisConfigurationModule(IJokeService jokeService)
    {
        _jokeService = jokeService;
    }

    private void AddHandlers()
    {
        _jokeService.ChuckNorrisJokesEnabled += JokeServiceOnChuckNorrisJokesEnabled;
        _jokeService.ChuckNorrisJokesDisabled += JokeServiceOnChuckNorrisJokesDisabled;
        _jokeService.ChuckNorrisJokesInitialized += JokeServiceOnChuckNorrisJokesInitialized;
    }

    private void RemoveHandlers()
    {
        _jokeService.ChuckNorrisJokesEnabled -= JokeServiceOnChuckNorrisJokesEnabled;
        _jokeService.ChuckNorrisJokesDisabled -= JokeServiceOnChuckNorrisJokesDisabled;
        _jokeService.ChuckNorrisJokesInitialized -= JokeServiceOnChuckNorrisJokesInitialized;
    }

    private async void JokeServiceOnChuckNorrisJokesInitialized(object? sender, ConfigurationServiceEventArgs e)
    {
        await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = e.Message);
    }

    private async void JokeServiceOnChuckNorrisJokesDisabled(object? sender, ConfigurationServiceEventArgs e)
    {
        await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = e.Message);
    }

    private async void JokeServiceOnChuckNorrisJokesEnabled(object? sender, ConfigurationServiceEventArgs e)
    {
        await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = e.Message);
    }

    [SlashCommand("initialize-auto", "Initialize Chuck Norris Facts and let bot create the channel")]
    public async Task InitializeChuckNorrisJokes(string channelName)
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Initializing chuck norris facts");
        _ = _jokeService.InitializeChuckNorrisJokes(Context, channelName);
    }

    [SlashCommand("initialize", "Initialize Chuck Norris Facts for an existing channel")]
    public async Task InitializeChuckNorrisJokes(ITextChannel channel)
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Initializing Chuck Norris facts");
        _ = _jokeService.InitializeChuckNorrisJokes(Context, channel);
    }

    [SlashCommand("enable", "Enable Chuck Norris facts. Must be configured first")]
    public async Task EnableChuckNorrisJokes()
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Enabling Chuck Norris facts");
        _ = _jokeService.EnableChuckNorrisJokes(Context);
    }

    [SlashCommand("disable", "Disable Chuck Norris facts. Must be configured first")]
    public async Task DisableChuckNorrisJokes()
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Disabling Chuck Norris facts");
        _ = _jokeService.DisableChuckNorrisJokes(Context);
    }
}