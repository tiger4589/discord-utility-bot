using System.Text;
using Discord;
using Discord.Interactions;
using UtilityBot.Services.CacheService;

namespace UtilityBot.Modules;

[DefaultMemberPermissions(GuildPermission.ManageRoles)]
public class ModeratorModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICacheManager _cacheManager;

    public ModeratorModule(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    [SlashCommand("send-verify-message", "Send the verify message to unverified players")]
    public async Task SendVerifyMessage()
    {
        var verifyMessageConfiguration = _cacheManager.GetVerifyMessageConfiguration();
        if (verifyMessageConfiguration == null)
        {
            await RespondAsync("Configuration isn't set.", ephemeral: true);
            return;
        }

        var role = Context.Guild.GetRole(verifyMessageConfiguration.RoleId);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(role.Mention);
        sb.AppendLine(verifyMessageConfiguration.Message);

        await Context.Channel.SendMessageAsync(sb.ToString());

        await RespondAsync("Done!", ephemeral: true);
    }
}