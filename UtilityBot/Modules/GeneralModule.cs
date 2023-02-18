using Discord;
using Discord.Interactions;

namespace UtilityBot.Modules;

public class GeneralModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("slap", "Slap a user mIRC style!")]
    public async Task RequestVerification(IUser user)
    {
        await RespondAsync("Sending a friendly slap for you!", ephemeral: true);
        await Context.Channel.SendMessageAsync(
            $"*{Context.User.Mention} slaps {user.Mention} around a bit with a large trout*");
    }
}