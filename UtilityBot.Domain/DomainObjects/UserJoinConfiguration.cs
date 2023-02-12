using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class UserJoinConfiguration
{
    public ulong GuildId { get; set; }
    [ForeignKey(nameof(GuildId))]
    public JoinedServer? JoinedServer { get; set; }
    public string Action { get; set; } = null!;
}