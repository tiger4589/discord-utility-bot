using MediatR;
using UtilityBot.Contracts;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class AddUserJoinRoleConfigurationRequest : IRequest
{
    public UserJoinConfiguration UserJoinConfiguration { get; set; } = null!;
    public UserJoinRole UserJoinRole { get; set; } = null!;
}