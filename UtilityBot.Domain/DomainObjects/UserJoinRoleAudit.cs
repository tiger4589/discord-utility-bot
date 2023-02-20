using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class UserJoinRoleAudit : BaseAudit
{
    public ulong GuildId { get; set; }
    public ulong RoleId { get; set; }
}