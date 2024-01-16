using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class HangmanGames
{
    [Key]
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public string Word { get; set; } = null!;
    public int Score { get; set; }
    public bool IsCorrect { get; set; }
}