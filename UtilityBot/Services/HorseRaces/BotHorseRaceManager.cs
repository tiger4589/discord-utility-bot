using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Casino.HorseRaces;
using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;
using UtilityBot.Domain.Services.HorseRaceServices;
using UtilityBot.Domain.Services.HorseRaceServices.Models;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;
using Timer = System.Timers.Timer;

namespace UtilityBot.Services.HorseRaces;

public interface IBotHorseRaceManager
{
    Task PredictWinner(SocketInteractionContext context, string raceId, int horseId);
    Task GetTopPredictionsByNumber(SocketInteractionContext context);
    Task GetTopPredictionsByPercentage(SocketInteractionContext context);
    Task GetTopHorsesByNumber(SocketInteractionContext context);
    Task GetTopHorsesByPercentage(SocketInteractionContext context);
}

public class BotHorseRaceManager : IBotHorseRaceManager
{
    private readonly DiscordSocketClient _client;
    private readonly IHorseRaceManager _horseRaceManager;
    private readonly IConfiguration _configuration;
    private readonly IHorseRaceService _horseRaceService;
    private readonly object _lock = new();
    private readonly List<Race> _runningRaces = new();

    public BotHorseRaceManager(DiscordSocketClient client, IHorseRaceManager horseRaceManager, IConfiguration configuration, IHorseRaceService horseRaceService)
    {
        _client = client;
        _horseRaceManager = horseRaceManager;
        _configuration = configuration;
        _horseRaceService = horseRaceService;

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

        //var timer = new Timer(60 * 60 * 1000);
        //timer.Elapsed += async (_, _) => { await ShowStandings(); };
        //timer.AutoReset = true;
        //timer.Enabled = true;
    }

    private async Task ShowStandings()
    {
        var channelId = ulong.Parse(_configuration["HorseRaceChannelId"]!);
        var channel = (ITextChannel)await _client.GetChannelAsync(channelId);
        var userPredictions = await GetUserPredictionsByNumber();
        if (userPredictions.Any())
        {
            var embed = await GetUserPredictionsByNumberEmbed(userPredictions);
            await channel.SendMessageAsync(embed: embed);
        }

        userPredictions = await GetUserPredictionsByPercentage();
        if (userPredictions.Any())
        {
            var embed = await GetUserPredictionsByPercentageEmbed(userPredictions);
            await channel.SendMessageAsync(embed: embed);
        }

        var horsesStandings = await GetTopHorsesByNumber();
        if (horsesStandings.Any())
        {
            var embed = GetHorseByNumberEmbed(horsesStandings);
            await channel.SendMessageAsync(embed: embed);
        }

        horsesStandings = await GetHorsesByPercentage();
        if (horsesStandings.Any())
        {
            var embed = GetHorsesPercentageEmbed(horsesStandings);
            await channel.SendMessageAsync(embed: embed);
        }
    }

    private async Task HorseRaceManagerOnRaceStarting(object? sender, RaceEventArgs e)
    {
        try
        {
            var channelId = ulong.Parse(_configuration["HorseRaceChannelId"]!);
            var channel = (ITextChannel)await _client.GetChannelAsync(channelId);
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

    public async Task GetTopPredictionsByNumber(SocketInteractionContext context)
    {
        var userPredictions = await GetUserPredictionsByNumber();

        if (!userPredictions.Any())
        {
            await context.Interaction.RespondAsync(embed: new EmbedBuilder()
                           .WithColor(Colors.Red)
                           .WithDescription("No predictions found!")
                           .Build());
            return;
        }
        
        var embed = await GetUserPredictionsByNumberEmbed(userPredictions);
        await context.Interaction.RespondAsync(embed: embed);
    }

    private async Task<Embed> GetUserPredictionsByNumberEmbed(List<UserPrediction> userPredictions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("```");

        for (int i = 0; i < userPredictions.Count; i++)
        {
            var user = await _client.GetUserAsync(userPredictions[i].UserId);
            sb.AppendLine($"#{i + 1} - {user?.Username ?? "UserNotFound"} - {userPredictions[i].CorrectPredictions}");
        }

        sb.AppendLine("```");
        var embed = new EmbedBuilder()
            .WithColor(Colors.Green)
            .WithTitle("Top Predictors By Correct Predictions Number")
            .WithDescription(sb.ToString())
            .Build();
        return embed;
    }

    private async Task<List<UserPrediction>> GetUserPredictionsByNumber()
    {
        var userPredictions = (await _horseRaceService.GetAllPredictionsAsync()).ToList();
        userPredictions = userPredictions.OrderByDescending(x => x.CorrectPredictions).Take(10).ToList();
        return userPredictions;
    }

    public async Task GetTopPredictionsByPercentage(SocketInteractionContext context)
    {
        var userPredictions = await GetUserPredictionsByPercentage();

        if (!userPredictions.Any())
        {
            await context.Interaction.RespondAsync(embed: new EmbedBuilder()
                .WithColor(Colors.Red)
                .WithDescription("No predictions found!")
                .Build());
            return;
        }

        var embed = await GetUserPredictionsByPercentageEmbed(userPredictions);
        await context.Interaction.RespondAsync(embed: embed);
    }

    private async Task<Embed> GetUserPredictionsByPercentageEmbed(List<UserPrediction> userPredictions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("```");

        for (int i = 0; i < userPredictions.Count; i++)
        {
            var user = await _client.GetUserAsync(userPredictions[i].UserId);
            sb.AppendLine(
                $"#{i + 1} - {user?.Username ?? "UserNotFound"} - {((userPredictions[i].CorrectPredictions * 1.0) / (userPredictions[i].CorrectPredictions + userPredictions[i].WrongPredictions)) * 100:F}%");
        }

        sb.AppendLine("```");

        var embed = new EmbedBuilder()
            .WithColor(Colors.Green)
            .WithTitle("Top Predictors By Correct Predictions Percentage")
            .WithDescription(sb.ToString())
            .Build();
        return embed;
    }

    private async Task<List<UserPrediction>> GetUserPredictionsByPercentage()
    {
        var userPredictions = (await _horseRaceService.GetAllPredictionsAsync()).ToList();
        userPredictions = userPredictions
            .OrderByDescending(x => (x.CorrectPredictions * 1.0) / (x.CorrectPredictions + x.WrongPredictions)).Take(10)
            .ToList();
        return userPredictions;
    }

    public async Task GetTopHorsesByNumber(SocketInteractionContext context)
    {
        var horsesStandings = await GetTopHorsesByNumber();

        if (!horsesStandings.Any())
        {
            await context.Interaction.RespondAsync(embed: new EmbedBuilder()
                           .WithColor(Colors.Red)
                           .WithDescription("No races found!")
                           .Build());
            return;
        }

        var embed = GetHorseByNumberEmbed(horsesStandings);
        await context.Interaction.RespondAsync(embed: embed);
    }

    private static Embed GetHorseByNumberEmbed(List<HorsesAndWins> horsesStandings)
    {
        var sb = new StringBuilder();
        sb.AppendLine("```");

        for (int i = 0; i < horsesStandings.Count; i++)
        {
            sb.AppendLine($"#{i + 1} - {horsesStandings[i].Horse.Name} - {horsesStandings[i].RacesWon}");
        }

        sb.AppendLine("```");
        var embed = new EmbedBuilder()
            .WithColor(Colors.Green)
            .WithTitle("Top Horses By Race Won Number")
            .WithDescription(sb.ToString())
            .Build();
        return embed;
    }

    private async Task<List<HorsesAndWins>> GetTopHorsesByNumber()
    {
        var horsesStandings = (await _horseRaceService.GetHorsesStandings()).ToList();
        horsesStandings = horsesStandings.OrderByDescending(x => x.RacesWon).Take(10).ToList();
        return horsesStandings;
    }

    public async Task GetTopHorsesByPercentage(SocketInteractionContext context)
    {
        var horsesStandings = await GetHorsesByPercentage();

        if (!horsesStandings.Any())
        {
            await context.Interaction.RespondAsync(embed: new EmbedBuilder()
                .WithColor(Colors.Red)
                .WithDescription("No races found!")
                .Build());
            return;
        }

        var embed = GetHorsesPercentageEmbed(horsesStandings);
        await context.Interaction.RespondAsync(embed: embed);
    }

    private static Embed GetHorsesPercentageEmbed(List<HorsesAndWins> horsesStandings)
    {
        var sb = new StringBuilder();
        sb.AppendLine("```");

        for (int i = 0; i < horsesStandings.Count; i++)
        {
            sb.AppendLine($"#{i + 1} - {horsesStandings[i].Horse.Name} - {horsesStandings[i].WinPercentage * 100:F}%");
        }

        sb.AppendLine("```");

        var embed = new EmbedBuilder()
            .WithColor(Colors.Green)
            .WithTitle("Top Horses By Race Won Percentage")
            .WithDescription(sb.ToString())
            .Build();
        return embed;
    }

    private async Task<List<HorsesAndWins>> GetHorsesByPercentage()
    {
        var horsesStandings = (await _horseRaceService.GetHorsesStandings()).ToList();
        horsesStandings = horsesStandings.OrderByDescending(x => x.WinPercentage).Take(10).ToList();
        return horsesStandings;
    }
}
