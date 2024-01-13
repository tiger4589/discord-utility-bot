namespace UtilityBot.Casino.HorseRaces;

public class RaceEventArgs : EventArgs
{
    public Race Race { get; }

    public RaceEventArgs(Race race)
    {
        Race = race;
    }
}