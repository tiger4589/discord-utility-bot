using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects.CasinoModels.HorseRaces;
using UtilityBot.Domain.Services.HorseRaceServices.Models;

namespace UtilityBot.Domain.Services.HorseRaceServices;

public interface IHorseRaceService
{
    Task<HorsesAndTracks> GetHorsesAndTracksAsync();
    Task<int> AddRaceAsync(int trackId, List<RaceStanding> standings);
    Task InsertPredictions(List<ulong> correctPredictions, List<ulong> wrongPredictions);
    Task<IEnumerable<UserPrediction>> GetAllPredictionsAsync();
    Task<IEnumerable<HorsesAndWins>> GetHorsesStandings();
}

public class HorseRaceService : IHorseRaceService
{
    private readonly UtilityBotContext _context;

    public HorseRaceService(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task<HorsesAndTracks> GetHorsesAndTracksAsync()
    {
        var horses = await _context.Horses!.ToListAsync();
        var tracks = await _context.Tracks!.ToListAsync();

        return new HorsesAndTracks
        {
            Horses = horses,
            Tracks = tracks
        };
    }

    public async Task<int> AddRaceAsync(int trackId, List<RaceStanding> standings)
    {
        var race = new HorseRace
        {
            TrackId = trackId,
            RaceDate = DateTime.Now
        };

        _context.HorseRaces!.Add(race);

        await _context.SaveChangesAsync();
        standings.ForEach(x => x.RaceId = race.Id);
        _context.RaceStandings!.AddRange(standings);

        return race.Id;
    }

    public async Task InsertPredictions(List<ulong> correctPredictions, List<ulong> wrongPredictions)
    {
        var userPredictions = await _context.UserPredictions!.ToListAsync();
        foreach (var prediction in correctPredictions)
        {
            var userPrediction = userPredictions.SingleOrDefault(x => x.UserId == prediction);
            if (userPrediction != null)
            {
                userPrediction.CorrectPredictions++;
            }
            else
            {
                _context.UserPredictions!.Add(new UserPrediction { UserId = prediction, CorrectPredictions = 1, WrongPredictions = 0 });
            }
        }
        foreach (var prediction in wrongPredictions)
        {
            var userPrediction = userPredictions.SingleOrDefault(x => x.UserId == prediction);
            if (userPrediction != null)
            {
                userPrediction.WrongPredictions++;
            }
            else
            {
                _context.UserPredictions!.Add(new UserPrediction { UserId = prediction, CorrectPredictions = 0, WrongPredictions = 1 });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserPrediction>> GetAllPredictionsAsync()
    {
        return await _context.UserPredictions!.ToListAsync();
    }

    public async Task<IEnumerable<HorsesAndWins>> GetHorsesStandings()
    {
        return await _context.HorseRaces!
            .Join(_context.RaceStandings!, hr => hr.Id, rs => rs.RaceId, (hr, rs) => new { hr, rs })
            .Join(_context.Horses!, arg => arg.rs.HorseId, h => h.Id, (rs, h) => new { rs, h })
            .GroupBy(x => x.h.Id)
            .Select(x => new HorsesAndWins
            {
                Horse = x.First(y => y.h.Id == x.Key).h,
                RacesParticipatedAt = x.Count(),
                RacesWon = x.Count(y => y.rs.rs.Position == 1)
            }).ToListAsync();
    }
}