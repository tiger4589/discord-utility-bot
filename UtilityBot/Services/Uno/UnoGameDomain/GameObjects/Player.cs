using Discord;
using Discord.WebSocket;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;

namespace UtilityBot.Services.Uno.UnoGameDomain.GameObjects;

public class Player
{
    public SocketUser SocketUser { get; set; } = null!;
    public IList<Card>? Hand { get; set; }
    public SocketMessageComponent? CardMenuMessage { get; set; }

    public void AddCardToHand(Card card)
    {
        Hand ??= new List<Card>();

        Hand.Add(card);
    }

    public async Task RemoveAllPlayerCardMenusWithMessage(string message)
    {
        if (CardMenuMessage == null)
        {
            return;

        }

        await CardMenuMessage.ModifyOriginalResponseAsync(x =>
        {
            x.Content = message;
            x.Embed = null;
            x.Components = new ComponentBuilder().Build();
        });
    }

    public async Task UpdateCardMenu(SocketMessageComponent? command, Card lastPlayedCard, EColor chosenColor, bool isPlayerTurn = false, string extraMessage = "")
    {
        var buttons = new ComponentBuilder();

        var row = 0;
        var count = 0;
        var index = 0;

        foreach (var card in Hand!)
        {
            buttons.WithButton($"{card}", $"card_{card.UniqueId}", style: ButtonStyle.Secondary, row: row, emote: card.GetColorEmoji(), disabled: !isPlayerTurn || !CheckIfCardCanBePlayed(card, lastPlayedCard, chosenColor));

            count++;
            index++;

            if (count == 6)
            {
                count = 0;
                row++;
            }
        }

        // Add the draw card button
        buttons.WithButton("Draw Card", "draw-card", style: ButtonStyle.Secondary, row: row, disabled: !isPlayerTurn);

        if (extraMessage != "")
            extraMessage = $"\n\n**{extraMessage}**";

        if (command == null)
        {
            try
            {
                if (CardMenuMessage == null)
                {
                    return;
                }

                await CardMenuMessage.ModifyOriginalResponseAsync(m =>
                {
                    m.Content = "";

                    m.Embed = new EmbedBuilder()
                        .WithColor(isPlayerTurn ? Colors.Green : Colors.Red)
                        .WithDescription($"You have {Hand!.Count} cards.{extraMessage}{(isPlayerTurn ? "\n\nIt's your turn." : "")}")
                        .Build();

                    m.Components = buttons.Build();
                });
            }
            // Ignore, they probably dismissed the message or something
            catch { }
        }
        else
        {
            CardMenuMessage = command;
            await CardMenuMessage.UpdateAsync(m =>
            {
                m.Content = "";

                m.Embed = new EmbedBuilder()
                    .WithColor(isPlayerTurn ? Colors.Green : Colors.Red)
                    .WithDescription($"You have {Hand!.Count} cards.{extraMessage}{(isPlayerTurn ? "\n\nIt's your turn." : "")}")
                    .Build();

                m.Components = buttons.Build();
            });
        }
    }

    public bool CheckIfCardCanBePlayed(Card card, Card lastPlayedCard, EColor chosenColor, bool returnFalseForWildPlusFour = false)
    {
        if (card.Special == ESpecial.WildPlusFour)
        {
            //make sure that no other cards can be played
            if (returnFalseForWildPlusFour)
            {
                return false;
            }

            return Hand!.All(x => CheckIfCardCanBePlayed(x, lastPlayedCard, chosenColor, true) == false);
        }

        if (card.Special == ESpecial.Wild)
        {
            return true;
        }

        if (card.Special == lastPlayedCard.Special && card.Special != ESpecial.None)
        {
            return true;
        }

        if (lastPlayedCard.Special == ESpecial.Wild && chosenColor == EColor.None)
        {
            return true;
        }

        if ((lastPlayedCard.Special == ESpecial.Wild || lastPlayedCard.Special == ESpecial.WildPlusFour) && chosenColor == card.Color)
        {
            return true;
        }

        if (card.Color == lastPlayedCard.Color)
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(card.Value) && card.Value == lastPlayedCard.Value)
        {
            return true;
        }
        
        return false;
    }
}