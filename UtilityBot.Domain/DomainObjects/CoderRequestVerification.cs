using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class CoderRequestVerification
{
    [Key]
    public int Id { get; set; }
    public ulong RoleId { get; set; }
    public ulong ChannelId { get; set; }
}