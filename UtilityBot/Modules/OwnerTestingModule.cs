using System.Runtime.CompilerServices;
using Discord;
using Discord.Interactions;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Services.UserJoinedServices;

namespace UtilityBot.Modules;

[Group("owner", "my testing module")]
[RequireOwner(Group = "owner")]
public class OwnerTestingModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IUserJoinedService _userJoinedService;

    public OwnerTestingModule(IUserJoinedService userJoinedService)
    {
        _userJoinedService = userJoinedService;
    }

    [SlashCommand("join-user", "test-on-join-user")]
    public async Task SendMessage(IUser user)
    {
        await _userJoinedService.TriggerSendMessageOnJoin(user);
        await RespondAsync("Done", ephemeral: true);
    }

    [SlashCommand("test-choices", "just-testing-choices")]
    public async Task ChooseEvent(EEventName eventName)
    {
        await RespondAsync($"You have chosen {eventName}");
    }

    //[SlashCommand("test-shuffle", "just-testing-shuffle")]
    //public async Task ShuffleDeck()
    //{
    //    UnoGame game = new();
    //    game.ShuffleDeck();
    //    await RespondAsync("done");
    //}
}
