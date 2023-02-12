using MediatR;
using UtilityBot.Contracts;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class ConfigurationOfSingleServerRequest : IRequest<Configuration>
{
    public ConnectedServer ConnectedServer { get; set; } = null!;
}