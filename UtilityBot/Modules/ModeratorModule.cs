using System.Text;
using Discord;
using Discord.Interactions;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.PlayerServices;

namespace UtilityBot.Modules;

[DefaultMemberPermissions(GuildPermission.ManageRoles)]
public class ModeratorModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICacheManager _cacheManager;
    private readonly IPlayerService _playerService;

    public ModeratorModule(ICacheManager cacheManager, IPlayerService playerService)
    {
        _cacheManager = cacheManager;
        _playerService = playerService;
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

    [SlashCommand("reset-role", "Reset a specific role for a user")]
    public async Task ResetRole(IUser user, IRole role)
    {
        await _playerService.ResetRole(Context, user.Id, role.Id);
    }
}