using System.Text;
using Discord;
using Discord.WebSocket;

namespace UtilityBot.Services.MessageHandlers;

public class EmbedMessageBuilder : IEmbedMessageBuilder
{
    public Embed BuildVerificationEmbed(SocketUser user, string recruitLink)
    {
        var builder = CreateBuilder();
        builder.WithTitle($"Verification Request for {user.Username}");
        builder.WithColor(Color.DarkBlue);

        var sb = new StringBuilder();
        sb.AppendLine($"{user.Mention} - ({user.Username})");
        sb.AppendLine(recruitLink);

        builder.WithDescription(sb.ToString());
        return builder.Build();
    }

    private EmbedBuilder CreateBuilder()
    {
        EmbedBuilder builder = new EmbedBuilder();

        builder.Author = new EmbedAuthorBuilder()
        {
            Name = "DUB",
            Url = "https://kingsofchaos.com"
        };
        builder.WithThumbnailUrl("https://www.kingsofchaos.com/images/koc_shield.png");

        return builder;
    }
}