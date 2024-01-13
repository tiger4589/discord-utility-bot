using Discord;
using Discord.Interactions;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;

namespace UtilityBot.Services.HorseRaces;

public class HorseRaceModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IBotHorseRaceManager _horseRaceManager;

    public HorseRaceModule(IBotHorseRaceManager horseRaceManager)
    {
        _horseRaceManager = horseRaceManager;
    }

    [ComponentInteraction("predict_horse_*_*")]
    public async Task PredictHorse(string raceId, int horseId)
    {
        await _horseRaceManager.PredictWinner(Context, raceId, horseId);
    }
}