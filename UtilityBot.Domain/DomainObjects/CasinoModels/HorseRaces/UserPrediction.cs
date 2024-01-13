using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;

public class UserPrediction
{
    [Key]
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public int CorrectPredictions { get; set; }
    public int WrongPredictions { get; set; }
}