using Discord;
using Discord.Interactions;
using UtilityBot.Services.CacheService;

namespace UtilityBot.Services.PlayerServices;

public class PlayerService : IPlayerService
{
    private readonly ICacheManager _cacheManager;

    public PlayerService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task RequestVerification(SocketInteractionContext context, string recruitLink)
    {
        var conf = _cacheManager.GetVerifyConfiguration();
        if (conf == null)
        {
            await context.Interaction.RespondAsync("Verify Configuration is not currently set... Notify Moderators!", ephemeral:true);
            return;
        }

        var channel = context.Guild.GetChannel(conf.ChannelId) as ITextChannel;
        if (channel == null)
        {
            //raise an error in an owned channel or something
            return;
        }

        await channel.SendMessageAsync(
            $"Will Change To An Embed! Verification Request! @<{context.User.Id}> {context.User.Username}: {recruitLink}",
            components: new ComponentBuilder()
                .WithButton("Verify", "some-id", ButtonStyle.Success, new Emoji("\u2705"))
                .WithButton("reject", "another-id", ButtonStyle.Danger, new Emoji("⛔"))
                .Build());

        await context.Interaction.RespondAsync("Moderators have been notified, please be patient.");
    }
}