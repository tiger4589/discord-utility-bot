using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

namespace UtilityBot.Domain.Services.HorseRaceServices.Models;

public class HorsesAndWins
{
    public Horse Horse { get; set; } = null!;
    public int RacesParticipatedAt { get; set; }
    public int RacesWon { get; set; }
    public double WinPercentage => (1.0*RacesWon) / RacesParticipatedAt;
}