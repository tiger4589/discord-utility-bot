using MediatR;
using UtilityBot.Contracts;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class AddUserJoinMessageConfigurationRequest : IRequest
{
    public UserJoinConfiguration UserJoinConfiguration { get; set; } = null!;
    public UserJoinMessage UserJoinMessage { get; set; } = null!;
}