namespace UtilityBot.Domain.DomainObjects;

public class VerifyConfigurationAudit : BaseAudit
{
    public ulong ChannelId { get; set; }
    public ulong RoleId { get; set; }
}