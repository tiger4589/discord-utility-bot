using Microsoft.Extensions.Configuration;
using UtilityBot.Services.ApiCallerServices;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.HangmanServices;

public interface IHangmanService
{
    Task<HangmanWord?> GetRandomWord();
}

public class HangmanService : BaseApiCallService, IHangmanService
{
    private readonly IConfiguration _configuration;

    public HangmanService(IConfiguration configuration) : base(configuration)
    {
        _configuration = configuration;
    }

    public override string? ServiceUrl { get; set; } = "https://wordsapiv1.p.rapidapi.com/words/";

    public async Task<HangmanWord?> GetRandomWord()
    {
        var apiUrl = _configuration["WordsApiUrl"]!;
        ServiceUrl = string.Concat(apiUrl, "?random=true");

        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "User-Agent", "discord-utility-bot" },
            {"X-RapidAPI-Key",_configuration["WordsApiKey"]!},
            {"X-RapidAPI-Host", _configuration["WordsApiHost"]!}
        };

        var word = await GetApiFromServiceUrl<HangmanWord>(headers);

        if (word == null)
        {
            await Logger.Log($"Couldn't find a random word from API");
            return null;
        }

        return word;
    }
}

public class HangmanWord
{
    public string Word { get; set; } = null!;
    public Dictionary<string, string> Definitions { get; set; } = null!;
}