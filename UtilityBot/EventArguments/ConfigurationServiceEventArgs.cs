using Discord.Interactions;

namespace UtilityBot.EventArguments;

public class ConfigurationServiceEventArgs : EventArgs
{
    public ConfigurationServiceEventArgs(SocketInteractionContext interactionContext)
    {
        InteractionContext = interactionContext;
    }

    public SocketInteractionContext InteractionContext { get; }
}