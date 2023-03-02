using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class CapsProtectionConfiguration
{
    [Key]
    public int Id { get; set; }

    public int MinimumLength { get; set; }
    public int MinimumPercentage { get; set; }
}