using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects;

public class JoinedServer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong GuildId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActivated { get; set; }
    public bool IsConnected { get; set; }
}