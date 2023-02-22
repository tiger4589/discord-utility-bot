using Discord;
using Discord.Interactions;
using UtilityBot.Services.JokesServices;

namespace UtilityBot.Modules;

public class GeneralModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IJokeService _jokeService;

    public GeneralModule(IJokeService jokeService)
    {
        _jokeService = jokeService;
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
}