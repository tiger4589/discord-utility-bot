using System.Linq.Expressions;
using UtilityBot.Contracts;
using UtilityBot.Domain.DomainObjects;
using UserJoinConfiguration = UtilityBot.Domain.DomainObjects.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Domain.DomainObjects.UserJoinMessage;
using UserJoinRole = UtilityBot.Domain.DomainObjects.UserJoinRole;

namespace UtilityBot.Domain.Tests.Unit.Fakes;

public class Constants
{
    public static JoinedServer ToKeepServer => new JoinedServer
    {
        GuildId = 123456789,
        IsActivated = true,
        IsConnected = true,
        Name = "t45_server"
    };

    public static JoinedServer DisconnectedServer => new JoinedServer
    {
        GuildId = 123456789,
        IsActivated = true,
        IsConnected = false,
        Name = "t45_server"
    };

    public static JoinedServer ToRemoveServer => new JoinedServer
    {
        GuildId = 987654321,
        IsActivated = true,
        IsConnected = true,
        Name = "Other_Server"
    };

    public static UserJoinConfiguration UserJoinConfigurationRole => new UserJoinConfiguration
    {
        GuildId = ToKeepServer.GuildId,
        Action = ActionTypeNames.AddRole
    };

    public static UserJoinRole UserJoinRole => new UserJoinRole
    {
        GuildId = ToKeepServer.GuildId,
        RoleId = 53114287138
    };

    public static UserJoinRole NewUserJoinRole => new UserJoinRole
    {
        GuildId = ToKeepServer.GuildId,
        RoleId = 3523384314
    };

    public static UserJoinMessage ExistingUserMessage => new UserJoinMessage
    {
        ChannelId = null,
        GuildId = ToKeepServer.GuildId,
        IsPrivate = true,
        Message = "Welcome!"
    };

    public static UserJoinConfiguration UserJoinConfigurationMessage => new UserJoinConfiguration
    {
        GuildId = ToKeepServer.GuildId,
        Action = ActionTypeNames.SendMessage
    };
}