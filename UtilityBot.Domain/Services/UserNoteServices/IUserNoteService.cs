using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.UserNoteServices;

public interface IUserNoteService
{
    Task<int> CreateNote(ulong userId, string username, ulong addedById, string addedByUsername, string note);
    Task<IEnumerable<UserNote>> GetNotes(ulong userId);
    Task DeleteNote(int id);
}