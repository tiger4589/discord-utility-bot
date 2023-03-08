using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.MessageHandlers;

namespace UtilityBot.Services.ButtonHandlers;

public class ButtonHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IConfiguration _configuration;
    private readonly IMessageHandler _messageHandler;

    public ButtonHandler(DiscordSocketClient client, ICacheManager cacheManager, IConfiguration configuration, IMessageHandler messageHandler)
    {
        _client = client;
        _cacheManager = cacheManager;
        _configuration = configuration;
        _messageHandler = messageHandler;
    }

    [ComponentInteraction("verify_*")]
    public async Task Verify(ulong userId)
    {
        var verifyConfiguration = _cacheManager.GetVerifyConfiguration();
        var onJoinRoleConfiguration = _cacheManager.GetGuildOnJoinConfiguration(ulong.Parse(_configuration["ServerId"]!));

        if (verifyConfiguration == null)
        {
            await Logger.Log("Verification Configuration is null.. pretty weird.. eh");
            return;
        }

        var guild = _client.GetGuild(ulong.Parse(_configuration["ServerId"]!));
        var user = guild.GetUser(userId);
        if (user == null)
        {
            await Logger.Log($"Couldn't find the user with id {userId} upon verification");
            return;
        }

        await user.AddRoleAsync(verifyConfiguration.RoleId);

        if (!string.IsNullOrWhiteSpace(verifyConfiguration.Message))
        {
            await user.SendMessageAsync(_messageHandler.ReplacePlaceHolders(verifyConfiguration.Message, user));
        }

        if (onJoinRoleConfiguration != null)
        {
            var roles = onJoinRoleConfiguration.UserJoinRoles.Select(x => x.RoleId).ToList();
            if (roles.Any())
            {
                await user.RemoveRolesAsync(roles);
            }
        }

        var interaction = (SocketMessageComponent)Context.Interaction;

        await interaction.UpdateAsync(prop =>
        {
            prop.Content = $"{interaction.User.Mention} verified {user.Username}";
        });

        await interaction.Message.ModifyAsync(o => { o.Components = new ComponentBuilder().Build(); });
    }

    [ComponentInteraction("reject_*")]
    public async Task Reject(ulong userId)
    {
        var interaction = (SocketMessageComponent)Context.Interaction;

        await interaction.UpdateAsync(prop =>
        {
            prop.Content = $"{interaction.User.Mention} rejected the request!";
        });

        await interaction.Message.ModifyAsync(o => { o.Components = new ComponentBuilder().Build(); });
    }

    [ComponentInteraction("coder_reject_*")]
    public async Task RejectCoder(ulong userId)
    {
        var interaction = (SocketMessageComponent)Context.Interaction;

        await interaction.UpdateAsync(prop =>
        {
            prop.Content = $"{interaction.User.Mention} rejected the request!";
        });

        await interaction.Message.ModifyAsync(o => { o.Components = new ComponentBuilder().Build(); });
    }

    [ComponentInteraction("coder_verify_*")]
    public async Task VerifyCoder(ulong userId)
    {
        var verifyConfiguration = _cacheManager.GetCoderRequestVerification();

        if (verifyConfiguration == null)
        {
            await Logger.Log("Verification Configuration is null.. pretty weird.. eh");
            return;
        }

        var guild = _client.GetGuild(ulong.Parse(_configuration["ServerId"]!));
        var user = guild.GetUser(userId);
        if (user == null)
        {
            await Logger.Log($"Couldn't find the user with id {userId} upon verification");
            return;
        }

        await user.AddRoleAsync(verifyConfiguration.RoleId);

        var interaction = (SocketMessageComponent)Context.Interaction;

        await interaction.UpdateAsync(prop =>
        {
            prop.Content = $"{interaction.User.Mention} added {user.Username} to coders role!";
        });

        await interaction.Message.ModifyAsync(o => { o.Components = new ComponentBuilder().Build(); });
    }
}