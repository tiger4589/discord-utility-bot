using UtilityBot.Services.Uno.UnoGameDomain.GameObjects;

namespace UtilityBot.Services.Uno.UnoGameDomain.GameAssets;

public class OrganizedDeck
{
    public static IList<Card> GetDeck()
    {
        var result = new List<Card>();

        //The 2018 edition of the game consists of 112 cards:
        //25 in each of four color suits (red, yellow, green, blue),
        //each suit consisting of one zero,
        //                        two each of 1 through 9,
        //                        and two each of the action cards "Skip",
        //                                                          "Draw Two",
        //                                                        and "Reverse".
        //The deck also contains four "Wild" cards, four "Wild Draw Four"

        //Create Colored Cards
        result.AddRange(GetCardsByColor(EColor.Red));
        result.AddRange(GetCardsByColor(EColor.Blue));
        result.AddRange(GetCardsByColor(EColor.Green));
        result.AddRange(GetCardsByColor(EColor.Yellow));
        
        //Add Wild Cards!
        for (int i = 0; i < 4; i++)
        {
            result.Add(new Card
            {
                Value = "",
                Color = EColor.None,
                Special = ESpecial.Wild,
                UniqueId = Guid.NewGuid()
            });
        }

        for (int i = 0; i < 4; i++)
        {
            result.Add(new Card
            {
                Value = "",
                Color = EColor.None,
                Special = ESpecial.WildPlusFour,
                UniqueId = Guid.NewGuid()
            });
        }

        return result;
    }

    private static IList<Card> GetCardsByColor(EColor color)
    {
        var result = new List<Card>();

        result.Add(new Card
        {
            Value = "0",
            Color = color,
            Special = ESpecial.None,
            UniqueId = Guid.NewGuid()
        });

        for (int i = 1; i < 10; i++)
        {
            result.Add(new Card
            {
                Value = $"{i}",
                Color = color,
                Special = ESpecial.None,
                UniqueId = Guid.NewGuid()
            });
            result.Add(new Card
            {
                Value = $"{i}",
                Color = color,
                Special = ESpecial.None,
                UniqueId = Guid.NewGuid()
            });
        }

        result.Add(new Card
        {
            Value = "",
            Color = color,
            Special = ESpecial.Skip,
            UniqueId = Guid.NewGuid()
        });

        result.Add(new Card
        {
            Value = "",
            Color = color,
            Special = ESpecial.Skip,
            UniqueId = Guid.NewGuid()
        });

        result.Add(new Card
        {
            Value = "",
            Color = color,
            Special = ESpecial.DrawTwo,
            UniqueId = Guid.NewGuid()
        });

        result.Add(new Card
        {
            Value = "",
            Color = color,
            Special = ESpecial.DrawTwo,
            UniqueId = Guid.NewGuid()
        });

        result.Add(new Card
        {
            Value = "",
            Color = color,
            Special = ESpecial.Reverse,
            UniqueId = Guid.NewGuid()
        });

        result.Add(new Card
        {
            Value = "",
            Color = color,
            Special = ESpecial.Reverse,
            UniqueId = Guid.NewGuid()
        });

        return result;
    }
}