using Discord.Interactions;
using UtilityBot.Services.PlayerServices;

namespace UtilityBot.Modules;

public class PlayerModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IPlayerService _playerService;

    public PlayerModule(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [SlashCommand("verify", "Request To Be Verified")]
    public async Task RequestVerification(string recruitLink)
    {
        await _playerService.RequestVerification(Context, recruitLink);
    }
}