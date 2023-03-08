using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class EventsConfiguration
{
    [Key]
    public int Id { get; set; }
    public string EventName { get; set; } = null!;
    public bool IsEnabled { get; set; }
}

public class EventsName
{
    public const string UserJoined = "UserJoined";
    public const string UserUnbanned = "UserUnbanned";
    public const string UserLeft = "UserLeft";
    public const string UserBanned = "UserBanned";
    public const string RoleCreated = "RoleCreated";
    public const string RoleDeleted = "RoleDeleted";
    public const string RoleUpdated = "RoleUpdated";
    public const string GuildMemberUpdated = "GuildMemberUpdated";
    public const string UserUpdated = "UserUpdated";
    public const string MessageUpdated = "MessageUpdated";
    public const string MessageDeleted = "MessageDeleted";

    public static string[] EventsNames => new[]
    {
        UserJoined, UserUnbanned, UserLeft, UserBanned, RoleCreated, RoleDeleted, RoleUpdated, GuildMemberUpdated,
        UserUpdated, MessageUpdated, MessageDeleted
    };
}

public enum EEventName
{
    UserJoined,
    UserUnbanned,
    UserLeft,
    UserBanned,
    RoleCreated,
    RoleDeleted,
    RoleUpdated,
    GuildMemberUpdated,
    UserUpdated,
    MessageUpdated,
    MessageDeleted
}