using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class LogConfiguration
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong GuildId { get; set; }
    [ForeignKey(nameof(GuildId))]
    public JoinedServer? JoinedServer { get; set; }

    public ulong ChannelId { get; set; }
}