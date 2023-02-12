using Discord.Interactions;
using UtilityBot.EventArguments;

namespace UtilityBot.Services.ConfigurationServices;

public interface IConfigurationService
{
    Task AddRoleToGuildOnJoin(SocketInteractionContext interactionContext, ulong guildId, ulong roleId);
    Task AddMessageToGuildOnJoin(SocketInteractionContext context, ulong guildId, string message, bool isPrivate, ulong? channelId);

    public event EventHandler<ConfigurationServiceEventArgs> RoleConfigured;
    public event EventHandler<ConfigurationServiceEventArgs> MessageConfigured;
    public event EventHandler<ConfigurationServiceEventArgs> ErrorOnPublicMessage;
}