using Discord;

namespace UtilityBot.Services.UserJoinedServices;

public interface IUserJoinedService
{
    Task TriggerSendMessageOnJoin(IUser user);
}