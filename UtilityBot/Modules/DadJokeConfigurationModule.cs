using Discord;
using Discord.Interactions;
using UtilityBot.EventArguments;
using UtilityBot.Services.JokesServices;

namespace UtilityBot.Modules;

[Group("dad-jokes", "Dad Jokes Configuration Module")]
[RequireUserPermission(GuildPermission.ManageGuild, Group = "dad-jokes")]
public class DadJokeConfigurationModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IJokeService _jokeService;

    public DadJokeConfigurationModule(IJokeService jokeService)
    {
        _jokeService = jokeService;
    }

    private void AddHandlers()
    {
        _jokeService.DadJokesEnabled += JokeServiceOnDadJokesEnabled;
        _jokeService.DadJokesDisabled += JokeServiceOnDadJokesDisabled;
        _jokeService.DadJokesInitialized += JokeServiceOnDadJokesInitialized;
    }

    private void RemoveHandlers()
    {
        _jokeService.DadJokesEnabled -= JokeServiceOnDadJokesEnabled;
        _jokeService.DadJokesDisabled -= JokeServiceOnDadJokesDisabled;
        _jokeService.DadJokesInitialized -= JokeServiceOnDadJokesInitialized;
    }

    private async void JokeServiceOnDadJokesInitialized(object? sender, ConfigurationServiceEventArgs e)
    {
        await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = e.Message);
    }

    private async void JokeServiceOnDadJokesDisabled(object? sender, ConfigurationServiceEventArgs e)
    {
        await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = e.Message);
    }

    private async void JokeServiceOnDadJokesEnabled(object? sender, ConfigurationServiceEventArgs e)
    {
        await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = e.Message);
    }

    [SlashCommand("initialize-auto", "Initialize Dad Jokes and let bot create the channel")]
    public async Task InitializeDadJokes(string channelName)
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Initializing dad jokes");
        _ = _jokeService.InitializeDadJokes(Context, channelName);
    }

    [SlashCommand("initialize", "Initialize Dad Jokes for an existing channel")]
    public async Task InitializeDadJokes(ITextChannel channel)
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Initializing dad jokes");
        _ = _jokeService.InitializeDadJokes(Context, channel);
    }

    [SlashCommand("enable", "Enable dad jokes. Must be configured first")]
    public async Task EnableDadJokes()
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Enabling dad jokes");
        _ = _jokeService.EnableDadJokes(Context);
    }

    [SlashCommand("disable", "Disable dad jokes. Must be configured first")]
    public async Task DisableDadJokes()
    {
        RemoveHandlers();
        AddHandlers();

        await RespondAsync("Disabling dad jokes");
        _ = _jokeService.DisableDadJokes(Context);
    }
}