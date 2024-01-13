using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

public class Horse
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int OddsToOne { get; set; }
    public string AdvantageOn { get; set; } = null!;
    public string DisadvantageOn { get; set; } = null!;
}