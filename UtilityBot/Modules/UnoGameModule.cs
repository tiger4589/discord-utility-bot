using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Services.Uno.Manager;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;

namespace UtilityBot.Modules;

public class UnoGameModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IUnoGameManager _unoGameManager;

    public UnoGameModule(IUnoGameManager unoGameManager)
    {
        _unoGameManager = unoGameManager;
    }

    [SlashCommand("initialize-uno", "Start a new game of UNO!")]
    public async Task InitializeUno()
    {
        await RespondAsync("Initializing a new game", ephemeral: true);
        _ = _unoGameManager.InitializeGame(Context);
    }

    [ComponentInteraction("start-uno_*")]
    public async Task StartUno(string gameId)
    {
        await _unoGameManager.StartGame(Context, Guid.Parse(gameId));
    }

    [ComponentInteraction("join-uno_*")]
    public async Task JoinUno(string gameId)
    {
        await _unoGameManager.JoinGame(Context, Guid.Parse(gameId));
    }

    [ComponentInteraction("card_*")]
    public async Task PlayCard(string cardId)
    {
        await _unoGameManager.PlayCard(Context, Guid.Parse(cardId));
    }

    [ComponentInteraction("wild_*_*")]
    public async Task PlayWildCard(string color, string cardId)
    {
        await _unoGameManager.PlayWildCard(Context,  color, Guid.Parse(cardId));
    }

    [ComponentInteraction("show-card-prompt")]
    public async Task ShowCardPrompt()
    {
        await Context.Interaction.RespondAsync("Please click the button below 😀",
            components: new ComponentBuilder()
                .WithButton("Click here to view your cards", "show-card-menu", style: ButtonStyle.Secondary)
                .Build(), ephemeral: true);
    }

    [ComponentInteraction("show-card-menu")]
    public async Task ShowCardMenu()
    {
        await _unoGameManager.ShowCards((SocketMessageComponent)Context.Interaction);
    }

    [ComponentInteraction("draw-card")]
    public async Task DrawCard()
    {
        await _unoGameManager.DrawCard(Context);
    }

    //[ComponentInteraction("cancel-uno_*")]
    //public async Task CancelUno(Guid gameId)
    //{

    //}

    //[ComponentInteraction("leave-uno_*")]
    //public async Task LeaveUno(Guid gameId)
    //{

    //}
}