using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class ConfiguredAction
{
    public ulong GuildId { get; set; }
    [ForeignKey(nameof(GuildId))]
    public JoinedServer? JoinedServer { get; set; }
    public string ConfigurationType { get; set; } = null!;
}