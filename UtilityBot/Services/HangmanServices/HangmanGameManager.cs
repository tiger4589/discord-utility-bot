using System.Collections;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace UtilityBot.Services.HangmanServices;

public interface IHangmanGameManager
{
    Task StartNewGame(SocketInteractionContext context, EWordsFormat format);
    Task ForceStopGame(SocketInteractionContext context);
    Task GuessLetter(SocketInteractionContext context, char letter);
    Task GiveUpGame(SocketInteractionContext context);
    Task GoNextWithLetters(SocketInteractionContext context);
    Task GoBackWithLetters(SocketInteractionContext context);
    Task GetPersonalStats(SocketInteractionContext context);
    Task GetTopStats(SocketInteractionContext context, SortBy sortBy);
}

public class HangmanGameManager : IHangmanGameManager
{
    private readonly IHangmanService _hangmanService;
    private readonly Hashtable _hangmanGames = new();
    private readonly Hashtable _hangmanMessages = new();
    private readonly object _lock = new();

    public HangmanGameManager(IHangmanService hangmanService)
    {
        _hangmanService = hangmanService;
    }

    public async Task StartNewGame(SocketInteractionContext context, EWordsFormat format)
    {
        bool isRunning;
        lock (_lock)
        {
            isRunning = _hangmanGames.ContainsKey(context.User.Id);
        }

        if (isRunning)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "You already have a game in progress! If this is an error, try using /force-stop-my-game");
            return;
        }

        var word = await _hangmanService.GetRandomWord();
        if (word.Item1 == null && !word.Item2)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "Couldn't find a random word from API - Blame tiger45.");
            return;
        }

        if (word.Item1 == null && word.Item2)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "Hangman word request limit reached!");
            return;
        }

        var game = new HangmanGame(word.Item1!, format);
        lock (_lock)
        {
            _hangmanGames.Add(context.User.Id, game);
            _hangmanMessages.Add(context.User.Id, context.Interaction);
        }

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
        {
            prop.Content = string.Empty;
            prop.Embed = BuildEmbedForGame(game, context.User);
            prop.Components = BuildComponentsForGame(game);
        });
    }

    public async Task GuessLetter(SocketInteractionContext context, char letter)
    {
        bool isRunning;
        lock (_lock)
        {
            isRunning = _hangmanGames.ContainsKey(context.User.Id);
        }

        if (!isRunning)
        {
            await context.Interaction.RespondAsync(
                "THIS IS NOT YOUR GAME! Or, sadly, the game finished somehow, please start a new one.",
                ephemeral: true);
            return;
        }

        HangmanGame game;
        SocketInteraction interaction;
        lock (_lock)
        {
            game = (HangmanGame)_hangmanGames[context.User.Id]!;
            interaction = (SocketInteraction)_hangmanMessages[context.User.Id]!;
        }

        var result = game.GuessLetter(letter);
        if (result == -1)
        {
            await context.Interaction.RespondAsync(
                "You already guessed this letter!",
                ephemeral: true);
            return;
        }

        await context.Interaction.DeferAsync();

        await interaction.ModifyOriginalResponseAsync(prop =>
        {
            prop.Content = string.Empty;
            prop.Embed = BuildEmbedForGame(game, context.User, letter, result);
            prop.Components = BuildComponentsForGame(game);
        });

        if (game.State is HangmanGameState.Failed or HangmanGameState.Finished)
        {
            await _hangmanService.SaveGame(game, context.User.Id);
            lock (_lock)
            {
                _hangmanGames.Remove(context.User.Id);
                _hangmanMessages.Remove(context.User.Id);
            }
        }
    }

    public async Task GiveUpGame(SocketInteractionContext context)
    {
        bool isRunning;
        lock (_lock)
        {
            isRunning = _hangmanGames.ContainsKey(context.User.Id);
        }

        if (!isRunning)
        {
            await context.Interaction.RespondAsync(
                "THIS IS NOT YOUR GAME! Or, sadly, the game finished somehow, please start a new one.",
                ephemeral: true);
            return;
        }

        SocketInteraction interaction;
        lock (_lock)
        {
            interaction = (SocketInteraction)_hangmanMessages[context.User.Id]!;
        }

        await interaction.ModifyOriginalResponseAsync(prop => prop.Content = "Your game has been stopped. You can start a new game.");
        await context.Interaction.DeferAsync();
        lock (_lock)
        {
            _hangmanGames.Remove(context.User.Id);
            _hangmanMessages.Remove(context.User.Id);
        }
    }

    public async Task GoNextWithLetters(SocketInteractionContext context)
    {
        bool isRunning;
        lock (_lock)
        {
            isRunning = _hangmanGames.ContainsKey(context.User.Id);
        }

        if (!isRunning)
        {
            await context.Interaction.RespondAsync("Not your game, or game ended somehow!", ephemeral: true);
            return;
        }

        HangmanGame game;
        SocketInteraction interaction;
        lock (_lock)
        {
            game = (HangmanGame)_hangmanGames[context.User.Id]!;
            interaction = (SocketInteraction)_hangmanMessages[context.User.Id]!;
        }

        await interaction.ModifyOriginalResponseAsync(prop =>
        {
            prop.Content = string.Empty;
            prop.Embed = BuildEmbedForGame(game, context.User);
            prop.Components = BuildComponentsForGame(game, true);
        });

        await context.Interaction.DeferAsync();
    }

    public async Task GoBackWithLetters(SocketInteractionContext context)
    {
        bool isRunning;
        lock (_lock)
        {
            isRunning = _hangmanGames.ContainsKey(context.User.Id);
        }

        if (!isRunning)
        {
            await context.Interaction.RespondAsync("Not your game, or game ended somehow!", ephemeral: true);
            return;
        }

        HangmanGame game;
        SocketInteraction interaction;
        lock (_lock)
        {
            game = (HangmanGame)_hangmanGames[context.User.Id]!;
            interaction = (SocketInteraction)_hangmanMessages[context.User.Id]!;
        }

        await interaction.ModifyOriginalResponseAsync(prop =>
        {
            prop.Content = string.Empty;
            prop.Embed = BuildEmbedForGame(game, context.User);
            prop.Components = BuildComponentsForGame(game);
        });

        await context.Interaction.DeferAsync();
    }

    public async Task GetPersonalStats(SocketInteractionContext context)
    {
        var hangmanPersonalStats = await _hangmanService.GetPersonalStats(context.User.Id);

        var sb = new StringBuilder();
        sb.AppendLine("```");
        sb.AppendLine($"Total Games: {hangmanPersonalStats.TotalGames}");
        sb.AppendLine($"Total Wins: {hangmanPersonalStats.TotalWins}");
        sb.AppendLine($"Total Score: {hangmanPersonalStats.TotalScore}");
        sb.AppendLine("```");

        var embed = new EmbedBuilder()
            .WithAuthor(new EmbedAuthorBuilder
            {
                Name = "Hangman Manager"
            })
            .WithCurrentTimestamp()
            .WithTitle($"Hangman Stats for {context.User.Username}")
            .WithDescription(sb.ToString()).Build();

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
        {
            prop.Content = string.Empty;
            prop.Embed = embed;
        });
    }

    public async Task GetTopStats(SocketInteractionContext context, SortBy sortBy)
    {
        var hangmanTopStatsList = await _hangmanService.GetTopStats();

        var results = sortBy switch
        {
            SortBy.Games => hangmanTopStatsList.OrderByDescending(x => x.TotalGames).Select(x => new { x.UserId, Value = x.TotalGames }).Take(10).ToList(),
            SortBy.Score => hangmanTopStatsList.OrderByDescending(x => x.TotalScore).Select(x => new { x.UserId, Value = x.TotalScore }).Take(10).ToList(),
            SortBy.Wins => hangmanTopStatsList.OrderByDescending(x => x.TotalWins).Select(x => new { x.UserId, Value = x.TotalWins }).Take(10).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null)
        };

        var sb = new StringBuilder();
        sb.AppendLine("```");

        for (int i = 0; i < results.Count; i++)
        {
            sb.AppendLine($"#{i + 1} - {await GetUsername(context, results[i].UserId)} - {results[i].Value}");
        }

        sb.AppendLine("```");

        var embed = new EmbedBuilder()
            .WithAuthor(new EmbedAuthorBuilder
            {
                Name = "Hangman Manager"
            })
            .WithCurrentTimestamp()
            .WithTitle($"Top Stats Sorted By {sortBy}")
            .WithDescription(sb.ToString()).Build();

        await context.Interaction.ModifyOriginalResponseAsync(prop =>
        {
            prop.Content = string.Empty;
            prop.Embed = embed;
        });
    }

    private async Task<string> GetUsername(SocketInteractionContext context, ulong userId)
    {
        var user = await context.Client.GetUserAsync(userId);
        if (user == null)
        {
            return "Unknown User";
        }

        return user.Username;
    }

    public async Task ForceStopGame(SocketInteractionContext context)
    {
        bool isRunning;
        lock (_lock)
        {
            isRunning = _hangmanGames.ContainsKey(context.User.Id);
        }

        if (!isRunning)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "You don't have a game in progress! Ask tiger45 for help!");
            return;
        }

        lock (_lock)
        {
            _hangmanGames.Remove(context.User.Id);
            _hangmanMessages.Remove(context.User.Id);
        }

        await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "Your game has been stopped. You can start a new game.");
    }

    private Embed BuildEmbedForGame(HangmanGame game, SocketUser user, char? letter = null, int? places = null)
    {
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(new EmbedAuthorBuilder
            {
                Name = "Hangman Manager"
            })
            .WithCurrentTimestamp()
            .WithTitle($"Hangman vs. {user.Username}")
            .WithDescription(GetDescriptionForGame(game, letter, places))
            .WithColor(GetColor(game));

        return embedBuilder.Build();
    }

    private static Color GetColor(HangmanGame game)
    {
        if (game.State == HangmanGameState.Failed)
        {
            return Color.Red;
        }

        if (game.State == HangmanGameState.Finished)
        {
            return Color.Green;
        }

        if (game.TriesLeft == 6)
        {
            return Color.LightGrey;
        }

        if (game.TriesLeft == 5)
        {
            return Color.DarkGrey;
        }

        if (game.TriesLeft == 4)
        {
            return Color.DarkerGrey;
        }

        if (game.TriesLeft == 3)
        {
            return Color.DarkBlue;
        }

        if (game.TriesLeft == 2)
        {
            return Color.Blue;
        }

        if (game.TriesLeft == 1)
        {
            return Color.DarkOrange;
        }

        return Color.Orange;
    }

    private string GetDescriptionForGame(HangmanGame game, char? letter, int? places)
    {
        var sb = new StringBuilder();
        sb.AppendLine("```");
        sb.AppendLine($"Word: {game.HiddenWord}");
        sb.AppendLine($"Number of letters: {game.NumberOfLetters}");
        sb.AppendLine($"Letters left: {game.HiddenWord.Count(x => x == '_')}");
        sb.AppendLine($"Tries Left: {game.TriesLeft}");
        sb.AppendLine();
        DrawHangman(sb, game);
        DrawMessage(sb, game, letter, places);
        sb.AppendLine("```");

        return sb.ToString();
    }

    private void DrawMessage(StringBuilder sb, HangmanGame game, char? letter = null, int? places = null)
    {
        if (letter != null && places != null)
        {
            sb.AppendLine($"| The letter '{letter}' was found in {places} places!");
        }

        AddUsedLetters(sb, game);

        if (game.State == HangmanGameState.Failed)
        {
            sb.AppendLine($"| GAME OVER! The word was '{game.OriginalWord}'");
            DrawEndOfGameMessages(sb, game);
            return;
        }

        if (game.State == HangmanGameState.Finished)
        {
            sb.AppendLine($"| YOU WIN! The word was indeed '{game.OriginalWord}'");
            DrawEndOfGameMessages(sb, game);
        }
    }

    private void DrawEndOfGameMessages(StringBuilder sb, HangmanGame game)
    {
        AddScore(sb, game);
        DrawDefinition(sb, game);
    }

    private void AddScore(StringBuilder sb, HangmanGame game)
    {
        sb.AppendLine($"| You have scored {game.Score} points! Hooray!");
    }

    private static void AddUsedLetters(StringBuilder sb, HangmanGame game)
    {
        if (game.UsedLetters.Any())
        {
            sb.AppendLine($"| Used Letters: {string.Join(", ", game.UsedLetters)}");
        }
    }

    private static void DrawDefinition(StringBuilder sb, HangmanGame game)
    {
        sb.AppendLine();
        if (game.Word.Results.Any())
        {
            var result = game.Word.Results.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Definition));
            if (result == null)
            {
                return;
            }
            sb.AppendLine($"Definition: {result.Definition}");
        }
    }

    private void DrawHangman(StringBuilder sb, HangmanGame game)
    {
        DrawTop(sb);
        DrawHead(sb, game);
        DrawBody(sb, game);
        DrawLegs(sb, game);
        DrawBase(sb);
    }

    private void DrawLegs(StringBuilder sb, HangmanGame game)
    {
        if (game.TriesLeft >= 2)
        {
            sb.AppendLine("|");
            return;
        }

        if (game.TriesLeft >= 1)
        {
            sb.AppendLine("|    /");
            return;
        }

        sb.AppendLine("|    / \\");
    }

    private void DrawBody(StringBuilder sb, HangmanGame game)
    {
        if (game.TriesLeft >= 5)
        {
            sb.AppendLine("|");
            return;
        }

        if (game.TriesLeft >= 4)
        {
            sb.AppendLine("|     |");
            return;
        }

        if (game.TriesLeft >= 3)
        {
            sb.AppendLine("|    /|");
            return;
        }

        sb.AppendLine("|    /|\\");
    }

    private void DrawHead(StringBuilder sb, HangmanGame game)
    {
        sb.AppendLine(game.TriesLeft <= 5 ? "|     O" : "|");
    }

    private void DrawTop(StringBuilder sb)
    {
        sb.AppendLine("_______");
        sb.AppendLine("|     |");
    }

    private void DrawBase(StringBuilder sb)
    {
        sb.AppendLine("|");
        sb.AppendLine("|_____");
    }


    private MessageComponent BuildComponentsForGame(HangmanGame game, bool isExtra = false)
    {
        var componentBuilder = new ComponentBuilder();

        if (game.State is HangmanGameState.Failed or HangmanGameState.Finished)
        {
            return componentBuilder.Build();
        }

        if (isExtra)
        {
            componentBuilder.WithButton("Back", "less_letters_hangman");

            for (int i = 24; i < game.AvailableLetters.Count; i++)
            {
                componentBuilder.WithButton(game.AvailableLetters[i].ToString(), $"guess_letter_{game.AvailableLetters[i]}", ButtonStyle.Secondary);
            }

            componentBuilder.WithButton("Give Up", "give_up_hangman", ButtonStyle.Danger);

            return componentBuilder.Build();
        }

        if (game.AvailableLetters.Count < 24)
        {
            foreach (var letter in game.AvailableLetters)
            {
                componentBuilder.WithButton(letter.ToString(), $"guess_letter_{letter}", ButtonStyle.Secondary);
            }

            componentBuilder.WithButton("Give Up", "give_up_hangman", ButtonStyle.Danger);

            return componentBuilder.Build();
        }

        for (int i = 0; i < 24; i++)
        {
            componentBuilder.WithButton(game.AvailableLetters[i].ToString(), $"guess_letter_{game.AvailableLetters[i]}", ButtonStyle.Secondary);
        }

        componentBuilder.WithButton("Next", "extra_letters_hangman");

        return componentBuilder.Build();
    }
}

public class HangmanGame
{
    private readonly EWordsFormat _format;
    public Guid Id { get; } = Guid.NewGuid();
    public HangmanGameState State { get; set; } = HangmanGameState.InProgress;
    public HangmanWord Word { get; }
    public List<char> AvailableLetters => _availableLetters;
    public List<char> UsedLetters => _usedLetters;
    public int NumberOfLetters => _numberOfLetters;
    public int TriesLeft => 6 - _tries;
    public string HiddenWord { get; private set; } = string.Empty;
    public string OriginalWord { get; }
    public int Score => _numberOfLettersFound + TriesLeft + (State == HangmanGameState.Finished ? OriginalWord.Length : 0);

    private readonly List<char> _allLetters = new()
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };

    private readonly List<char> _availableLetters = new();
    private readonly List<char> _usedLetters = new();
    private int _tries;
    private int _numberOfLettersFound;
    private int _numberOfLetters;
    public HangmanGame(HangmanWord word, EWordsFormat format)
    {
        _format = format;
        Word = word;
        OriginalWord = word.Word.ToLower();
        Initialize();
    }

    private void Initialize()
    {
        switch (_format)
        {
            case EWordsFormat.WithoutSpace:
                InitializeWithoutSpace();
                break;
            case EWordsFormat.WithSpace:
                InitializeWithSpace();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InitializeWithSpace()
    {

        List<char> hiddenWordWithSpaces = new();

        for (int i = 0; i < OriginalWord.Length; i++)
        {
            char o = OriginalWord[i];
            if (_allLetters.Contains(o))
            {
                _numberOfLetters++;
                hiddenWordWithSpaces.Add('_');
                hiddenWordWithSpaces.Add(' ');
                continue;
            }

            hiddenWordWithSpaces.Add(o);
            hiddenWordWithSpaces.Add(' ');
        }

        HiddenWord = new string(hiddenWordWithSpaces.ToArray());
        _availableLetters.AddRange(_allLetters);
    }

    private void InitializeWithoutSpace()
    {
        char[] hiddenWord = new char[OriginalWord.Length];
        for (int i = 0; i < OriginalWord.Length; i++)
        {
            char o = OriginalWord[i];
            if (_allLetters.Contains(o))
            {
                _numberOfLetters++;
                hiddenWord[i] = '_';
                continue;
            }

            hiddenWord[i] = o;
        }

        HiddenWord = new string(hiddenWord);
        _availableLetters.AddRange(_allLetters);
    }

    public int GuessLetter(char letter)
    {
        if (!_availableLetters.Contains(letter))
        {
            return -1;
        }

        _availableLetters.Remove(letter);
        _usedLetters.Add(letter);
        int numberOfPlaces = 0;

        if (OriginalWord.Contains(letter))
        {
            _numberOfLettersFound++;
            char[] hiddenWord = HiddenWord.ToCharArray();
            for (int i = 0; i < OriginalWord.Length; i++)
            {
                if (OriginalWord[i] == letter)
                {
                    numberOfPlaces++;
                    switch (_format)
                    {
                        case EWordsFormat.WithoutSpace:
                            hiddenWord[i] = letter;
                            break;
                        case EWordsFormat.WithSpace:
                            hiddenWord[i*2] = letter;
                            break;
                    }
                }
            }

            HiddenWord = new string(hiddenWord);
            if (!HiddenWord.Contains('_'))
            {
                State = HangmanGameState.Finished;
            }

            return numberOfPlaces;
        }

        _tries++;
        if (_tries >= 6)
        {
            State = HangmanGameState.Failed;
        }

        return numberOfPlaces;
    }
}

public enum HangmanGameState
{
    InProgress,
    Finished,
    Failed
}

public enum SortBy
{
    Games,
    Score,
    Wins
}

public enum EWordsFormat
{
    [ChoiceDisplay("Without Space")]
    WithoutSpace,
    [ChoiceDisplay("With Space")]
    WithSpace
}