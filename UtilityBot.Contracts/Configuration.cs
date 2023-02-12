namespace UtilityBot.Contracts;

public record Configuration(IList<UserJoinConfiguration> UserJoinConfigurations, IList<UserJoinMessage> UserJoinMessages, IList<UserJoinRole> UserJoinRoles);

public record UserJoinConfiguration(ulong GuildId, string Action);
public record UserJoinMessage(ulong GuildId, string Message, bool IsPrivate, ulong? ChannelId = null);
public record UserJoinRole(ulong GuildId, ulong RoleId);

public record UserJoinMessageConfiguration(UserJoinConfiguration UserJoinConfiguration, UserJoinMessage UserJoinMessage);
public record UserJoinRoleConfiguration(UserJoinConfiguration UserJoinConfiguration, UserJoinRole UserJoinRole);