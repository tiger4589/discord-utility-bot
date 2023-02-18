using MediatR;
using UtilityBot.Contracts;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class AddVerifyConfigurationRequest : IRequest
{
    public VerifyConfiguration VerifyConfiguration { get; set; } = null!;
}