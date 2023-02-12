using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class UserJoinRole
{
    public ulong GuildId { get; set; }
    [ForeignKey(nameof(GuildId))]
    public JoinedServer? JoinedServer { get; set; }
    public ulong RoleId { get; set; }
}