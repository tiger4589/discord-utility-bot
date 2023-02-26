using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Services.UserNoteServices;

public class UserNoteService : IUserNoteService
{
    private readonly UtilityBotContext _context;

    public UserNoteService(UtilityBotContext context)
    {
        _context = context;
    }

    public async Task<int> CreateNote(ulong userId, string username, ulong addedById, string addedByUsername, string note)
    {
        var userNote = new UserNote
        {
            Username = username,
            AddedBy = addedById,
            AddedByUsername = addedByUsername,
            CreationDate = DateTime.Now,
            Note = note,
            UserId = userId
        };

        await _context.UserNotes!.AddAsync(userNote);
        await _context.SaveChangesAsync();

        return userNote.Id;
    }

    public async Task<IEnumerable<UserNote>> GetNotes(ulong userId)
    {
        return await _context.UserNotes!.AsNoTracking().Where(x => x.UserId == userId && !x.IsDeleted).ToListAsync();
    }

    public async Task DeleteNote(int id)
    {
        var userNote = await _context.UserNotes!.SingleOrDefaultAsync(x => x.Id == id);

        if (userNote == null)
        {
            return;
        }

        userNote.IsDeleted = true;
        userNote.DeletionDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }
}