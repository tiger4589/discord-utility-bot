using Discord;
using Discord.Interactions;

namespace UtilityBot.Modules;

[Group("configuration", "Guild Configuration Module")]
[RequireUserPermission(GuildPermission.ManageGuild, Group = "Configuration")]
public class ConfigurationModule : InteractionModuleBase<SocketInteractionContext>
{
    [Group("on-join-role", "Add A Specific Role For User On Join")]
    public class UserJoinRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("add", "Specify Role")]
        public async Task AddRole(IRole role)
        {
            await RespondAsync($"Will add this role to the user: {role.Name} - {role.Id}");
        }
    }

    [Group("on-join-message", "Add A Specific Message For User On Join")]
    public class UserJoinMessageModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("add", "Specify Role")]
        public async Task AddMessage(string message, bool isPrivate, IChannel? channel = null)
        {
            await RespondAsync($"Will send this message to the user: {message} - {isPrivate} - {channel?.Name} - {channel?.Id}");
        }
    }
}