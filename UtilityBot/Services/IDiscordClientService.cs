using Discord.WebSocket;

namespace UtilityBot.Services;

public interface IDiscordClientService
{
    Task InitializeService(DiscordSocketClient client);
}