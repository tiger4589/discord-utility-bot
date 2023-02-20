using Discord.Interactions;
using UtilityBot.EventArguments;

namespace UtilityBot.Services.ConfigurationServices;

public interface IConfigurationService
{
    Task AddRoleToGuildOnJoin(SocketInteractionContext interactionContext, ulong guildId, ulong roleId);
    Task AddMessageToGuildOnJoin(SocketInteractionContext context, ulong guildId, string message, bool isPrivate, ulong? channelId);
    Task AddVerifyConfiguration(SocketInteractionContext context, ulong channelId, ulong roleId, string? message);
    Task RemoveWelcomeMessage(SocketInteractionContext context);
    Task RemoveOnJoinRole(SocketInteractionContext context, ulong roleId);

    public event EventHandler<ConfigurationServiceEventArgs> RoleConfigured;
    public event EventHandler<ConfigurationServiceEventArgs> RoleRemoved;
    public event EventHandler<ConfigurationServiceEventArgs> ErrorOnRole;
    public event EventHandler<ConfigurationServiceEventArgs> MessageConfigured;
    public event EventHandler<ConfigurationServiceEventArgs> MessageRemoved;
    public event EventHandler<ConfigurationServiceEventArgs> ErrorOnPublicMessage;
    public event EventHandler<ConfigurationServiceEventArgs> VerifyConfigurationSet;
}