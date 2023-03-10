using System.Runtime.CompilerServices;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UtilityBot.Services.Uno.UnoGameDomain.GameObjects;

public class UnoGame
{

    public Queue<Card>? Deck { get; set; }
    public Stack<Card>? DiscardPile { get; set; }

    public ulong ChannelId { get; }
    public Player Host { get; }
    public Guid Id { get; }
    public bool HasStarted { get; private set; }

    private EColor _lastCardColor = EColor.None;

    private readonly IList<Player> _players = new List<Player>();
    private SocketInteraction _socketInteraction;

    private int _playerIndex = 0;
    private int _turn = 0;
    private bool _isReversed = false;
    private readonly Random _random = new();

    private bool _isGameOver;
    public bool IsGameOver => _isGameOver;
    private Player? _winner;

    public int NumberOfPlayers => _players.Count;

    public UnoGame(ulong channelId, Player host, SocketInteraction socketInteraction)
    {
        _socketInteraction = socketInteraction;
        ChannelId = channelId;
        Host = host;
        _players.Add(host);
        Id = Guid.NewGuid();
    }

    private void ShuffleDeck()
    {
        Deck ??= new Queue<Card>();

        var cards = OrganizedDeck.GetDeck();
        var shuffledCards = cards.OrderBy(x => Guid.NewGuid()).ToList();
        foreach (var card in shuffledCards)
        {
            Deck.Enqueue(card);
        }
    }

    private void DistributeCards()
    {
        for (int i = 0; i < 7; i++)
        {
            foreach (var player in _players)
            {
                player.AddCardToHand(Deck!.Dequeue());
            }
        }
    }

    public string ListPlayers(bool highlightCurrent = false, bool listCardCount = true)
    {
        var result = new StringBuilder();

        foreach (var player in _players)
            result.AppendLine($"{(player == Host ? "👑" : "👤")} {player.SocketUser.Mention} {(listCardCount ? ((player.Hand!.Count == 1 ? "**UNO!**" : $"- {player.Hand.Count} cards")) : "")}");

        if (highlightCurrent)
            result.Replace(_players[_playerIndex].SocketUser.Mention, $"{_players[_playerIndex].SocketUser.Mention} {(_isReversed ? "⬆️" : "⬇️")}");

        return result.ToString();
    }

    public bool IsPlayerExist(ulong userId)
    {
        return _players.Any(x => x.SocketUser.Id == userId);
    }

    public async Task ShowPlayerCards(ulong userId, SocketMessageComponent component)
    {
        var player = _players.Single(x => x.SocketUser.Id == userId);
        player.CardMenuMessage = component;
        await player.UpdateCardMenu(component, DiscardPile!.Peek(), _lastCardColor, _players[_playerIndex] == player);
    }

    public void AddPlayer(SocketUser user)
    {
        _players.Add(new Player
        {
            SocketUser = user
        });
    }

    public async Task StartGame(SocketInteractionContext context)
    {
        HasStarted = true;
        ShuffleDeck();
        DistributeCards();
        var firstCard = DrawFirstCard();
        _playerIndex = _random.Next(0, NumberOfPlayers);
        _socketInteraction = context.Interaction;
        await DoTurn(false);
    }

    private async Task DoTurn(bool isCardPlayed = true)
    {
        _turn++;
        await CheckForWinner();

        if (_isGameOver)
        {
            return;
        }

        var lastPlayer = _players[_playerIndex];
        var playedCard = DiscardPile!.Peek();

        StringBuilder sb = new StringBuilder();

        if (isCardPlayed)
        {
            if (playedCard.Special == ESpecial.Wild || playedCard.Special == ESpecial.WildPlusFour)
            {
                sb.AppendLine($"{lastPlayer.SocketUser.Mention} played {playedCard} {_lastCardColor}");
            }
            else
            {
                sb.AppendLine($"{lastPlayer.SocketUser.Mention} played {playedCard}");
            }
        }
        else
        {
            sb.AppendLine($"Top card is {playedCard}");
        }

        Player? currentPlayer = null;
        Player? nextPlayer = null;

        if (playedCard.Special == ESpecial.Skip)
        {
            if (isCardPlayed)
            {
                NextPlayerIndex();
                currentPlayer = _players[_playerIndex];
                sb.AppendLine($"{currentPlayer.SocketUser.Mention} had to skip his turn!");
                NextPlayerIndex();
                nextPlayer = _players[_playerIndex];
            }
            else
            {
                NextPlayerIndex();
                currentPlayer = _players[_playerIndex];
                sb.AppendLine($"{lastPlayer.SocketUser.Mention} had to skip his turn!");
            }
        }

        else if (playedCard.Special == ESpecial.Reverse)
        {
            if (!isCardPlayed)
            {
                currentPlayer = lastPlayer;
            }
            else
            {
                if (NumberOfPlayers == 2)
                {
                    NextPlayerIndex();
                    currentPlayer = _players[_playerIndex];
                    sb.AppendLine($"{currentPlayer.SocketUser.Mention} had to skip his turn!");
                    NextPlayerIndex();
                    nextPlayer = _players[_playerIndex];
                }
                else
                {
                    _isReversed = !_isReversed;
                    NextPlayerIndex();
                    currentPlayer = _players[_playerIndex];
                }
            }
        }

        else if (playedCard.Special == ESpecial.DrawTwo)
        {
            if (!isCardPlayed)
            {
                sb.AppendLine($"{lastPlayer.SocketUser.Mention} had to draw two cards and skip his turn!");
                DrawCardsForPlayer(lastPlayer, 2);
                NextPlayerIndex();
                currentPlayer = _players[_playerIndex];
            }
            else
            {
                NextPlayerIndex();
                currentPlayer = _players[_playerIndex];
                sb.AppendLine($"{currentPlayer.SocketUser.Mention} had to draw two cards and skip his turn!");
                DrawCardsForPlayer(currentPlayer, 2);
                NextPlayerIndex();
                nextPlayer = _players[_playerIndex];
            }
        }

        else if (playedCard.Special == ESpecial.Wild)
        {
            if (!isCardPlayed)
            {
                _lastCardColor = EColor.None;
                currentPlayer = lastPlayer;
            }
            else
            {
                NextPlayerIndex();
                currentPlayer = _players[_playerIndex];
            }
        }

        else if (playedCard.Special == ESpecial.WildPlusFour)
        {
            NextPlayerIndex();
            currentPlayer = _players[_playerIndex];
            sb.AppendLine($"{currentPlayer.SocketUser.Mention} had to draw FOUR cards and skip his turn!");
            DrawCardsForPlayer(currentPlayer, 4);
            NextPlayerIndex();
            nextPlayer = _players[_playerIndex];
        }

        else
        {
            if (_turn != 1)
            {
                NextPlayerIndex();
                currentPlayer = _players[_playerIndex];
                nextPlayer = currentPlayer;
            }
            else
            {
                currentPlayer = lastPlayer;
            }
        }

        if (currentPlayer != nextPlayer)
        {
            if (nextPlayer != null)
            {
                await currentPlayer.UpdateCardMenu(null, DiscardPile!.Peek(), _lastCardColor, false);
                await nextPlayer.UpdateCardMenu(null, DiscardPile!.Peek(), _lastCardColor, true);
            }
            else
            {
                await currentPlayer.UpdateCardMenu(null, DiscardPile!.Peek(), _lastCardColor, true);
                nextPlayer = currentPlayer;
            }
        }
        else
        {
            await currentPlayer.UpdateCardMenu(null, DiscardPile!.Peek(), _lastCardColor, true);
        }

        var initialCommand = (SocketMessageComponent)_socketInteraction;

        if (_turn == 1)
        {
            await initialCommand.UpdateAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                    .WithColor(DiscardPile!.Peek().GetDiscordColor())
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithName($"{nextPlayer!.SocketUser.Username}'s Turn - Round #{_turn}")
                        .WithIconUrl(nextPlayer.SocketUser.GetAvatarUrl() ?? nextPlayer.SocketUser.GetDefaultAvatarUrl()))
                    .WithDescription($"{sb}")
                    .WithThumbnailUrl(DiscardPile!.Peek().GetImageUrl())
                    .WithFields(new EmbedFieldBuilder()
                    {
                        Name = $"Players {(_isReversed ? "🔃" : "")}",
                        Value = ListPlayers(true),
                    })
                    .Build();

                m.Components = new ComponentBuilder()
                    .WithButton("View Cards", $"show-card-prompt", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("Leave Game", $"leave-during-game", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("End Game", $"end-during-game", row: 0, style: ButtonStyle.Secondary)
                    .Build();
            });
        }
        else
        {
            await initialCommand.Message.ModifyAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                    .WithColor(DiscardPile!.Peek().GetDiscordColor())
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithName($"{nextPlayer!.SocketUser.Username}'s Turn - Round #{_turn}")
                        .WithIconUrl(nextPlayer.SocketUser.GetAvatarUrl() ?? nextPlayer.SocketUser.GetDefaultAvatarUrl()))
                    .WithDescription($"{sb}")
                    .WithThumbnailUrl(DiscardPile!.Peek().GetImageUrl())
                    .WithFields(new EmbedFieldBuilder()
                    {
                        Name = $"Players {(_isReversed ? "🔃" : "")}",
                        Value = ListPlayers(true),
                    })
                    .Build();

                m.Components = new ComponentBuilder()
                    .WithButton("View Cards", $"show-card-prompt", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("Leave Game", $"leave-during-game", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("End Game", $"end-during-game", row: 0, style: ButtonStyle.Secondary)
                    .Build();
            });
        }
    }

    private async Task CheckForWinner()
    {
        if (NumberOfPlayers == 1)
        {
            _winner = _players[0];
            _isGameOver = true;
        }

        else if (_players.Any(x => x.Hand?.Count == 0))
        {
            _winner = _players.Single(x => x.Hand?.Count == 0);
            _isGameOver = true;
        }

        if (_isGameOver)
        {
            var initialCommand = (SocketMessageComponent)_socketInteraction;
            await initialCommand.Message.ModifyAsync(m =>
            {
                m.Content = "The game is over, I hope you had fun 😊";

                m.Embed = new EmbedBuilder()
                    .WithColor(DiscardPile!.Peek().GetDiscordColor())
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithName(_winner!.SocketUser.Username)
                        .WithIconUrl(_winner!.SocketUser.GetAvatarUrl() ?? _winner!.SocketUser.GetDefaultAvatarUrl()))
                    .WithDescription($"{_winner!.SocketUser.Username} has won after {_turn} rounds!")
                    .WithThumbnailUrl(DiscardPile!.Peek().GetImageUrl())
                    .Build();

                m.Components = null;
            });

            foreach (var player in _players)
                await player.RemoveAllPlayerCardMenusWithMessage("The game is over, I hope you had fun 😊");
        }
    }

    private Card DrawFirstCard()
    {
        DiscardPile ??= new Stack<Card>();
        var firstCard = Deck!.Dequeue();

        while (firstCard.Special == ESpecial.WildPlusFour)
        {
            Deck.Enqueue(firstCard);
            firstCard = Deck.Dequeue();
        }

        DiscardPile.Push(firstCard);
        return firstCard;
    }

    private void NextPlayerIndex()
    {
        if (_isReversed)
        {
            _playerIndex--;
            if (_playerIndex < 0)
                _playerIndex = NumberOfPlayers - 1;
        }
        else
        {
            _playerIndex++;
            if (_playerIndex >= NumberOfPlayers)
                _playerIndex = 0;
        }
    }

    private void DrawCardsForPlayer(Player player, int numberOfCards)
    {
        //First we need to check if there's another cards in the deck
        if (Deck!.Count <= numberOfCards)
        {
            var tempCard = DiscardPile!.Pop();

            List<Card> discardedCards = new List<Card>();
            while (DiscardPile!.Count > 0)
            {
                discardedCards.Add(DiscardPile!.Pop());
            }

            DiscardPile!.Push(tempCard);

            discardedCards = discardedCards.OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var discardedCard in discardedCards)
            {
                Deck!.Enqueue(discardedCard);
            }
        }

        for (int i = 0; i < numberOfCards; i++)
        {
            player.AddCardToHand(Deck!.Dequeue());
        }
    }

    public async Task PlayCard(SocketInteractionContext context, Guid cardId, SocketMessageComponent? messageComponent = null)
    {
        var currentPlayer = _players[_playerIndex];
        var potentialPlayer = _players.SingleOrDefault(x => x.SocketUser.Id == context.User.Id);

        if (potentialPlayer == null)
        {
            await context.Interaction.RespondAsync($"Player Not Found!", ephemeral: true);
            return;
        }

        if (currentPlayer.SocketUser.Id != context.User.Id)
        {
            await context.Interaction.RespondAsync($"Not your turn!", ephemeral: true);
            return;
        }

        if (potentialPlayer.Hand!.All(x => x.UniqueId != cardId))
        {
            await context.Interaction.RespondAsync($"Not your card!", ephemeral: true);
            return;
        }

        var card = potentialPlayer.Hand!.Single(x => x.UniqueId == cardId);

        var canBePlayed = potentialPlayer.CheckIfCardCanBePlayed(card, DiscardPile!.Peek(), _lastCardColor);
        if (!canBePlayed)
        {
            await context.Interaction.RespondAsync($"Can't play that card!", ephemeral: true);
            return;
        }

        if (card.Special == ESpecial.Wild || card.Special == ESpecial.WildPlusFour)
        {
            var command = (SocketMessageComponent)context.Interaction;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Select a color for this Wild card. For info, you have:");
            sb.AppendLine($"{currentPlayer.Hand!.Count(x => x.Color == EColor.Red)} red cards");
            sb.AppendLine($"{currentPlayer.Hand!.Count(x => x.Color == EColor.Green)} green cards");
            sb.AppendLine($"{currentPlayer.Hand!.Count(x => x.Color == EColor.Blue)} blue cards");
            sb.AppendLine($"{currentPlayer.Hand!.Count(x => x.Color == EColor.Yellow)} yellow cards");

            if (messageComponent != null)
            {
                command = messageComponent;
                await command.ModifyOriginalResponseAsync(m =>
                {
                    m.Embed = new EmbedBuilder()
                        .WithColor(Colors.Green)
                        .WithDescription($"{sb}")
                        .Build();

                    m.Components = new ComponentBuilder()
                        .WithButton("Red", $"wild_Red_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟥"))
                        .WithButton("Green", $"wild_Green_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟩"))
                        .WithButton("Blue", $"wild_Blue_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟦"))
                        .WithButton("Yellow", $"wild_Yellow_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟨"))
                        .WithButton("Cancel", "cancel-wild", style: ButtonStyle.Secondary)
                        .Build();
                });
                return;
            }
            
            await command.UpdateAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                    .WithColor(Colors.Green)
                    .WithDescription($"{sb}")
                    .Build();

                m.Components = new ComponentBuilder()
                    .WithButton("Red", $"wild_Red_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟥"))
                    .WithButton("Green", $"wild_Green_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟩"))
                    .WithButton("Blue", $"wild_Blue_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟦"))
                    .WithButton("Yellow", $"wild_Yellow_{cardId}", style: ButtonStyle.Secondary, new Emoji("🟨"))
                    .WithButton("Cancel", "cancel-wild", style: ButtonStyle.Secondary)
                    .Build();
            });
            return;
        }

        potentialPlayer.Hand!.Remove(card);
        DiscardPile!.Push(card);
        _lastCardColor = card.Color;
        await DoTurn();

        if (!_isGameOver)
        {
            await potentialPlayer.UpdateCardMenu((SocketMessageComponent)context.Interaction, card, _lastCardColor,
                _players[_playerIndex].SocketUser.Id == context.User.Id, $"You played a {card}");
        }
    }

    public async Task DrawCard(SocketInteractionContext context)
    {
        var currentPlayer = _players[_playerIndex];
        var potentialPlayer = _players.SingleOrDefault(x => x.SocketUser.Id == context.User.Id);

        if (potentialPlayer == null)
        {
            await context.Interaction.RespondAsync($"Player Not Found!", ephemeral: true);
            return;
        }

        if (currentPlayer.SocketUser.Id != context.User.Id)
        {
            await context.Interaction.RespondAsync($"Not your turn!", ephemeral: true);
            return;
        }

        DrawCardsForPlayer(potentialPlayer, 1);


        var drawnCard = potentialPlayer.Hand!.Last();
        if (potentialPlayer.CheckIfCardCanBePlayed(drawnCard, DiscardPile!.Peek(), _lastCardColor))
        {
            if (drawnCard.Special != ESpecial.Wild && drawnCard.Special != ESpecial.WildPlusFour)
            {
                await potentialPlayer.UpdateCardMenu((SocketMessageComponent)context.Interaction, DiscardPile!.Peek(),
                    _lastCardColor, false, $"You drew a {drawnCard} and played it!");
                await PlayCard(context, drawnCard.UniqueId);
                return;
            }

            await potentialPlayer.UpdateCardMenu((SocketMessageComponent)context.Interaction, DiscardPile!.Peek(),
                _lastCardColor, false, $"You drew a {drawnCard} card! Choose your color!");
            await PlayCard(context, drawnCard.UniqueId, (SocketMessageComponent)context.Interaction);
            return;
        }

        await potentialPlayer.UpdateCardMenu((SocketMessageComponent)context.Interaction, DiscardPile!.Peek(),
            _lastCardColor, false, $"You drew a {drawnCard} but it can't be played");

        NextPlayerIndex();
        var nextPlayer = _players[_playerIndex];
        await nextPlayer.UpdateCardMenu(null, DiscardPile!.Peek(), _lastCardColor, true);

        _turn++;

        var initialCommand = (SocketMessageComponent)_socketInteraction;


        if (_turn == 1)
        {
            await initialCommand.UpdateAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                    .WithColor(DiscardPile!.Peek().GetDiscordColor())
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithName($"{nextPlayer!.SocketUser.Username}'s Turn - Round #{_turn}")
                        .WithIconUrl(nextPlayer.SocketUser.GetAvatarUrl() ?? nextPlayer.SocketUser.GetDefaultAvatarUrl()))
                    .WithDescription($"{potentialPlayer.SocketUser.Mention} drew 1 card and lost his turn!{Environment.NewLine}Top card is {DiscardPile!.Peek()}")
                    .WithThumbnailUrl(DiscardPile!.Peek().GetImageUrl())
                    .WithFields(new EmbedFieldBuilder()
                    {
                        Name = $"Players {(_isReversed ? "🔃" : "")}",
                        Value = ListPlayers(true),
                    })
                    .Build();

                m.Components = new ComponentBuilder()
                    .WithButton("View Cards", $"show-card-prompt", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("Leave Game", $"leave-during-game", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("End Game", $"end-during-game", row: 0, style: ButtonStyle.Secondary)
                    .Build();
            });
        }
        else
        {
            await initialCommand.Message.ModifyAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                    .WithColor(DiscardPile!.Peek().GetDiscordColor())
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithName($"{nextPlayer!.SocketUser.Username}'s Turn - Round #{_turn}")
                        .WithIconUrl(nextPlayer.SocketUser.GetAvatarUrl() ?? nextPlayer.SocketUser.GetDefaultAvatarUrl()))
                    .WithDescription($"{potentialPlayer.SocketUser.Mention} drew 1 card and lost his turn!{Environment.NewLine}Top card is {DiscardPile!.Peek()}")
                    .WithThumbnailUrl(DiscardPile!.Peek().GetImageUrl())
                    .WithFields(new EmbedFieldBuilder()
                    {
                        Name = $"Players {(_isReversed ? "🔃" : "")}",
                        Value = ListPlayers(true),
                    })
                    .Build();

                m.Components = new ComponentBuilder()
                    .WithButton("View Cards", $"show-card-prompt", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("Leave Game", $"leave-during-game", row: 0, style: ButtonStyle.Secondary)
                    .WithButton("End Game", $"end-during-game", row: 0, style: ButtonStyle.Secondary)
                    .Build();
            });
        }
    }

    public async Task PlayWildCard(SocketInteractionContext context, Guid cardId, EColor color)
    {
        var currentPlayer = _players[_playerIndex];
        var potentialPlayer = _players.SingleOrDefault(x => x.SocketUser.Id == context.User.Id);

        if (potentialPlayer == null)
        {
            await context.Interaction.RespondAsync($"Player Not Found!", ephemeral: true);
            return;
        }

        if (currentPlayer.SocketUser.Id != context.User.Id)
        {
            await context.Interaction.RespondAsync($"Not your turn!", ephemeral: true);
            return;
        }

        if (potentialPlayer.Hand!.All(x => x.UniqueId != cardId))
        {
            await context.Interaction.RespondAsync($"Not your card!", ephemeral: true);
            return;
        }

        var card = potentialPlayer.Hand!.Single(x => x.UniqueId == cardId);

        var canBePlayed = potentialPlayer.CheckIfCardCanBePlayed(card, DiscardPile!.Peek(), _lastCardColor);
        if (!canBePlayed)
        {
            await context.Interaction.RespondAsync($"Can't play that card!", ephemeral: true);
            return;
        }

        if (card.Special != ESpecial.Wild && card.Special != ESpecial.WildPlusFour)
        {
            await context.Interaction.RespondAsync($"Can't play that card! It's not wild!", ephemeral: true);
            return;
        }

        potentialPlayer.Hand!.Remove(card);
        DiscardPile!.Push(card);
        _lastCardColor = color;
        await DoTurn();

        if (!_isGameOver)
        {
            await potentialPlayer.UpdateCardMenu((SocketMessageComponent)context.Interaction, card, _lastCardColor,
                _players[_playerIndex].SocketUser.Id == context.User.Id, $"You played a {card} {color}");
        }
    }
}