using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class RumbleMessageConfiguration
{
    [Key]
    public int Id { get; set; }
    public string Message { get; set; } = null!;
}