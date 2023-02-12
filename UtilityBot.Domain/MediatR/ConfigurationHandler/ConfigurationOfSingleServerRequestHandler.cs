using MediatR;
using UtilityBot.Contracts;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class ConfigurationOfSingleServerRequestHandler : IRequestHandler<ConfigurationOfSingleServerRequest, Configuration>
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationOfSingleServerRequestHandler(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<Configuration> Handle(ConfigurationOfSingleServerRequest request, CancellationToken cancellationToken)
    {
        return await _configurationService.GetConfigurationsOfConnectedServer(request.ConnectedServer);
    }
}