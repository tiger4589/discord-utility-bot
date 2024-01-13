using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

namespace UtilityBot.Domain.Services.HorseRaceServices.Models;

public class HorsesAndTracks
{
    public List<Horse> Horses { get; set; } = new ();
    public List<Track> Tracks { get; set; } = new ();
}