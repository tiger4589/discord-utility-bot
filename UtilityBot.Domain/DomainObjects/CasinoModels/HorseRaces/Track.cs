using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

public class Track
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Length { get; set; }
    public int TimeBetweenRacesInMinutes { get; set; }
    public string Type { get; set; } = null!;
}