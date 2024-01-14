using Microsoft.Extensions.Configuration;
using UtilityBot.Services.ApiCallerServices;

namespace UtilityBot.Services.HangmanServices;

public interface IHangmanService
{

}

public class HangmanService : BaseApiCallService, IHangmanService
{
    public HangmanService(IConfiguration configuration) : base(configuration)
    {
    }

    public override string? ServiceUrl { get; set; } = "https://wordsapiv1.p.rapidapi.com/words/";
}

public class HangmanWord
{
    public string Word { get; set; } = null!;
    public Dictionary<string, string> Definitions { get; set; } = null!;
}