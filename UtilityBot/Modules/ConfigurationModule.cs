using Discord;
using Discord.Interactions;
using UtilityBot.EventArguments;
using UtilityBot.Services.ConfigurationServices;
using UtilityBot.Services.LoggingServices;

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

        [SlashCommand("add", "Assign the role you want the user to get once he joins the server")]
        public async Task AddRole([Summary(description:"Choose role you want the user to get once he joins the server")]IRole role)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Configuring on user join to add role: {role.Name}");
            _ = _configurationService.AddRoleToGuildOnJoin(Context, Context.Guild.Id, role.Id);
        }

        [SlashCommand("remove", "Remove a role from being given for a user after he joins the server")]
        public async Task RemoveRole(
            [Summary(description: "Choose the role you want to remove from being given to a user after he joins")]
            IRole role)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Removing role: {role.Name}");
            _ = _configurationService.RemoveOnJoinRole(Context, role.Id);
        }

        private void AddHandlers()
        {
            _configurationService.RoleConfigured += ConfigurationServiceOnRoleConfigured;
            _configurationService.ErrorOnRole += OnRoleError;
            _configurationService.RoleRemoved += ConfigurationServiceOnRoleRemoved;
        }

        private void RemoveHandlers()
        {
            _configurationService.RoleConfigured -= ConfigurationServiceOnRoleConfigured;
            _configurationService.ErrorOnRole -= OnRoleError;
            _configurationService.RoleRemoved -= ConfigurationServiceOnRoleRemoved;
        }

        private async void ConfigurationServiceOnRoleRemoved(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Removed the role!");
        }

        private async void OnRoleError(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "I can't give a higher role than mine! Fix that before.");
        }

        private async void ConfigurationServiceOnRoleConfigured(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = e.Message);
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

        [SlashCommand("add-from-existing", "Add a welcome message from an existing message in the channel!")]
        public async Task AddMessageFromExisting([Summary(description:"Right click on the message, and copy id")]string messageId, bool isPrivate, ITextChannel? channel = null)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Configuring on user join to send message");
            bool b = ulong.TryParse(messageId, out ulong realId);
            if (!b)
            {
                await ModifyOriginalResponseAsync(prop =>
                    prop.Content = "Apparently, I can't convert this id... weird");
                return;
            }

            var msg = await Context.Channel.GetMessageAsync(realId);
            _ = _configurationService.AddMessageToGuildOnJoin(Context, Context.Guild.Id, msg.Content, isPrivate,
                channel?.Id);
        }

        [SlashCommand("remove", "Remove the current configured welcome message from the server")]
        public async Task RemoveMessageConfiguration()
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync("Removing welcome message from this server");
            _ = _configurationService.RemoveWelcomeMessage(Context);
        }

        private void AddHandlers()
        {
            _configurationService.ErrorOnPublicMessage += ErrorPublicMessage;
            _configurationService.MessageConfigured += ConfigurationServiceOnMessageConfigured;
            _configurationService.MessageRemoved += ConfigurationServiceOnMessageRemoved;
        }

        private void RemoveHandlers()
        {
            _configurationService.ErrorOnPublicMessage -= ErrorPublicMessage;
            _configurationService.MessageConfigured -= ConfigurationServiceOnMessageConfigured;
            _configurationService.MessageRemoved -= ConfigurationServiceOnMessageRemoved;
        }

        private async void ConfigurationServiceOnMessageConfigured(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = e.Message);
        }

        private async void ConfigurationServiceOnMessageRemoved(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Welcome Message configuration has been removed");
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

    [Group("verify-configuration", "Add Verify Configuration")]
    public class VerifyConfigurationModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IConfigurationService _configurationService;

        public VerifyConfigurationModule(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [SlashCommand("add", "Add Verify Configuration")]
        public async Task AddVerifyConfiguration(ITextChannel channel, IRole role)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Configuring verify behavior!");
            _ = _configurationService.AddVerifyConfiguration(Context, channel.Id, role.Id);
        }

        private void AddHandlers()
        {
            _configurationService.ErrorOnRole += OnRoleError;
            _configurationService.ErrorOnPublicMessage += ErrorPublicMessage;
            _configurationService.VerifyConfigurationSet += OnVerifyConfigurationSet;
        }

        private void RemoveHandlers()
        {
            _configurationService.ErrorOnRole -= OnRoleError;
            _configurationService.ErrorOnPublicMessage -= ErrorPublicMessage;
            _configurationService.VerifyConfigurationSet -= OnVerifyConfigurationSet;
        }

        private protected async void OnVerifyConfigurationSet(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = e.Message);
        }

        private protected async void ErrorPublicMessage(object? sender, ConfigurationServiceEventArgs e)
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

        private protected async void OnRoleError(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "I can't give a higher role than mine! Fix that before.");
        }
    }

    [Group("log-configuration", "Bot log configuration")]
    public class LogConfigurationModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILoggingService _loggingService;

        public LogConfigurationModule(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [SlashCommand("add", "Add a channel so the bot can send us important logs there")]
        public async Task AddLogConfiguration([Summary(description:"The channel to send logs to")] ITextChannel channel)
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Adding Log Configuration.");
            _ = _loggingService.AddLogConfiguration(Context, channel);
        }

        [SlashCommand("remove", "Remove Current Log Configuration")]
        public async Task RemoveLogConfiguration()
        {
            RemoveHandlers();
            AddHandlers();
            await RespondAsync($"Removing Log Configuration.");
            _ = _loggingService.RemoveLogConfiguration(Context);
        }

        private void AddHandlers()
        {
            _loggingService.LogConfigurationRemoved += LoggingServiceOnLogConfigurationRemoved;
            _loggingService.LogConfigurationAdded += LoggingServiceOnLogConfigurationAdded;
            _loggingService.LogConfigurationError += LoggingServiceOnLogConfigurationError;
        }

        private async void LoggingServiceOnLogConfigurationError(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = e.Message);
        }

        private async void LoggingServiceOnLogConfigurationAdded(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = e.Message);
        }

        private async void LoggingServiceOnLogConfigurationRemoved(object? sender, ConfigurationServiceEventArgs e)
        {
            await e.InteractionContext.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = e.Message);
        }

        private void RemoveHandlers()
        {
            _loggingService.LogConfigurationRemoved -= LoggingServiceOnLogConfigurationRemoved;
            _loggingService.LogConfigurationAdded -= LoggingServiceOnLogConfigurationAdded;
            _loggingService.LogConfigurationError -= LoggingServiceOnLogConfigurationError;
        }
    }
}