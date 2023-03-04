using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.RumbleServices;

public class RumbleService : IRumbleService
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IRumbleConfigurationService _rumbleConfigurationService;
    private readonly IConfiguration _configuration;

    public RumbleService(DiscordSocketClient client, ICacheManager cacheManager, IRumbleConfigurationService rumbleConfigurationService, IConfiguration configuration)
    {
        _client = client;
        _cacheManager = cacheManager;
        _rumbleConfigurationService = rumbleConfigurationService;
        _configuration = configuration;
        _client.Ready -= ClientOnReady;
        _client.Ready += ClientOnReady;
    }

    private async Task ClientOnReady()
    {
        var rumbleConfiguration = await _rumbleConfigurationService.GetLatestConfiguration();
        if (rumbleConfiguration != null)
        {
            _cacheManager.AddOrUpdate(rumbleConfiguration);
            await Logger.Log($"Loaded Rumble Configuration");
        }
        else
        {
            await Logger.Log($"Rumble Configuration is not set!");
        }

        var messages = await _rumbleConfigurationService.GetRumbleMessages();
        foreach (var messageConfiguration in messages)
        {
            _cacheManager.Add(messageConfiguration);
        }

        await Logger.Log($"Loaded Rumble Messages");

        _client.ReactionAdded -= ClientOnReactionAdded;
        _client.ReactionAdded += ClientOnReactionAdded;
        await Logger.Log($"Looking for battles, if I am configured!");
    }

    private async Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
    {
        var rumbleConfiguration = _cacheManager.GetRumbleConfiguration();
        if (rumbleConfiguration == null)
        {
            return;
        }

        if (rumbleConfiguration.ChannelId != arg2.Id)
        {
            return;
        }

        var user = (SocketGuildUser)arg3.User;

        var readOnlyCollection = user.Roles;
        if (readOnlyCollection.All(x => x.Id != rumbleConfiguration.BotRoleId))
        {
            return;
        }

        if (arg3.Emote.Name != rumbleConfiguration.EmojiToWatch)
        {
            return;
        }

        var serverId = ulong.Parse(_configuration["ServerId"]!);

        var socketGuild = _client.GetGuild(serverId);
        var socketRole = socketGuild.GetRole(rumbleConfiguration.RoleId);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(socketRole.Mention);

        var rumbleMessageConfigurations = _cacheManager.GetRumbleMessageConfigurations();
        Random rand = new Random();

        var messageConfiguration = rumbleMessageConfigurations[rand.Next(0, rumbleMessageConfigurations.Count)];

        sb.AppendLine(messageConfiguration.Message);

        await arg2.Value.SendMessageAsync(sb.ToString());

        if (rumbleConfiguration.JoinGame)
        {
            var message = await arg1.DownloadAsync();
            await message.AddReactionAsync(arg3.Emote);
        }
    }

    public async Task AddMessageConfiguration(SocketInteractionContext context, string message)
    {
        var rumbleMessageConfiguration = new RumbleMessageConfiguration
        {
            Message = message
        };

        await _rumbleConfigurationService.AddRumbleMessage(rumbleMessageConfiguration);

        _cacheManager.Add(rumbleMessageConfiguration);

        await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"Message added: {message}");
    }

    public async Task AddConfiguration(SocketInteractionContext context, ITextChannel channel, IRole botRole, string emoji,
        IRole roleToPing, bool isJoinGame)
    {
        var conf = new RumbleConfiguration
        {
            RoleId = roleToPing.Id,
            BotRoleId = botRole.Id,
            ChannelId = channel.Id,
            CreationTime = DateTime.Now,
            EmojiToWatch = emoji,
            JoinGame = isJoinGame
        };

        await _rumbleConfigurationService.AddConfiguration(conf);
        _cacheManager.AddOrUpdate(conf);

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
            prop.Content =
                $"Configuration added. Will watch {channel.Name} and bot role {botRole.Name} and emoji {emoji} and will ping {roleToPing.Name}");
    }

    public async Task Subscribe(SocketInteractionContext context)
    {
        var rumbleConfiguration = _cacheManager.GetRumbleConfiguration();
        if (rumbleConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Rumble Subscription is not set up yet!");
            return;
        }

        var serverId = ulong.Parse(_configuration["ServerId"]!);

        var socketGuild = _client.GetGuild(serverId);
        var socketRole = socketGuild.GetRole(rumbleConfiguration.RoleId);

        var socketGuildUser = (SocketGuildUser)context.User;

        if (socketGuildUser.Roles.Contains(socketRole))
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "You are already subscribed");
            return;
        }

        await socketGuildUser.AddRoleAsync(socketRole);
        await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "You are now subscribed");
    }

    public async Task Unsubscribe(SocketInteractionContext context)
    {
        var rumbleConfiguration = _cacheManager.GetRumbleConfiguration();
        if (rumbleConfiguration == null)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "Rumble Subscription is not set up yet!");
            return;
        }

        var serverId = ulong.Parse(_configuration["ServerId"]!);

        var socketGuild = _client.GetGuild(serverId);
        var socketRole = socketGuild.GetRole(rumbleConfiguration.RoleId);

        var socketGuildUser = (SocketGuildUser)context.User;

        if (!socketGuildUser.Roles.Contains(socketRole))
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "You are not subscribed in first place");
            return;
        }

        await socketGuildUser.RemoveRoleAsync(socketRole);
        await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "You are now unsubscribed");
    }
}