using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class RumbleConfiguration
{
    [Key]
    public int Id { get; set; }
    public DateTime CreationTime { get; set; }
    public ulong ChannelId { get; set; }
    public ulong RoleId { get; set; }
    public ulong BotRoleId { get; set; }
    public string EmojiToWatch { get; set; } = null!;
    public bool JoinGame { get; set; }
}