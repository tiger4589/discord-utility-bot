using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class LogConfigurationAudit : BaseAudit
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}