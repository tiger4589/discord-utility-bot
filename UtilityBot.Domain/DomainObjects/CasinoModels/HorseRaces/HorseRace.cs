using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

public class HorseRace
{
    [Key]
    public int Id { get; set; }
    public int TrackId { get; set; }
    public DateTime RaceDate { get; set; }
}