using MediatR;
using UtilityBot.Contracts;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class ConfigurationOfAllServerRequest : IRequest<Configuration>
{
    public IList<ConnectedServer> ConnectedServers { get; set; } = null!;
}