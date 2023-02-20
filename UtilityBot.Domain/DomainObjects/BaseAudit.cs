using System.ComponentModel.DataAnnotations;

namespace UtilityBot.Domain.DomainObjects;

public class BaseAudit
{
    [Key]
    public int Id { get; set; }
    public DateTime CreationDate { get; set; }
    public string UpdateType { get; set; } = null!;
}