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

    [SlashCommand("request-coder-status", "Request access to coders channels, alliance coders it is")]
    public async Task RequestCoderStatus(
        [Summary(description:"Your in-game recruit link")]string recruitLink,
        [Summary(description: "Your in-game alliance link")] string allianceGameLink,
        [Summary(description: "What kind of application do you code for your alliance?")] string role)
    {
        await _playerService.RequestCoderStatus(Context, recruitLink, allianceGameLink, role);
    }
}