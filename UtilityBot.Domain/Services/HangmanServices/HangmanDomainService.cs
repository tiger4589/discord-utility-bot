using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.HangmanServices;

public interface IHangmanDomainService
{
    Task<bool> IsAllowed(EWordsApiSource source, int allowedNumbers, int lastHours);
    Task AddWordRequest(string word, string? definition, EWordsApiSource source);
    Task AddGame(HangmanGames game);
    Task<IEnumerable<HangmanGames>> GetGames(ulong userId);
    Task<IEnumerable<HangmanGames>> GetGames();
}

public class HangmanDomainService : IHangmanDomainService
{
    private readonly UtilityBotContext _context;

    public HangmanDomainService(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task<bool> IsAllowed(EWordsApiSource source, int allowedNumbers, int lastHours)
    {
        var last24H = DateTime.UtcNow.AddHours(lastHours);

        var count = await _context.HangmanWordRequests!
            .Where(x => x.RequestedAt > last24H && x.Source == source)
            .CountAsync();

        return count < allowedNumbers;
    }

    public async Task AddWordRequest(string word, string? definition, EWordsApiSource source)
    {
        await _context.HangmanWordRequests!.AddAsync(new HangmanWordRequest
        {
            Definition = definition,
            RequestedAt = DateTime.UtcNow,
            Word = word,
            Source = source
        });
        await _context.SaveChangesAsync();
    }

    public async Task AddGame(HangmanGames game)
    {
        await _context.HangmanGames!.AddAsync(game);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<HangmanGames>> GetGames(ulong userId)
    {
        return await _context.HangmanGames!.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<HangmanGames>> GetGames()
    {
        return await _context.HangmanGames!.AsNoTracking().ToListAsync();
    }
}