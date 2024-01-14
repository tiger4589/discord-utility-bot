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

    [SlashCommand("top-predictions-by-number", "Get top predictors based on the correct predictions only")]
    public async Task TopPredictionsByNumber()
    {
        await _horseRaceManager.GetTopPredictionsByNumber(Context);
    }

    [SlashCommand("top-predictions-by-percentage", "Get top predictors based on the percentage of correct predictions")]
    public async Task TopPredictionsByPercentage()
    {
        await _horseRaceManager.GetTopPredictionsByPercentage(Context);
    }

    [SlashCommand("top-horses-by-number", "Get top horses based on won game by numbers only")]
    public async Task TopHorsesByNumber()
    {
        await _horseRaceManager.GetTopHorsesByNumber(Context);
    }

    [SlashCommand("top-horses-by-percentage", "Get top horses based percentage of won races")]
    public async Task TopHorsesByPercentage()
    {
        await _horseRaceManager.GetTopHorsesByPercentage(Context);
    }
}