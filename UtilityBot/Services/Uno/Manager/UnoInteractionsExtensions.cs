using Discord;
using Discord.WebSocket;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;

namespace UtilityBot.Services.Uno.Manager;

public static class UnoInteractionsExtensions
{
    public static async Task PrintError(this SocketInteraction interaction, string message)
        => await interaction.RespondAsync(ephemeral: true, embed: new EmbedBuilder()
            .WithColor(Colors.Red)
            .WithAuthor(new EmbedAuthorBuilder()
                .WithName("Error"))
            .WithDescription(message)
            .Build());
}