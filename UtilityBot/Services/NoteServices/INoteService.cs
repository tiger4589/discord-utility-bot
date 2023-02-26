using Discord.Interactions;
using Discord.WebSocket;

namespace UtilityBot.Services.NoteServices;

public interface INoteService
{
    Task AddUserNote(SocketInteractionContext context, SocketGuildUser user, string note);
    Task ListNotes(SocketInteractionContext context, SocketGuildUser user);
    Task RemoveNote(SocketInteractionContext context, int noteId);


}