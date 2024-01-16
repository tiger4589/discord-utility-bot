using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class HangmanWordRequest
{
    [Key]
    public int Id { get; set; }
    public string Word { get; set; } = null!;
    public string? Definition { get; set; }
    public DateTime RequestedAt { get; set; }
}