using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

public class RaceStanding
{
    public int RaceId { get; set; }
    public int HorseId { get; set; }
    public int Position { get; set; }
}