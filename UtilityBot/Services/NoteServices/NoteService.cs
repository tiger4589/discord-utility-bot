using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UtilityBot.Domain.Services.UserNoteServices;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.NoteServices;

public class NoteService : INoteService
{
    private readonly IUserNoteService _userNoteService;

    public NoteService(IUserNoteService userNoteService)
    {
        _userNoteService = userNoteService;
    }

    public async Task AddUserNote(SocketInteractionContext context, SocketGuildUser user, string note)
    {
        try
        {
            var newId = await _userNoteService.CreateNote(user.Id, user.Username, context.User.Id, context.User.Username, note);
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"New note added, id={newId}");
        }
        catch (Exception e)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"Couldn't add new note! Check logs!");
            await Logger.Log(e.ToString());
        }
    }

    public async Task ListNotes(SocketInteractionContext context, SocketGuildUser user)
    {
        try
        {
            var noteList = (await _userNoteService.GetNotes(user.Id)).ToArray();
            if (!noteList.Any())
            {
                await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = "No notes found");
                return;
            }

            foreach (var userNote in noteList)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("```");

                sb.AppendLine($"Note Id: {userNote.Id} - Created: {userNote.CreationDate} - Created by: {userNote.AddedByUsername}");
                sb.AppendLine($"User username at time of creation: {userNote.Username}");
                sb.AppendLine($"{userNote.Note}");
                sb.AppendLine("```");
                await context.Channel.SendMessageAsync(sb.ToString());
            }
        }
        catch (Exception e)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"Couldn't get a list for this user!");
            await Logger.Log(e.ToString());
        }
    }

    public async Task RemoveNote(SocketInteractionContext context, int noteId)
    {
        try
        {
           await _userNoteService.DeleteNote(noteId);
           await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"Deleted note {noteId}");
        }
        catch (Exception e)
        {
            await context.Interaction.ModifyOriginalResponseAsync(prop => prop.Content = $"Couldn't delete this note!");
            await Logger.Log(e.ToString());
        }
    }
}