using Discord;
using Discord.WebSocket;
using UtilityBot.Contracts;
using UtilityBot.Services.CacheService;

namespace UtilityBot.Services.UserJoinedServices;

public class UserJoinedService : IUserJoinedService
{
    private readonly ICacheManager _cahCacheManager;
    private readonly DiscordSocketClient _client;

    public UserJoinedService(ICacheManager cahCacheManager, DiscordSocketClient client)
    {
        _cahCacheManager = cahCacheManager;
        _client = client;
        _client.Ready += InitializeService;
    }

    public Task InitializeService()
    {
        _client.UserJoined += ClientOnUserJoined;
        return Task.CompletedTask;
    }

    private async Task ClientOnUserJoined(SocketGuildUser arg)
    {
        var config = _cahCacheManager.GetGuildOnJoinConfiguration(arg.Guild.Id);

        if (config == null)
        {
            return;
        }

        foreach (var userJoinConfiguration in config.UserJoinConfigurations)
        {
            if (userJoinConfiguration.Action == ActionTypeNames.AddRole)
            {
                await AddRolesOnJoin(arg, config.UserJoinRoles);
            }

            if (userJoinConfiguration.Action == ActionTypeNames.SendMessage)
            {
                await SendMessageOnJoin(arg, config.UserJoinMessages);
            }
        }
    }

    private async Task SendMessageOnJoin(SocketGuildUser socketGuildUser, IList<UserJoinMessage> configUserJoinMessages)
    {
        foreach (var configUserJoinMessage in configUserJoinMessages)
        {
            if (configUserJoinMessage.IsPrivate)
            {
                await socketGuildUser.SendMessageAsync(configUserJoinMessage.Message);
            }
            else
            {
                if (configUserJoinMessage.ChannelId == null)
                {
                    continue;
                }

                var channel = socketGuildUser.Guild.GetChannel(configUserJoinMessage.ChannelId.Value) as ISocketMessageChannel;
                if (channel == null)
                {
                    continue;
                }

                await channel.SendMessageAsync($"<@{socketGuildUser.Id}>: {configUserJoinMessage.Message}");
            }
        }
    }

    private async Task AddRolesOnJoin(SocketGuildUser user, IList<UserJoinRole> joinRoles)
    {
        foreach (var userJoinRole in joinRoles)
        {
            await user.AddRoleAsync(userJoinRole.RoleId);
        }
    }

    public async Task TriggerSendMessageOnJoin(IUser user)
    {
        await ClientOnUserJoined((SocketGuildUser)user);
    }
}