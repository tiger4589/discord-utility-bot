using Discord.Interactions;
using Discord;
using Discord.WebSocket;
using UtilityBot.Services.NoteServices;

namespace UtilityBot.Modules;

[DefaultMemberPermissions(GuildPermission.ManageRoles)]
[Group("note", "A Note system for users")]
public class UserNotesModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly INoteService _noteService;

    public UserNotesModule(INoteService noteService)
    {
        _noteService = noteService;
    }

    [SlashCommand("add", "Add a note about a specific user")]
    public async Task AddNote(SocketGuildUser user, string note)
    {
        await RespondAsync("Adding note");
        _ = _noteService.AddUserNote(Context, user, note);
    }

    [SlashCommand("remove", "Remove a note by note id")]
    public async Task RemoveNote(int id)
    {
        await RespondAsync("Removing note");
        _ = _noteService.RemoveNote(Context, id);
    }

    [SlashCommand("list", "List all notes for a specific user")]
    public async Task ListNotes(SocketGuildUser user)
    {
        await RespondAsync("Getting the list!");
        _ = _noteService.ListNotes(Context, user);
    }
}