using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using UtilityBot.Services.HangmanServices;

namespace UtilityBot.Modules;

public class HangmanModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHangmanGameManager _hangmanGameManager;
    private readonly IConfiguration _configuration;

    public HangmanModule(IHangmanGameManager hangmanGameManager, IConfiguration configuration)
    {
        _hangmanGameManager = hangmanGameManager;
        _configuration = configuration;
    }

    [SlashCommand("hangman", "Start a hangman game!")]
    public async Task StartHangman()
    {
        var channelId = ulong.Parse(_configuration["HangmanChannelId"]!);

        if (Context.Channel.Id != channelId)
        {
            await Context.Interaction.RespondAsync("You can only start a hangman game in the hangman channel!");
            return;
        }

        await Context.Interaction.RespondAsync("Starting a new hangman game - Stay Put!");
        await _hangmanGameManager.StartNewGame(Context);
    }

    [SlashCommand("force-stop-my-game", "Force stop your hangman game.")]
    public async Task ForceStopHangman()
    {
        var channelId = ulong.Parse(_configuration["HangmanChannelId"]!);

        if (Context.Channel.Id != channelId)
        {
            await Context.Interaction.RespondAsync("You can only force stop a hangman game in the hangman channel!");
            return;
        }

        await Context.Interaction.RespondAsync("Force stopping your hangman game - Stay Put!");
        await _hangmanGameManager.ForceStopGame(Context);
    }

    [SlashCommand("hangman-personal-stats", "Get your stats of the game!")]
    public async Task GetPersonalStats()
    {
        var channelId = ulong.Parse(_configuration["HangmanChannelId"]!);

        if (Context.Channel.Id != channelId)
        {
            await Context.Interaction.RespondAsync("You can only get your hangman stats in the hangman channel!");
            return;
        }

        await Context.Interaction.RespondAsync("Getting your hangman stats");
        await _hangmanGameManager.GetPersonalStats(Context);
    }

    [SlashCommand("hangman-top-stats", "Get your stats of the game!")]
    public async Task GetTopStats(SortBy sortBy)
    {
        var channelId = ulong.Parse(_configuration["HangmanChannelId"]!);

        if (Context.Channel.Id != channelId)
        {
            await Context.Interaction.RespondAsync("You can only get your hangman stats in the hangman channel!");
            return;
        }

        await Context.Interaction.RespondAsync($"Getting top stats sorted by {sortBy}");
        await _hangmanGameManager.GetTopStats(Context, sortBy);
    }

    [ComponentInteraction("guess_letter_*")]
    public async Task GuessLetter(char letter)
    {
        await _hangmanGameManager.GuessLetter(Context, letter);
    }

    [ComponentInteraction("less_letters_hangman")]
    public async Task GoBackWithLetters()
    {
        await _hangmanGameManager.GoBackWithLetters(Context);
    }    
    
    [ComponentInteraction("give_up_hangman")]
    public async Task GiveUp()
    {
        await _hangmanGameManager.GiveUpGame(Context);
    }    
    
    [ComponentInteraction("extra_letters_hangman")]
    public async Task GoNextWithLetters()
    {
        await _hangmanGameManager.GoNextWithLetters(Context);
    }
}