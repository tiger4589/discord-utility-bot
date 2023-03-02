using Discord;
using Discord.Interactions;
using UtilityBot.Contracts;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.EventArguments;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.SpamProtectionServices;
using UserJoinConfiguration = UtilityBot.Contracts.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Contracts.UserJoinMessage;
using UserJoinRole = UtilityBot.Contracts.UserJoinRole;
using VerifyConfiguration = UtilityBot.Contracts.VerifyConfiguration;

namespace UtilityBot.Services.ConfigurationServices;

public class ConfigurationService : IConfigurationService
{
    private readonly ICacheManager _cacheManager;
    private readonly Domain.Services.ConfigurationService.Interfaces.IConfigurationService _configurationService;
    private readonly ISpamProtectionService _spamProtectionService;

    public ConfigurationService(ICacheManager cacheManager, Domain.Services.ConfigurationService.Interfaces.IConfigurationService configurationService, ISpamProtectionService spamProtectionService)
    {
        _cacheManager = cacheManager;
        _configurationService = configurationService;
        _spamProtectionService = spamProtectionService;
    }

    public async Task AddRoleToGuildOnJoin(SocketInteractionContext interactionContext, ulong guildId, ulong roleId)
    {
        var configuration = new UserJoinRoleConfiguration(new UserJoinConfiguration(guildId, ActionTypeNames.AddRole),
            new UserJoinRole(guildId, roleId));

        var myRoles = interactionContext.Guild.CurrentUser.Roles;
        var socketRole = interactionContext.Guild.GetRole(roleId);
        
        if (myRoles.All(x => x.Position <= socketRole.Position))
        {
            RaiseErrorOnRole(new ConfigurationServiceEventArgs(interactionContext));
            return;
        }

        await _configurationService.AddUserJoinRoleConfiguration(configuration.UserJoinConfiguration, configuration.UserJoinRole);
        _cacheManager.AddOrUpdate(configuration);
        RaiseRoleConfiguredEvent(new ConfigurationServiceEventArgs(interactionContext, $"Will be giving new user who join the following role: {socketRole.Name}"));
    }

    public async Task AddMessageToGuildOnJoin(SocketInteractionContext context, ulong guildId, string message, bool isPrivate,
        ulong? channelId)
    {
        if (!isPrivate && channelId == null)
        {
            RaiseErrorOnPublicMessage(new ConfigurationServiceEventArgs(context));
            return;
        }

        ITextChannel? channel = null;
        if (channelId != null && !isPrivate)
        {
            //todo: find a better way to check this
            channel = context.Guild.GetChannel(channelId.Value) as ITextChannel;
            if (channel == null)
            {
                RaiseErrorOnPublicMessage(new ConfigurationServiceEventArgs(context, "I can't see this channel!"));
                return;
            }

            try
            {
                var userMessage = await channel.SendMessageAsync("Test");
                await userMessage.DeleteAsync();
            }
            catch 
            {
                RaiseErrorOnPublicMessage(new ConfigurationServiceEventArgs(context, "I probably can't send messages to this channel!"));
                return;
            }
        }

        var configuration = new UserJoinMessageConfiguration(
            new UserJoinConfiguration(guildId, ActionTypeNames.SendMessage),
            new UserJoinMessage(guildId, message, isPrivate, channelId));


        await _configurationService.AddUserJoinMessageConfiguration(configuration.UserJoinConfiguration,
            configuration.UserJoinMessage);
        _cacheManager.AddOrUpdate(configuration);
        RaiseMessageConfigured(new ConfigurationServiceEventArgs(context, $"Will send the following welcome message to users: {message}. IsPrivate: {isPrivate}, Channel: {(channel == null ? "No Channel" : channel.Name)}"));
    }

    public async Task AddVerifyConfiguration(SocketInteractionContext context, ulong channelId, ulong roleId, string? message)
    {
        var channel = context.Guild.GetChannel(channelId) as ITextChannel;
        if (channel == null)
        {
            RaiseErrorOnPublicMessage(new ConfigurationServiceEventArgs(context, "I can't see this channel!"));
            return;
        }

        try
        {
            var userMessage = await channel.SendMessageAsync("Test");
            await userMessage.DeleteAsync();
        }
        catch
        {
            RaiseErrorOnPublicMessage(new ConfigurationServiceEventArgs(context, "I probably can't send messages to this channel!"));
            return;
        }

        var myRoles = context.Guild.CurrentUser.Roles;
        var socketRole = context.Guild.GetRole(roleId);

        if (myRoles.All(x => x.Position <= socketRole.Position))
        {
            RaiseErrorOnRole(new ConfigurationServiceEventArgs(context));
            return;
        }

        var configuration = new VerifyConfiguration(channelId, roleId, message);
        await _configurationService.AddVerifyConfiguration(configuration);
        _cacheManager.AddOrUpdate(configuration);
        RaiseVerifyConfigurationEvent(new ConfigurationServiceEventArgs(context, $"Will send verification request to {channel.Name} and will give {socketRole.Name} upon acceptance."));
    }

    public async Task RemoveWelcomeMessage(SocketInteractionContext context)
    {
        await _configurationService.RemoveUserJoinMessageConfiguration(context.Guild.Id);
        _cacheManager.RemoveMessageConfiguration(context.Guild.Id);

        RaiseMessageRemoved(new ConfigurationServiceEventArgs(context));
    }

    public async Task RemoveOnJoinRole(SocketInteractionContext context, ulong roleId)
    {
        await _configurationService.RemoveUserJoinRoleConfiguration(context.Guild.Id, roleId);
        _cacheManager.RemoveRoleConfiguration(context.Guild.Id, roleId);

        RaiseRoleRemoved(new ConfigurationServiceEventArgs(context));
    }

    public async Task AddVerifyMessageConfiguration(SocketInteractionContext context, IRole role, string message)
    {
        await _configurationService.AddOrUpdateVerifyMessageConfiguration(context.Guild.Id, role.Id, message);
        _cacheManager.AddOrUpdate(new VerifyMessageConfiguration
        {
            GuildId = context.Guild.Id,
            RoleId = role.Id,
            Message = message
        });

        RaiseVerifyMessageConfigured(new ConfigurationServiceEventArgs(context, $"Role to mention: {role.Mention} {Environment.NewLine} Message: {message}"));
    }

    private protected void RaiseVerifyConfigurationEvent(ConfigurationServiceEventArgs args)
    {
        var handler = VerifyConfigurationSet;
        handler?.Invoke(this, args);
    }

    private protected void RaiseRoleConfiguredEvent(ConfigurationServiceEventArgs configurationServiceEventArgs)
    {
        var handler = RoleConfigured;
        handler?.Invoke(this, configurationServiceEventArgs);
    }

    private protected void RaiseMessageConfigured(ConfigurationServiceEventArgs configurationServiceEventArgs)
    {
        var handler = MessageConfigured;
        handler?.Invoke(this, configurationServiceEventArgs);
    }

    private protected void RaiseErrorOnPublicMessage(ConfigurationServiceEventArgs args)
    {
        var handler = ErrorOnPublicMessage;
        handler?.Invoke(this, args);
    }

    private protected void RaiseErrorOnRole(ConfigurationServiceEventArgs args)
    {
        var handler = ErrorOnRole;
        handler?.Invoke(this, args);
    }

    private protected void RaiseMessageRemoved(ConfigurationServiceEventArgs args)
    {
        var handler = MessageRemoved;
        handler?.Invoke(this, args);
    }

    private protected void RaiseRoleRemoved(ConfigurationServiceEventArgs args)
    {
        var handler = RoleRemoved;
        handler?.Invoke(this, args);
    }

    private protected void RaiseVerifyMessageConfigured(ConfigurationServiceEventArgs args)
    {
        var handler = VerifyMessageConfigured;
        handler?.Invoke(this, args);
    }

    public event EventHandler<ConfigurationServiceEventArgs>? RoleConfigured;
    public event EventHandler<ConfigurationServiceEventArgs>? RoleRemoved;
    public event EventHandler<ConfigurationServiceEventArgs>? MessageConfigured;
    public event EventHandler<ConfigurationServiceEventArgs>? ErrorOnPublicMessage;
    public event EventHandler<ConfigurationServiceEventArgs>? VerifyConfigurationSet;
    public async Task AddCapsProtection(SocketInteractionContext context, int minimumLength, int percentage)
    {
        var capsProtectionConfiguration = new CapsProtectionConfiguration
        {
            MinimumLength = minimumLength,
            MinimumPercentage = percentage
        };

        await _configurationService.AddConfiguration(capsProtectionConfiguration);
        _cacheManager.AddOrUpdate(capsProtectionConfiguration);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content = $"Will delete and warn users with a message of minimum {minimumLength} characters of which at least {percentage}% are caps!");

        await _spamProtectionService.ForceInitialize();
    }

    public event EventHandler<ConfigurationServiceEventArgs>? ErrorOnRole;
    public event EventHandler<ConfigurationServiceEventArgs>? MessageRemoved;
    public event EventHandler<ConfigurationServiceEventArgs>? VerifyMessageConfigured;
}