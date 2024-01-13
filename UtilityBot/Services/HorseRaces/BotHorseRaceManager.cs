using System.ComponentModel;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Casino.HorseRaces;
using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;

namespace UtilityBot.Services.HorseRaces;

public interface IBotHorseRaceManager
{
    Task PredictWinner(SocketInteractionContext context, string raceId, int horseId);
}

public class BotHorseRaceManager : IBotHorseRaceManager
{
    private readonly DiscordSocketClient _client;
    private readonly IHorseRaceManager _horseRaceManager;
    private readonly object _lock = new();
    private readonly List<Race> _runningRaces = new();

    public BotHorseRaceManager(DiscordSocketClient client, IHorseRaceManager horseRaceManager)
    {
        _client = client;
        _horseRaceManager = horseRaceManager;

        _client.Ready -= ClientOnReady;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        await _horseRaceManager.LoadConfiguration();

        _horseRaceManager.RaceStarting += async (sender, args) =>
        {
            await HorseRaceManagerOnRaceStarting(sender, args);
        };
    }

    private async Task HorseRaceManagerOnRaceStarting(object? sender, RaceEventArgs e)
    {
        try
        {
            //todo: move this to a config file
            var channel = (ITextChannel)await _client.GetChannelAsync(1195691533609992232);
            var message = await channel.SendMessageAsync(embed: new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder
                    {
                        Name = "Race Manager"
                    })
                    .WithDescription($"A new race will start in 2 minutes in {e.Race.Track.Name}. {Environment.NewLine} You can now predict the winner by using the button with the desired name!")
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp()
                    .WithFields(new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                            .WithName("Track")
                            .WithValue($"Type: {e.Race.Track.Type}. {Environment.NewLine}Length: {e.Race.Track.Length}"),
                        new EmbedFieldBuilder()
                            .WithName("Horses")
                            .WithValue(string.Join(Environment.NewLine, e.Race.Horses.Select(x=>$"{x.Name} - Odds: 1-{x.OddsToOne} - Advantage: {GetAdvantageEmoji(x, e.Race.Track.Type)}")))
                    })
                    .Build(),
                components: BuildComponentsForRace(e.Race));

            e.Race.UserMessage = message;

            lock (_lock)
            {
                _runningRaces.Add(e.Race);
            }
        }
        catch (Exception exception)
        {
            await Logger.Log($"Couldn't start a race! Error message: {exception.Message}");
        }
        
    }

    private string GetAdvantageEmoji(Horse horse, string trackType)
    {
        if (horse.AdvantageOn == trackType)
        {
            return "⬆️";
        }

        if (horse.DisadvantageOn == trackType)
        {
            return "⬇️";
        }

        return "➖";
    }

    private MessageComponent BuildComponentsForRace(Race race)
    {
        var componentBuilder = new ComponentBuilder();

        foreach (var horse in race.Horses)
        {
            componentBuilder.WithButton(horse.Name, $"predict_horse_{race.Id}_{horse.Id}", ButtonStyle.Secondary);
        }

        return componentBuilder.Build();
    }

    public async Task PredictWinner(SocketInteractionContext context, string raceId, int horseId)
    {
        Guid id = Guid.Parse(raceId);
        Race? race;
        lock (_lock)
        {
            race = _runningRaces.FirstOrDefault(x => x.Id == id);
        }

        if (race == null)
        {
            await context.Interaction.RespondAsync(ephemeral: true, embed: new EmbedBuilder()
                .WithColor(Colors.Red)
                .WithDescription("Race not found!")
                .Build());
            return;
        }

        var horse = race.Horses.Single(x => x.Id == horseId);

        await context.Interaction.RespondAsync(ephemeral: true, embed: new EmbedBuilder()
            .WithColor(Colors.Green)
            .WithDescription($"You have predicted {horse.Name} to be the winner! Good luck!")
            .Build());

        var predicted = race.AddPrediction(new()
        {
            HorseId = horse.Id,
            UserId = context.User.Id
        });

        if (predicted)
        {
            await context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = new EmbedBuilder()
                    .WithColor(Colors.Green)
                    .WithDescription($"Prediction for {horse.Name} Has been registered")
                    .Build();
            });
        }
        else
        {
            await context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = new EmbedBuilder()
                    .WithColor(Colors.Red)
                    .WithDescription($"You already have another prediction, sorry!")
                    .Build();
            });
        }
    }
}
