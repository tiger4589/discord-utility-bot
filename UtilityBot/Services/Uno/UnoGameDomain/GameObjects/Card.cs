using Discord;
using UtilityBot.Services.Uno.UnoGameDomain.GameAssets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UtilityBot.Services.Uno.UnoGameDomain.GameObjects;

public class Card
{
    public EColor Color { get; set; }
    public ESpecial Special { get; set; }
    public string Value { get; set; } = string.Empty;
    public Guid UniqueId { get; set; }

    public Color GetDiscordColor()
    {
        switch (Color)
        {
            case EColor.Red:
                return Colors.UnoRed;

            case EColor.Green:
                return Colors.UnoGreen;

            case EColor.Blue:
                return Colors.UnoBlue;

            case EColor.Yellow:
                return Colors.UnoYellow;

            default:
                return Colors.Black;
        }
    }

    public string GetImageUrl(EColor color = EColor.None)
    {
        if (Special == ESpecial.None)
        {
            return $"https://raw.githubusercontent.com/tiger4589/discord-utility-bot/main/UtilityBot/Services/Uno/UnoGameDomain/images/{Color}{Value}.png";
        }

        if (color == EColor.None)
        {
            return $"https://raw.githubusercontent.com/tiger4589/discord-utility-bot/main/UtilityBot/Services/Uno/UnoGameDomain/images/{Color}{Value}{Special}.png";
        }

        return $"https://raw.githubusercontent.com/tiger4589/discord-utility-bot/main/UtilityBot/Services/Uno/UnoGameDomain/images/{color}{Value}{Special}.png";
    }

    public override string ToString()
    {
        return $"{Color.ToString().Replace("None", "")} {Value}{(Special == ESpecial.None ? "" : Special.ToString().Replace("Plus", "+").Replace("Four", "4").Replace("DrawTwo", "+2"))} {GetSpecialEmoji()}";
    }

    public string GetSpecialEmoji()
    {
        switch (Special)
        {
            case ESpecial.Reverse:
                return "🔃";

            case ESpecial.Skip:
                return "🚫";
        }

        return "";
    }

    public Emoji GetColorEmoji()
    {
        switch (Color)
        {
            case EColor.Red:
                return new Emoji("🟥");

            case EColor.Green:
                return new Emoji("🟩");

            case EColor.Blue:
                return new Emoji("🟦");

            case EColor.Yellow:
                return new Emoji("🟨");

            default:
                return new Emoji("🎨");

        }
    }
}