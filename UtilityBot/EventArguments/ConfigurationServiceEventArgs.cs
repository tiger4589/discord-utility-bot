using Discord.Interactions;

namespace UtilityBot.EventArguments;

public class ConfigurationServiceEventArgs : EventArgs
{
    public ConfigurationServiceEventArgs(SocketInteractionContext interactionContext, string message = "")
    {
        InteractionContext = interactionContext;
        Message = message;
    }

    public SocketInteractionContext InteractionContext { get; }
    public string Message { get; }
}