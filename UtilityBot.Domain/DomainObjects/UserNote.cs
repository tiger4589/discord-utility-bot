using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class UserNote
{
    [Key]
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public string Username { get; set; } = null!;
    public ulong AddedBy { get; set; }
    public string AddedByUsername { get; set; } = null!;
    public string Note { get; set; } = null!;
    public DateTime CreationDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionDate { get; set; }
}