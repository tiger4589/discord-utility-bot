using Discord;
using Discord.Interactions;
using UtilityBot.Services.JokesServices;
using UtilityBot.Services.RumbleServices;

namespace UtilityBot.Modules;

public class GeneralModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IJokeService _jokeService;
    private readonly IRumbleService _rumbleService;

    public GeneralModule(IJokeService jokeService, IRumbleService rumbleService)
    {
        _jokeService = jokeService;
        _rumbleService = rumbleService;
    }

    [SlashCommand("slap", "Slap a user mIRC style!")]
    public async Task RequestVerification(IUser user)
    {
        await RespondAsync("Sending a friendly slap for you!", ephemeral: true);
        await Context.Channel.SendMessageAsync(
            $"*{Context.User.Mention} slaps {user.Mention} around a bit with a large trout*");
    }

    [SlashCommand("dadjoke", "Get a random dad joke!")]
    public async Task GetRandomDadJoke()
    {
        await RespondAsync("Getting a random dad joke for ya!", ephemeral: true);
        _ = _jokeService.GetRandomDadJoke(Context);
    }

    [SlashCommand("chucknorris", "Get a random Chuck Norris Fact")]
    public async Task GetChuckNorrisFact()
    {
        await RespondAsync("Getting a random fact!", ephemeral: true);
        _ = _jokeService.GetRandomChuckNorrisJoke(Context);
    }

    [SlashCommand("subscribe-to-rumble", "Get notified when a royal rumble starts")]
    public async Task SubscribeToRumble()
    {
        await RespondAsync("Subscribing...", ephemeral: true);
        _ = _rumbleService.Subscribe(Context);
    }

    [SlashCommand("unsubscribe-from-rumble", "Stop getting notified when a royal rumble starts")]
    public async Task UnsubscribeToRumble()
    {
        await RespondAsync("Unsubscribing... :(", ephemeral: true);
        _ = _rumbleService.Unsubscribe(Context);
    }
}