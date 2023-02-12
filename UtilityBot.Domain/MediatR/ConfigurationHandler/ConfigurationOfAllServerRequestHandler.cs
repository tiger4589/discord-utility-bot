using MediatR;
using UtilityBot.Contracts;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class ConfigurationOfAllServerRequestHandler : IRequestHandler<ConfigurationOfAllServerRequest, Configuration>
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationOfAllServerRequestHandler(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<Configuration> Handle(ConfigurationOfAllServerRequest request, CancellationToken cancellationToken)
    {
        return await _configurationService.GetConfigurationsOfConnectedServers(request.ConnectedServers);
    }
}