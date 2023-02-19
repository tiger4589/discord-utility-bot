using Discord;
using UtilityBot.Contracts;

namespace UtilityBot.Services.UserJoinedServices;

public interface IUserJoinedService
{
    Task TriggerSendMessageOnJoin(IUser user);
    Task TriggerAfterRestart(Configuration configuration);
}