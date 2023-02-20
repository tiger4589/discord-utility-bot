using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class UserJoinConfigurationAudit : BaseAudit
{
    public ulong GuildId { get; set; }
    public string Action { get; set; } = null!;
}