using System.Text;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using UtilityBot.Services.CacheService;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;
using UtilityBot.Services.Uno.UnoGameDomain.GameObjects;
using Timer = System.Timers.Timer;

namespace UtilityBot.Services.Uno.Manager;

public class UnoGameManager : IUnoGameManager
{
    private readonly ICacheManager _cacheManager;

    private readonly IList<UnoGame> _runningGames = new List<UnoGame>();

    private Timer? _checkForFinishedGamesTimer;

    private Timer? _timer;
    private DateTimeOffset? _startTime;
    public UnoGameManager(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
        StartCheckingForDoneGames();
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

        if (_runningGames.Any(x => x.ChannelId == context.Channel.Id))
        {
            var unoGame = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
            if (!unoGame.IsGameOver)
            {
                await context.Interaction.ModifyOriginalResponseAsync(prop =>
                    prop.Content = $"A UNO Game is already running in #{context.Channel.Name} - If you think this a mistake, please notify a moderator!");
                return;
            }
        }

        var game = new UnoGame(context.Channel.Id, new Player
        {
            SocketUser = context.User
        }, context.Interaction, context.Client, _cacheManager);

        _runningGames.Add(game);

        _startTime = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(5));

        StringBuilder sb = new StringBuilder();

        ulong? roleIdForChannel = _cacheManager.GetRoleIdForChannel(context.Channel.Id);
        if (roleIdForChannel != null)
        {
            var role = context.Guild.GetRole(roleIdForChannel.Value);
            sb.AppendLine(role.Mention);
        }

        sb.AppendLine("Initialized a new game!");
        var message = await context.Interaction.FollowupAsync(sb.ToString(), allowedMentions: AllowedMentions.All, embed: new EmbedBuilder()
                .WithColor(Colors.Red)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName("UNO"))
                .WithDescription($"{context.User.Username} has initialized a game of UNO! Click the button below to join!\n\nCurrent Players:\n{game.ListPlayers(listCardCount: false)}{Environment.NewLine}Game will end automatically <t:{_startTime.Value.ToUnixTimeSeconds()}:R> if not started.")
                .Build(),
            components: new ComponentBuilder()
                .WithButton("Start Game", $"start-uno_{game.Id}", row: 0, style: ButtonStyle.Secondary, disabled: true)
                .WithButton("Join Game", $"join-uno_{game.Id}", row: 1, style: ButtonStyle.Secondary)
                .WithButton("Leave Game", $"leave-uno_{game.Id}", row: 1, style: ButtonStyle.Secondary)
                .Build());

        game.SaveFollowupMessage(message);

        _timer = new Timer(5*60*1000);
        _timer.Elapsed += async (sender, args) =>
        {
            await EndGameBecauseNotStarted(message, game.Id);
        };

        _timer.AutoReset = false;
        _timer.Enabled = true;
    }

    private void StartCheckingForDoneGames()
    {
        _checkForFinishedGamesTimer = new Timer( 30 * 1000);
        _checkForFinishedGamesTimer.Elapsed += (sender, args) =>
        {
            RemoveFinishedGames();
        };

        _checkForFinishedGamesTimer.AutoReset = true;
        _checkForFinishedGamesTimer.Enabled = true;
    }

    private void RemoveFinishedGames()
    {
        List<UnoGame> gamesToBeRemoved  = new List<UnoGame>();
        foreach (var game in _runningGames)
        {
            if (game.IsGameOver)
            {
                gamesToBeRemoved.Add(game);
            }
        }

        foreach (var unoGame in gamesToBeRemoved)
        {
            if (_runningGames.Contains(unoGame))
            {
                _runningGames.Remove(unoGame);
            }
        }
    }

    private async Task EndGameBecauseNotStarted(RestFollowupMessage context, Guid gameId)
    {
        var unoGame = _runningGames.SingleOrDefault(x => x.Id == gameId);
        if (unoGame == null)
        {
            return;
        }

        await context.ModifyAsync(prop =>
        {
            prop.Embed = new EmbedBuilder().WithColor(Colors.Red).WithAuthor(new EmbedAuthorBuilder().WithName("UNO"))
                .WithDescription("Game ended before it even starts!").Build();
            prop.Components = new ComponentBuilder().Build();
        });

        _runningGames.Remove(unoGame);
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
                .WithDescription($"{game.Host.SocketUser.Mention} has started a game of UNO! Click the button below to join!\n\n{game.ListPlayers(listCardCount:false)}\n\n*{command.User.Mention} just joined*" +
                                 $"{(_startTime == null ? "" : $"{Environment.NewLine}Game will end automatically <t:{_startTime.Value.ToUnixTimeSeconds()}:R> if not started.")}")
                .Build();

            m.Components = new ComponentBuilder()
                .WithButton("Start Game", $"start-uno_{game.Id}", row: 0, style: ButtonStyle.Secondary, disabled: game.NumberOfPlayers < UnoGameConfiguration.MinimumPlayers)
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

        await game.StartGame();

        if (_timer is { Enabled: true })
        {
            _timer.Stop();
        }
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

    public async Task LeaveGame(SocketMessageComponent context, Guid gameId)
    {
        if (_runningGames.All(x => x.ChannelId != context.Channel.Id))
        {
            await context.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
        if (!game.IsPlayerExist(context.User.Id))
        {
            await context.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }
        
        await game.RemovePlayer(context);

        if (game.IsGameOver)
        {
            _runningGames.Remove(game);
        }
    }

    public async Task LeaveDuringGame(SocketMessageComponent context, Guid parse)
    {
        if (_runningGames.All(x => x.ChannelId != context.Channel.Id))
        {
            await context.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
        if (!game.IsPlayerExist(context.User.Id))
        {
            await context.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }

        await game.RemovePlayerDuringGame(context);

        if (game.IsGameOver)
        {
            _runningGames.Remove(game);
        }
    }

    public async Task CancelWild(SocketMessageComponent context, Guid parse)
    {
        if (_runningGames.All(x => x.ChannelId != context.Channel.Id))
        {
            await context.RespondAsync("No game running here..", ephemeral: true);
            return;
        }

        var game = _runningGames.Single(x => x.ChannelId == context.Channel.Id);
        if (!game.IsPlayerExist(context.User.Id))
        {
            await context.RespondAsync("Not involved here..", ephemeral: true);
            return;
        }

        await game.CancelPlayerWild(context);
    }
}