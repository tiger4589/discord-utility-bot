namespace UtilityBot.Domain.DomainObjects;

public class MagicEightBallConfiguration
{
    public int Id { get; set; }
    public ulong ChannelId { get; set; }
    public bool IsEnabled { get; set; }
}