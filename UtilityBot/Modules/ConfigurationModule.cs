using Discord;
using Discord.Interactions;
using System.Data;
using UtilityBot.EventArguments;
using UtilityBot.Services.ConfigurationServices;

namespace UtilityBot.Modules;

[Group("configuration", "Guild Configuration Module")]
[RequireUserPermission(GuildPermission.ManageGuild, Group = "Configuration")]
public class ConfigurationModule : InteractionModuleBase<SocketInteractionContext>
{
    [Group("on-join-role", "Add A Specific Role For User On Join")]
    public class UserJoinRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IConfigurationService _configurationService;

        public UserJoinRoleModule(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [SlashCommand("add", "Specify Role")]
        public async Task AddRole(IRole role)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Configuring on user join to add role: {role.Name}");
            _ = _configurationService.AddRoleToGuildOnJoin(Context, Context.Guild.Id, role.Id);
        }

        private void AddHandlers()
        {
            _configurationService.RoleConfigured += ConfigurationServiceOnRoleConfigured;
            _configurationService.ErrorOnRole += OnRoleError;
        }

        private void RemoveHandlers()
        {
            _configurationService.RoleConfigured -= ConfigurationServiceOnRoleConfigured;
            _configurationService.ErrorOnRole -= OnRoleError;
        }

        private async void OnRoleError(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "I can't give a higher role than mine! Fix that before.");
        }

        private async void ConfigurationServiceOnRoleConfigured(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Finished Configuring");
        }
    }

    [Group("on-join-message", "Add A Specific Message For User On Join")]
    public class UserJoinMessageModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IConfigurationService _configurationService;

        public UserJoinMessageModule(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [SlashCommand("add", "Specify Message and Parameters")]
        public async Task AddMessage(string message, bool isPrivate, ITextChannel? channel = null)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Configuring on user join to send message");
            _ = _configurationService.AddMessageToGuildOnJoin(Context, Context.Guild.Id, message, isPrivate,
                channel?.Id);
        }

        private void AddHandlers()
        {
            _configurationService.ErrorOnPublicMessage += ErrorPublicMessage;
            _configurationService.MessageConfigured += ConfigurationServiceOnMessageConfigured;
        }

        private void RemoveHandlers()
        {
            _configurationService.ErrorOnPublicMessage -= ErrorPublicMessage;
            _configurationService.MessageConfigured -= ConfigurationServiceOnMessageConfigured;
        }

        private async void ConfigurationServiceOnMessageConfigured(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Finished Configuring!");
        }

        private async void ErrorPublicMessage(object? sender, ConfigurationServiceEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Message))
            {
                await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                    prop.Content = e.Message);
                return;
            }

            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Couldn't add the configuration! You want it in public but didn't specify a channel!");
        }
    }
}