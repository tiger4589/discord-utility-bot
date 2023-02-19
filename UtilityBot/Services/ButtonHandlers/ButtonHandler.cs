﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.ButtonHandlers;

public class ButtonHandler : IButtonHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ICacheManager _cacheManager;
    private readonly IConfiguration _configuration;
    private readonly ILoggingService _loggingService;

    public ButtonHandler(DiscordSocketClient client, ICacheManager cacheManager, IConfiguration configuration, ILoggingService loggingService)
    {
        _client = client;
        _cacheManager = cacheManager;
        _configuration = configuration;
        _loggingService = loggingService;
        _client.ButtonExecuted += ClientOnButtonExecuted;
    }

    private async Task ClientOnButtonExecuted(SocketMessageComponent arg)
    {
        var customId = arg.Data.CustomId;
        if (customId == null)
        {
            await _loggingService.Log("A Button was clicked, but it doesn't contain a custom id.. really weird.");
            return;
        }

        var verifyConfiguration = _cacheManager.GetVerifyConfiguration();

        var onJoinRoleConfiguration = _cacheManager.GetGuildOnJoinConfiguration(ulong.Parse(_configuration["ServerId"]!));

        var data = customId.Split('_');
        if (data[0] == "verify")
        {
            if (verifyConfiguration == null)
            {
                await _loggingService.Log("Verification Configuration is null.. pretty weird.. eh");
                return;
            }
            var userId = ulong.Parse(data[1]);

            var guild = _client.GetGuild(ulong.Parse(_configuration["ServerId"]!));
            var user =  guild.GetUser(userId) as SocketGuildUser;
            if (user == null)
            {
                await _loggingService.Log($"Couldn't find the user with id {userId} upon verification");
                return;
            }

            await user.AddRoleAsync(verifyConfiguration.RoleId);

            if (onJoinRoleConfiguration != null)
            {
                var roles = onJoinRoleConfiguration.UserJoinRoles.Select(x => x.RoleId).ToList();
                if (roles.Any())
                {
                    await user.RemoveRolesAsync(roles);
                }
            }
            
            await arg.UpdateAsync(prop =>
            {
                prop.Content = $"{arg.User.Mention} verified {user.Username}";
            });

            await arg.Message.ModifyAsync(o => { o.Components = new ComponentBuilder().Build(); });

            return;
        }

        if (data[0] == "reject")
        {
            await arg.UpdateAsync(prop =>
            {
                prop.Content = $"{arg.User.Mention} rejected the request!";
            });

            await arg.Message.ModifyAsync(o => { o.Components = new ComponentBuilder().Build(); });
        }
    }
}