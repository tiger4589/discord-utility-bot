using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.MessageHandlers;

namespace UtilityBot.Services.PlayerServices;

public class PlayerService : IPlayerService
{
    private readonly ICacheManager _cacheManager;
    private readonly IEmbedMessageBuilder _embedMessageBuilder;

    public PlayerService(ICacheManager cacheManager, IEmbedMessageBuilder embedMessageBuilder)
    {
        _cacheManager = cacheManager;
        _embedMessageBuilder = embedMessageBuilder;
    }

    public async Task RequestVerification(SocketInteractionContext context, string recruitLink)
    {
        var conf = _cacheManager.GetVerifyConfiguration();
        if (conf == null)
        {
            await context.Interaction.RespondAsync("Verify Configuration is not currently set... Notify Moderators!", ephemeral:true);
            return;
        }

        var socketGuildUser = context.User as SocketGuildUser;
        if (socketGuildUser!.Roles.Any(x => x.Id == conf.RoleId))
        {
            await context.Interaction.RespondAsync("You are already verified!", ephemeral: true);
            return;
        }

        if (recruitLink.Split(' ').Length > 1 || !recruitLink.StartsWith("https://www.kingsofchaos.com/recruit.php?uniqid="))
        {
            await context.Interaction.RespondAsync("Invalid Recruit Link! Please use yours from your command center, if you think this is wrong, message a moderator", ephemeral: true);
            return;
        }

        var uniqueHash = recruitLink.Replace("https://www.kingsofchaos.com/recruit.php?uniqid=", "");
        if (uniqueHash.Length != 8)
        {
            await context.Interaction.RespondAsync("Invalid Recruit Link! Please use yours from your command center, if you think this is wrong, message a moderator", ephemeral: true);
            return;
        }

        var channel = context.Guild.GetChannel(conf.ChannelId) as ITextChannel;
        if (channel == null)
        {
            await Logger.Log($"Can't find the channel to send verification request!");
            return;
        }

        var embed  = _embedMessageBuilder.BuildVerificationEmbed(context.User, recruitLink);

        await channel.SendMessageAsync(
            embed: embed,   
            components: new ComponentBuilder()
                .WithButton("Verify", $"verify_{context.User.Id}", ButtonStyle.Success, new Emoji("\u2705"))
                .WithButton("Reject", $"reject_{context.User.Id}", ButtonStyle.Danger, new Emoji("\u26D4"))
                .Build());

        await context.Interaction.RespondAsync("Moderators have been notified, please be patient.", ephemeral: true);
    }
}