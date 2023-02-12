using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using UtilityBot.Contracts;
using UtilityBot.EventArguments;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.CacheService;

namespace UtilityBot.Services.ConfigurationServices;

public class ConfigurationService : BaseApiCallService, IConfigurationService
{
    private readonly ICacheManager _cacheManager;

    public ConfigurationService(IConfiguration configuration, ICacheManager cacheManager) : base(configuration)
    {
        _cacheManager = cacheManager;
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

        ServiceUrl = "configuration/add-user-join-role-conf";

        await CallApi(configuration);
        _cacheManager.AddOrUpdate(configuration);
        RaiseRoleConfiguredEvent(new ConfigurationServiceEventArgs(interactionContext));
    }

    public async Task AddMessageToGuildOnJoin(SocketInteractionContext context, ulong guildId, string message, bool isPrivate,
        ulong? channelId)
    {
        if (!isPrivate && channelId == null)
        {
            RaiseErrorOnPublicMessage(new ConfigurationServiceEventArgs(context));
            return;
        }

        if (channelId != null && !isPrivate)
        {
            //todo: find a better way to check this
            var channel = context.Guild.GetChannel(channelId.Value) as ITextChannel;
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

        ServiceUrl = "configuration/add-user-join-message-conf";
        await CallApi(configuration);
        _cacheManager.AddOrUpdate(configuration);
        RaiseMessageConfigured(new ConfigurationServiceEventArgs(context));
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

    public event EventHandler<ConfigurationServiceEventArgs>? RoleConfigured;
    public event EventHandler<ConfigurationServiceEventArgs>? MessageConfigured;
    public event EventHandler<ConfigurationServiceEventArgs>? ErrorOnPublicMessage;
    public event EventHandler<ConfigurationServiceEventArgs>? ErrorOnRole;

    public override string? ServiceUrl { get; set; }
}