using Discord.Interactions;
using UtilityBot.Services.HangmanServices;

namespace UtilityBot.Modules;

public class HangmanModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHangmanService _hangmanService;

    public HangmanModule(IHangmanService hangmanService)
    {
        _hangmanService = hangmanService;
    }

    [SlashCommand("hangman", "Start a hangman game!")]
    public async Task StartHangman()
    {
        var word = await _hangmanService.GetRandomWord();
        if (word == null)
        {
            await Context.Interaction.RespondAsync($"Couldn't find a word!");
            return;
        }

        await Context.Interaction.RespondAsync($"Your word for now is {word.Word}");

    }
}