using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HtmlAgilityPack;
using UtilityBot.EventArguments;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.LoggingServices;
using UtilityBot.Services.MessageHandlers;

namespace UtilityBot.Services.PlayerServices;

public class PlayerService : IPlayerService
{
    private readonly ICacheManager _cacheManager;
    private readonly IEmbedMessageBuilder _embedMessageBuilder;

    public PlayerService(ICacheManager cacheManager, IEmbedMessageBuilder embedMessageBuilder)
    {
        _cacheManager = cacheManager;
        _embedMessageBuilder = embedMessageBuilder;
    }

    public async Task RequestVerification(SocketInteractionContext context, string recruitLink)
    {
        var conf = _cacheManager.GetVerifyConfiguration();
        if (conf == null)
        {
            await context.Interaction.RespondAsync("Verify Configuration is not currently set... Notify Moderators!", ephemeral:true);
            return;
        }

        var socketGuildUser = context.User as SocketGuildUser;
        if (socketGuildUser!.Roles.Any(x => x.Id == conf.RoleId))
        {
            await context.Interaction.RespondAsync("You are already verified!", ephemeral: true);
            return;
        }

        if (recruitLink.Split(' ').Length > 1 || !recruitLink.StartsWith("https://www.kingsofchaos.com/recruit.php?uniqid="))
        {
            await context.Interaction.RespondAsync("Invalid Recruit Link! Please use yours from your command center, if you think this is wrong, message a moderator", ephemeral: true);
            return;
        }

        var uniqueHash = recruitLink.Replace("https://www.kingsofchaos.com/recruit.php?uniqid=", "");
        if (uniqueHash.Length != 8)
        {
            await context.Interaction.RespondAsync("Invalid Recruit Link! Please use yours from your command center, if you think this is wrong, message a moderator", ephemeral: true);
            return;
        }

        var channel = context.Guild.GetChannel(conf.ChannelId) as ITextChannel;
        if (channel == null)
        {
            await Logger.Log($"Can't find the channel to send verification request!");
            return;
        }

        var inGameUsername = await GetPlayerInGameName(recruitLink);

        var embed  = _embedMessageBuilder.BuildVerificationEmbed(context.User, recruitLink, inGameUsername);

        await channel.SendMessageAsync(
            embed: embed,   
            components: new ComponentBuilder()
                .WithButton("Verify", $"verify_{context.User.Id}", ButtonStyle.Success, new Emoji("\u2705"))
                .WithButton("Reject", $"reject_{context.User.Id}", ButtonStyle.Danger, new Emoji("\u26D4"))
                .Build());

        await context.Interaction.RespondAsync("Moderators have been notified, please be patient.", ephemeral: true);
    }

    public async Task RequestCoderStatus(SocketInteractionContext context, string recruitLink, string allianceGameLink, string role)
    {
        var conf = _cacheManager.GetCoderRequestVerification();
        if (conf == null)
        {
            await context.Interaction.RespondAsync("Coders Configuration is not currently set... Notify Moderators!", ephemeral: true);
            return;
        }

        var socketGuildUser = context.User as SocketGuildUser;
        if (socketGuildUser!.Roles.Any(x => x.Id == conf.RoleId))
        {
            await context.Interaction.RespondAsync("You already have the coders role!", ephemeral: true);
            return;
        }

        if (recruitLink.Split(' ').Length > 1 || !recruitLink.StartsWith("https://www.kingsofchaos.com/recruit.php?uniqid="))
        {
            await context.Interaction.RespondAsync("Invalid Recruit Link! Please use yours from your command center, if you think this is wrong, message a moderator", ephemeral: true);
            return;
        }

        var uniqueHash = recruitLink.Replace("https://www.kingsofchaos.com/recruit.php?uniqid=", "");
        if (uniqueHash.Length != 8)
        {
            await context.Interaction.RespondAsync("Invalid Recruit Link! Please use yours from your command center, if you think this is wrong, message a moderator", ephemeral: true);
            return;
        }

        var channel = context.Guild.GetChannel(conf.ChannelId) as ITextChannel;
        if (channel == null)
        {
            await Logger.Log($"Can't find the channel to send a coder request!");
            return;
        }

        var embed = _embedMessageBuilder.BuildCoderRequestVerification(context.User, recruitLink, allianceGameLink, role);

        await channel.SendMessageAsync(
            embed: embed,
            components: new ComponentBuilder()
                .WithButton("Verify", $"coder_verify_{context.User.Id}", ButtonStyle.Success, new Emoji("\u2705"))
                .WithButton("Reject", $"coder_reject_{context.User.Id}", ButtonStyle.Danger, new Emoji("\u26D4"))
                .Build());

        await context.Interaction.RespondAsync("Moderators have been notified, please be patient.", ephemeral: true);
    }

    public async Task ResetRole(SocketInteractionContext context, ulong userId, ulong roleId)
    {
        var myRoles = context.Guild.CurrentUser.Roles;
        var socketRole = context.Guild.GetRole(roleId);

        if (myRoles.All(x => x.Position <= socketRole.Position))
        {
            await context.Interaction.RespondAsync($"I am not allowed to remove or add {socketRole.Mention} role", ephemeral: true);
            return;
        }

        var user = context.Guild.Users.SingleOrDefault(x=>x.Id == userId);

        if (user == null)
        {
            await context.Interaction.RespondAsync($"can't find that user!", ephemeral: true);
            return;
        }

        var role = user.Roles.SingleOrDefault(x => x.Id == roleId);

        if (role == null)
        {
            await context.Interaction.RespondAsync($"{user.Mention} doesn't have {socketRole.Mention} role", ephemeral: true);
            return;
        }

        await user.RemoveRoleAsync(roleId);
        await user.AddRoleAsync(roleId);

        await context.Interaction.RespondAsync($"{socketRole.Mention} role removed and added for {user.Mention}", ephemeral: true);
    }

    private async Task<string> GetPlayerInGameName(string recruitLink)
    {
        using var client = new HttpClient();
        var htmlPage = await client.GetStringAsync(recruitLink);

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlPage);

        var userNameNodes = 
            htmlDoc.DocumentNode.Descendants("h2")
                .Where(node => node.InnerText.Contains("needs your help becoming"))
            .ToList();

        var userName = userNameNodes.First().InnerText.Replace("Your friend ", "").Replace(" needs your help becoming a King of Chaos!CHOOSE A RACE AND JOIN THE WAR NOW!", "");

        return userName;
    }
}