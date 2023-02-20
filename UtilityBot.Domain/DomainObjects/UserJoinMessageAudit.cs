using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class UserJoinMessageAudit : BaseAudit
{
    public ulong GuildId { get; set; }
    public string Message { get; set; } = null!;
    public bool IsPrivate { get; set; }
    public ulong? ChannelId { get; set; }
}