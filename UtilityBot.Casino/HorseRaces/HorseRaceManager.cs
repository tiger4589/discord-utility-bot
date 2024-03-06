using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;
using UtilityBot.Domain.Services.HorseRaceServices;
using UtilityBot.Domain.Services.HorseRaceServices.Models;
using Timer = System.Timers.Timer;
namespace UtilityBot.Casino.HorseRaces;

public interface IHorseRaceManager
{
    Task LoadConfiguration();
    event EventHandler<RaceEventArgs>? RaceStarting;
}

public class HorseRaceManager : IHorseRaceManager
{
    private readonly IHorseRaceService _horseRaceService;
    private HorsesAndTracks? _horsesAndTracks;

    public HorseRaceManager(IHorseRaceService horseRaceService)
    {
        _horseRaceService = horseRaceService;
    }

    public async Task LoadConfiguration()
    {
        _horsesAndTracks = await _horseRaceService.GetHorsesAndTracksAsync();
        //DISABLED FOR NOW
        //foreach (var track in _horsesAndTracks.Tracks)
        //{
        //    StartRaceOnTrack(track);
        //}
    }

    private void StartRaceOnTrack(Track track)
    {
        var horses = _horsesAndTracks!.Horses.ToList();
        var timer = new Timer(track.TimeBetweenRacesInMinutes * 60 * 1000);
        var race = new Race(timer, track, horses);
        timer.Elapsed += async (_, _) => { await StartRace(race); };

        timer.AutoReset = false;
        timer.Enabled = true;
    }

    private async Task StartRace(Race race)
    {
        try
        {
            race.Initialize();
            RaiseRaceStartingEvent(new RaceEventArgs(race));
            await Task.Delay(2 * 60 * 1000);
            await race.Start();
            
            var standings = race.GetFinalStandings();
            await _horseRaceService.AddRaceAsync(race.Track.Id, standings);

            var (correctPredictions, wrongPredictions) = race.GetPredictions();

            await _horseRaceService.InsertPredictions(correctPredictions, wrongPredictions);

            StartRaceOnTrack(race.Track);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public event EventHandler<RaceEventArgs>? RaceStarting;
    private protected void RaiseRaceStartingEvent(RaceEventArgs args)
    {
        var handler = RaceStarting;
        handler?.Invoke(this, args);
    }
}