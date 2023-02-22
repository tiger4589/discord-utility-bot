using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class JokeConfiguration
{
    [Key]
    public int Id { get; set; }

    public EJokeType JokeType { get; set; }
    public bool IsEnabled { get; set; }
    public ulong ChannelId { get; set; }
}

public enum EJokeType
{
    DadJoke
}