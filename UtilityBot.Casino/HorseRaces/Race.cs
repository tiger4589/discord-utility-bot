using System.Text;
using Discord;
using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;
using Timer = System.Timers.Timer;

namespace UtilityBot.Casino.HorseRaces;

public class Race
{
    public IUserMessage? UserMessage { get; set; }
    public Timer Timer { get; set; }
    public Track Track { get; set; }
    public List<Horse> Horses { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    private readonly Random _random = new();
    private readonly List<Prediction> _predictions = new();
    private readonly object _lock = new();
    public bool IsFinished => _horseSteps.Any(x => x.StepsTaken >= Track.Length);

    private readonly List<HorseStep> _horseSteps = new List<HorseStep>();

    public Race(Timer timer, Track track, List<Horse> horses)
    {
        Timer = timer;
        Track = track;
        List<Horse> allHorses = horses;
        Shuffle(allHorses);
        Horses = allHorses.Take(6).ToList();
    }

    public void Initialize()
    {
        foreach (var horse in Horses)
        {
            _horseSteps.Add(new HorseStep
            {
                Horse = horse,
                StepsTaken = 0
            });
        }
    }

    public async Task Stop()
    {

    }

    public async Task Start()
    {
        while (!IsFinished)
        {
            await Step();
            await Task.Delay(2000);
        }

        if (UserMessage == null)
        {
            Console.WriteLine("A race that I couldn't show just ended.");
            return;
        }

        try
        {
            await UserMessage!.ModifyAsync(properties => properties.Embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder
                {
                    Name = "Race Manager"
                })
                .WithDescription($"Race Finished!")
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .WithFields(new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                        .WithName("Track Info")
                        .WithValue(string.Concat("Name: ", Track.Name, Environment.NewLine, "Length: ", Track.Length, Environment.NewLine, "Type: ", Track.Type)),
                    new EmbedFieldBuilder()
                        .WithName("Final Standing")
                        .WithValue(string.Join(Environment.NewLine, GetStandingsWithOdds()))
                })
                .Build());
        }
        catch (Exception e)
        {
            Console.WriteLine("Couldn't show race end results.. poor fellas.");
        }
    }

    public (List<ulong>,List<ulong>) GetPredictions()
    {
        var winner = _horseSteps.OrderByDescending(x => x.StepsTaken).First();
        lock (_lock)
        {
            var correctPredictions = _predictions.Where(x => x.HorseId == winner.Horse.Id).Select(x => x.UserId).ToList();
            var wrongPredictions = _predictions.Where(x => x.HorseId != winner.Horse.Id).Select(x => x.UserId).ToList();

            return (correctPredictions, wrongPredictions);
        }
    }

    public async Task Step()
    {
        foreach (var horseStep in _horseSteps)
        {
            horseStep.StepsTaken += GetStepsToMake(horseStep);
        }

        if (UserMessage == null)
        {
            Console.WriteLine("A step was taken in a race that I couldn't show!");
            return;
        }

        try
        {
            await UserMessage!.ModifyAsync(properties =>
            {
                properties.Embed = new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder
                    {
                        Name = "Race Manager"
                    })
                    .WithDescription($"Race is underway!")
                    .WithColor(Color.DarkBlue)
                    .WithCurrentTimestamp()
                    .WithFields(new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                            .WithName("Track Info")
                            .WithValue(string.Concat("Name: ", Track.Name, Environment.NewLine, "Length: ", Track.Length, Environment.NewLine, "Type: ", Track.Type)),
                        new EmbedFieldBuilder()
                            .WithName("Current Standing")
                            .WithValue(GetStandings())
                    })
                    .Build();
                properties.Components = new ComponentBuilder().Build();
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't update a race message: {e.Message}");
        }
    }

    private int GetStepsToMake(HorseStep horseStep)
    {
        double chance = 1.0 / (horseStep.Horse.OddsToOne * 1.0);
        var actual = _random.NextDouble();

        if (actual <= chance)
        {
            var steps = _random.Next(2, Math.Max(9, horseStep.Horse.OddsToOne));
            horseStep.LapsSinceLastStep = 0;
            return GetStepsAccordingToTrackType(steps, horseStep);
        }

        if (horseStep.LapsSinceLastStep < 7)
        {
            horseStep.LapsSinceLastStep++;
            return GetStepsAccordingToTrackType(_random.Next(1, 3), horseStep);
        }

        var forcedSteps = _random.Next(Math.Max(horseStep.Horse.OddsToOne / 2, 1), horseStep.Horse.OddsToOne + 1);
        horseStep.LapsSinceLastStep = 0;
        return GetStepsAccordingToTrackType(forcedSteps, horseStep);
    }

    private int GetStepsAccordingToTrackType(int steps, HorseStep horseStep)
    {
        if (horseStep.Horse.AdvantageOn == Track.Type)
        {
            return steps + 1;
        }

        if (horseStep.Horse.DisadvantageOn == Track.Type)
        {
            return steps - 1;
        }

        return steps;
    }

    public string GetStandings()
    {
        var sb = new StringBuilder();
        sb.AppendLine("```");
        var pos = "Position";
        var name = "Horse";
        var steps = "Steps Taken";
        var maxNameLength = _horseSteps.Max(x => x.Horse.Name.Length);
        name = name.PadRight(maxNameLength);
        sb.AppendLine($@"{pos} | {name} | {steps}");
        var ordered = _horseSteps.OrderByDescending(x => x.StepsTaken).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            var horseStep = ordered[i];
            var rank = (i + 1).ToString().PadRight(pos.Length - 1);
            var horseName = horseStep.Horse.Name.PadRight(maxNameLength);
            sb.AppendLine($@"#{rank} | {horseName} | {horseStep.StepsTaken}");
        }
        sb.AppendLine("```");
        return sb.ToString();
    }

    public List<string> GetStandingsWithOdds()
    {
        var standings = new List<string>();
        var ordered = _horseSteps.OrderByDescending(x => x.StepsTaken).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            var horseStep = ordered[i];
            standings.Add($"#{i + 1} - {horseStep.Horse.Name} - 1-{horseStep.Horse.OddsToOne} - Advantage: {GetAdvantageEmoji(ordered[i])}");
        }
        return standings;
    }

    private string GetAdvantageEmoji(HorseStep horseStep)
    {
        if (horseStep.Horse.AdvantageOn == Track.Type)
        {
            return "⬆️";
        }

        if (horseStep.Horse.DisadvantageOn == Track.Type)
        {
            return "⬇️";
        }

        return "➖";
    }

    public List<RaceStanding> GetFinalStandings()
    {
        var standings = _horseSteps.OrderByDescending(x => x.StepsTaken).ToList();
        var result = new List<RaceStanding>();

        for (int i = 0; i < standings.Count; i++)
        {
            result.Add(new()
            {
                HorseId = standings[i].Horse.Id,
                Position = i + 1
            });
        }

        return result;
    }

    public bool AddPrediction(Prediction prediction)
    {
        lock (_lock)
        {
            if (_predictions.Any(x => x.UserId == prediction.UserId))
            {
                return false;
            }

            _predictions.Add(prediction);
            return true;
        }
    }

    static void Shuffle<T>(List<T> list)
    {
        Random random = new Random();

        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

public class HorseStep
{
    public Horse Horse { get; set; } = null!;
    public int StepsTaken { get; set; }
    public int LapsSinceLastStep { get; set; }
}

public class Prediction
{
    public ulong UserId { get; set; }
    public int HorseId { get; set; }
}
