using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Contracts;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.MessageHandlers;

namespace UtilityBot.Services.UserJoinedServices;

public class UserJoinedService : IUserJoinedService
{
    private readonly ICacheManager _cahCacheManager;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly IMessageHandler _messageHandler;

    public UserJoinedService(ICacheManager cahCacheManager, DiscordSocketClient client, IConfiguration configuration, IMessageHandler messageHandler)
    {
        _cahCacheManager = cahCacheManager;
        _client = client;
        _configuration = configuration;
        _messageHandler = messageHandler;
        _client.Ready += InitializeService;
    }

    public async Task InitializeService()
    {
        _client.UserJoined += ClientOnUserJoined;
        await Logger.Log("Listening to UserJoined event from now on!");
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

    private async Task SendMessageOnJoin(SocketGuildUser socketGuildUser, IList<UserJoinMessage> configUserJoinMessages, bool isForcedInPrivate = false)
    {
        foreach (var configUserJoinMessage in configUserJoinMessages)
        {
            var message = _messageHandler.ReplacePlaceHolders(configUserJoinMessage.Message, socketGuildUser);
            if (configUserJoinMessage.IsPrivate || isForcedInPrivate)
            {
                await socketGuildUser.SendMessageAsync(message);
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

                await channel.SendMessageAsync(message);
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

    public async Task TriggerAfterRestart(Configuration configuration)
    {
        try
        {
            var guild = _client.GetGuild(ulong.Parse(_configuration["ServerId"]!));
            var users = guild.Users;

            foreach (var user in users)
            {
                if ((user.Roles.Count == 1 && user.Roles.Single().IsEveryone) || user.Roles.Count == 0)
                {
                    foreach (var userJoinConfiguration in configuration.UserJoinConfigurations)
                    {
                        if (userJoinConfiguration.Action == ActionTypeNames.AddRole)
                        {
                            await AddRolesOnJoin(user, configuration.UserJoinRoles);
                        }

                        if (userJoinConfiguration.Action == ActionTypeNames.SendMessage)
                        {
                            await SendMessageOnJoin(user, configuration.UserJoinMessages, true);
                        }
                    }
                }
            }

            await Logger.Log("Should have finished checking for missed users while offline");
        }
        catch (Exception e)
        {
            await Logger.Log($"Errorrrrr!!! {e}");
        }
    }
}