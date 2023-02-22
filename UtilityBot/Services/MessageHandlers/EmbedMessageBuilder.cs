using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Services.JokesServices;

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

    public Embed BuildChuckNorrisFactEmbed(SocketInteractionContext context, ChuckNorrisJoke joke)
    {
        var builder = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                IconUrl = "https://api.chucknorris.io/img/chucknorris_logo_coloured_small@2x.png",
                Name = "Chuck Norris",
                Url = joke.Url
            }
        };

        builder.WithThumbnailUrl("https://api.chucknorris.io/img/chucknorris_logo_coloured_small@2x.png");

        builder.WithFields(new EmbedFieldBuilder().WithName("Requested By").WithValue(context.User.Mention));
        builder.WithFields(new EmbedFieldBuilder().WithName("Fact").WithValue(joke.Value));

        if (joke.Categories.Any())
        {
            builder.WithFields(new EmbedFieldBuilder().WithName("Categories")
                .WithValue(string.Join(',', joke.Categories)));
        }

        return builder.Build();
    }

    private EmbedBuilder CreateBuilder()
    {
        EmbedBuilder builder = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder()
            {
                Name = "DUB",
                Url = "https://kingsofchaos.com"
            }
        };

        builder.WithThumbnailUrl("https://www.kingsofchaos.com/images/koc_shield.png");

        return builder;
    }
}