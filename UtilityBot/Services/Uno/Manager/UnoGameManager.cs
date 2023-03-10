using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.ComponentModel;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;
using UtilityBot.Services.Uno.UnoGameDomain.GameObjects;

namespace UtilityBot.Services.Uno.Manager;

public class UnoGameManager : IUnoGameManager
{
    private readonly ICacheManager _cacheManager;

    private readonly IList<UnoGame> _runningGames = new List<UnoGame>();

    public UnoGameManager(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task InitializeGame(SocketInteractionContext context)
    {
        var allUnoConfigurations = _cacheManager.GetAllUnoConfigurations();

        if (!allUnoConfigurations.Any())
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = "UNO Configurations is not yet set by moderators, please be patient.");
            return;
        }

        if (!allUnoConfigurations.Contains(context.Channel.Id))
        {
            var channels = new List<string>();
            foreach (var allUnoConfiguration in allUnoConfigurations)
            {
                channels.Add(context.Guild.GetChannel(allUnoConfiguration).Name);
            }

            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"UNO can only be started in one of the following channel(s): {string.Join(',', channels)}");
            return;
        }

        if (_runningGames.Any(x=>x.ChannelId == context.Channel.Id))
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop =>
                prop.Content = $"A UNO Game is already running in #{context.Channel.Name} - If you think this a mistake, please notify a moderator!");
            return;
        }

        var game = new UnoGame(context.Channel.Id, new Player
        {
            SocketUser = context.User
        }, context.Interaction);

        _runningGames.Add(game);

        await context.Interaction.FollowupAsync("Started a new game", embed: new EmbedBuilder()
                .WithColor(Colors.Red)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName("UNO"))
                .WithDescription($"{context.User.Username} has started a game of UNO! Click the button below to join!\n\nCurrent Players:\n{game.ListPlayers(listCardCount: false)}")
                .Build(),
            components: new ComponentBuilder()
                .WithButton("Start Game", $"start-uno_{game.Id}", row: 0, style: ButtonStyle.Secondary, disabled: true)
                .WithButton("Cancel Game", $"cancel-uno_{game.Id}", row: 0, style: ButtonStyle.Secondary)
                .WithButton("Join Game", $"join-uno_{game.Id}", row: 1, style: ButtonStyle.Secondary)
                .WithButton("Leave Game", $"leave-uno_{game.Id}", row: 1, style: ButtonStyle.Secondary)
                .Build());
    }

    public async Task JoinGame(SocketInteractionContext context, Guid gameId)
    {
        if (_runningGames.All(x => x.Id != gameId))
        {
            await context.Interaction.PrintError("Game does not exist, or already finished.");
            return;
        }

        var game = _runningGames.Single(x => x.Id == gameId);
        if (game.HasStarted)
        {
            await context.Interaction.PrintError("Game has already started");
            return;
        }

        if (game.IsPlayerExist(context.User.Id))
        {
            await context.Interaction.PrintError("You already joined the game.. What's wrong with you?");
            return;
        }

        if (game.NumberOfPlayers == UnoGameConfiguration.MaximumPlayers)
        {
            await context.Interaction.PrintError("Game is full, sorry, try to start a new one in another channel!");
            return;
        }

        game.AddPlayer(context.User);

        var command = (SocketMessageComponent)context.Interaction;

        await command.UpdateAsync(m =>
        {
            m.Embed = new EmbedBuilder()
                .WithColor(Colors.Red)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName("UNO"))
                .WithDescription($"{game.Host.SocketUser.Mention} has started a game of UNO! Click the button below to join!\n\n{game.ListPlayers(listCardCount:false)}\n\n*{command.User.Mention} just joined*")
                .Build();

            m.Components = new ComponentBuilder()
                .WithButton("Start Game", $"start-uno_{game.Id}", row: 0, style: ButtonStyle.Secondary, disabled: game.NumberOfPlayers < UnoGameConfiguration.MinimumPlayers)
                .WithButton("Cancel Game", $"cancel-uno_{game.Id}", row: 0, style: ButtonStyle.Secondary)
                .WithButton("Join Game", $"join-uno_{game.Id}", row: 1, style: ButtonStyle.Secondary, disabled: game.NumberOfPlayers == UnoGameConfiguration.MaximumPlayers)
                .WithButton("Leave Game", $"leave-uno_{game.Id}", row: 1, style: ButtonStyle.Secondary)
                .Build();
        });
    }

    public async Task StartGame(SocketInteractionContext context, Guid gameId)
    {
        if (_runningGames.All(x => x.Id != gameId))
        {
            await context.Interaction.PrintError("Game does not exist, or already finished.");
            return;
        }

        var game = _runningGames.Single(x => x.Id == gameId);
        if (game.HasStarted)
        {
            await context.Interaction.PrintError("Game has already started");
            return;
        }

        if (game.Host.SocketUser.Id != context.User.Id)
        {
            await context.Interaction.PrintError("Only the host can start a game!");
            return;
        }

        //await context.Interaction.RespondAsync($"Starting Game", ephemeral: true);
        await game.StartGame(context);
    }

    public async Task ShowCards(SocketMessageComponent component)
    {
        if (_runningGames.All(x => x.ChannelId != component.Channel.Id))
        {
            await component.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == component.Channel.Id);
        if (!game.IsPlayerExist(component.User.Id))
        {
            await component.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }

        if (!game.HasStarted)
        {
            await component.RespondAsync("Game not started yet..", ephemeral: true);
            return;
        }

        await game.ShowPlayerCards(component.User.Id, component);
    }

    public async Task PlayCard(SocketInteractionContext context, Guid cardId)
    {
        if (_runningGames.All(x => x.ChannelId != context.Channel.Id))
        {
            await context.Interaction.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
        if (!game.IsPlayerExist(context.User.Id))
        {
            await context.Interaction.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }

        if (!game.HasStarted)
        {
            await context.Interaction.RespondAsync("Game not started yet..", ephemeral: true);
            return;
        }

        await game.PlayCard(context, cardId);

        if (game.IsGameOver)
        {
            _runningGames.Remove(game);   
        }
    }

    public async Task DrawCard(SocketInteractionContext context)
    {
        if (_runningGames.All(x => x.ChannelId != context.Channel.Id))
        {
            await context.Interaction.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
        if (!game.IsPlayerExist(context.User.Id))
        {
            await context.Interaction.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }

        if (!game.HasStarted)
        {
            await context.Interaction.RespondAsync("Game not started yet..", ephemeral: true);
            return;
        }

        await game.DrawCard(context);
    }

    public async Task PlayWildCard(SocketInteractionContext context, string color, Guid cardId)
    {
        if (_runningGames.All(x => x.ChannelId != context.Channel.Id))
        {
            await context.Interaction.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
        if (!game.IsPlayerExist(context.User.Id))
        {
            await context.Interaction.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }

        if (!game.HasStarted)
        {
            await context.Interaction.RespondAsync("Game not started yet..", ephemeral: true);
            return;
        }

        var colorEnum = (EColor)Enum.Parse(typeof(EColor), color);
        await game.PlayWildCard(context, cardId, colorEnum);

        if (game.IsGameOver)
        {
            _runningGames.Remove(game);
        }
    }
}