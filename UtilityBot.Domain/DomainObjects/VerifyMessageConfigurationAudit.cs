using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class VerifyMessageConfigurationAudit : BaseAudit
{
    public ulong GuildId { get; set; }
    public ulong RoleId { get; set; }
    public string Message { get; set; } = null!;
}