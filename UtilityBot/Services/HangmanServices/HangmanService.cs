﻿using Microsoft.Extensions.Configuration;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.HangmanServices;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.HangmanServices;

public interface IHangmanService
{
    Task<(HangmanWord?, bool)> GetRandomWord();
    Task SaveGame(HangmanGame game, ulong userId);
    Task<HangmanPersonalStats> GetPersonalStats(ulong userId);
    Task<List<HangmanTopStats>> GetTopStats();
}

public class HangmanService : BaseApiCallService, IHangmanService
{
    private readonly IConfiguration _configuration;
    private readonly IHangmanDomainService _domainService;

    public HangmanService(IConfiguration configuration, IHangmanDomainService domainService) : base(configuration)
    {
        _configuration = configuration;
        _domainService = domainService;
    }

    public override string? ServiceUrl { get; set; } = "https://wordsapiv1.p.rapidapi.com/words/";

    public async Task<(HangmanWord?, bool)> GetRandomWord()
    {
        var (hangmanWord, isOk) = await GetRandomWordsFromWordnik();

        if (isOk)
        {
            return (hangmanWord, false);
        }

        return await GetRandomWordFromWordsApi();
    }

    private async Task<(HangmanWord?, bool)> GetRandomWordsFromWordnik()
    {
        var allowed = await _domainService.IsAllowed(EWordsApiSource.Wordnik, 50, -1);
        if (!allowed)
        {
            await Logger.Log($"Hangman word request limit reached For Wordnik");
            return (null, true);
        }

        var apiUrl = _configuration["WordnikBaseUrl"]!;
        var apiKey = _configuration["WordnikApiKey"]!;
        ServiceUrl = string.Concat(apiUrl, $"words.json/randomWord?hasDictionaryDef=true&maxCorpusCount=-1&minDictionaryCount=1&maxDictionaryCount=-1&minLength=5&maxLength=-1&api_key={apiKey}");

        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "User-Agent", "discord-utility-bot" }
        };

        var word = await GetApiFromServiceUrl<WordnikWord>(headers);

        if (word == null || string.IsNullOrWhiteSpace(word.Word))
        {
            await Logger.Log($"Couldn't find a random word from Wordnik API");
            return (null, false);
        }

        ServiceUrl = string.Concat(apiUrl, $"word.json/{word.Word}/definitions?limit=5&includeRelated=false&sourceDictionaries=all&useCanonical=false&includeTags=false&api_key={apiKey}");
        var words = await GetApiFromServiceUrl<List<WordnikWord>>(headers);

        if (words == null || words.Count == 0)
        {
            await Logger.Log($"Couldn't find a random word definition from Wordnik API");
            return (null, false);
        }

        word = words.Count == 1  
            ? words.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Text)) 
            : words.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Text) && !x.Text.Contains('<'));

        await _domainService.AddWordRequest(word?.Word ?? "WordNotReturned",
            word?.Text,
            EWordsApiSource.Wordnik);

        if (word == null)
        {
            await Logger.Log($"Couldn't find a random word from Wordnik API");
            return (null, false);
        }

        return (new HangmanWord
        {
            Results = new List<Result>
            {
                new()
                {
                    Definition = word.Text?.Replace("<xref>","").Replace("</xref>","")
                        .Replace("<internalXref urlencoded=\"reard\">", "").Replace("</internalXref>","")
                }
            },
            Word = word.Word!
        }, true);
    }

    private async Task<(HangmanWord?, bool)> GetRandomWordFromWordsApi()
    {
        var allowed = await _domainService.IsAllowed(EWordsApiSource.WordsApi, 2500, -24);
        if (!allowed)
        {
            await Logger.Log($"Hangman word request limit reached");
            return (null, true);
        }

        var apiUrl = _configuration["WordsApiUrl"]!;
        ServiceUrl = string.Concat(apiUrl, "?random=true");

        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "User-Agent", "discord-utility-bot" },
            { "X-RapidAPI-Key", _configuration["WordsApiKey"]! },
            { "X-RapidAPI-Host", _configuration["WordsApiHost"]! }
        };

        var word = await GetApiFromServiceUrl<HangmanWord>(headers);

        await _domainService.AddWordRequest(word?.Word ?? "WordNotReturned",
            word?.Results.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Definition))?.Definition,
            EWordsApiSource.WordsApi);

        if (word == null)
        {
            await Logger.Log($"Couldn't find a random word from API");
            return (null, false);
        }

        return (word, false);
    }

    public async Task SaveGame(HangmanGame game, ulong userId)
    {
        await _domainService.AddGame(new HangmanGames
        {
            IsCorrect = game.State == HangmanGameState.Finished,
            Score = game.Score,
            Word = game.Word.Word,
            UserId = userId
        });
    }

    public async Task<HangmanPersonalStats> GetPersonalStats(ulong userId)
    {
        var games = (await _domainService.GetGames(userId)).ToList();
        return new HangmanPersonalStats
        {
            TotalGames = games.Count,
            TotalScore = games.Sum(x => x.Score),
            TotalWins = games.Count(x => x.IsCorrect)
        };
    }

    public async Task<List<HangmanTopStats>> GetTopStats()
    {
        var games = (await _domainService.GetGames()).ToList();

        var topStats = games.GroupBy(x => x.UserId)
            .Select(x => new HangmanTopStats
            {
                TotalGames = x.Count(),
                TotalScore = x.Sum(y => y.Score),
                TotalWins = x.Count(y => y.IsCorrect),
                UserId = x.Key
            })
            .ToList();

        return topStats;
    }
}

public class WordnikWord
{
    public string Word { get; set; } = null!;
    public string? SourceDictionary { get; set; }
    public string? Text { get; set; }
}

public class HangmanWord
{
    public string Word { get; set; } = null!;
    public List<Result> Results { get; set; } = new();
}

public class Result
{
    public string? Definition { get; set; }
}

public class HangmanPersonalStats
{
    public int TotalScore { get; set; }
    public int TotalGames { get; set; }
    public int TotalWins { get; set; }
}

public class HangmanTopStats
{
    public int TotalScore { get; set; }
    public int TotalGames { get; set; }
    public int TotalWins { get; set; }
    public ulong UserId { get; set; }
}