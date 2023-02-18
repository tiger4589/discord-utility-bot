using MediatR;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class AddVerifyConfigurationRequestHandler : IRequestHandler<AddVerifyConfigurationRequest>
{
    private readonly IConfigurationService _configurationService;

    public AddVerifyConfigurationRequestHandler(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<Unit> Handle(AddVerifyConfigurationRequest request, CancellationToken cancellationToken)
    {
        await _configurationService.AddVerifyConfiguration(request.VerifyConfiguration);
        return Unit.Value;
    }
}