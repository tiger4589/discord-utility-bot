using MediatR;
using UtilityBot.Contracts;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class GetVerifyConfigurationRequestHandler : IRequestHandler<GetVerifyConfigurationRequest, VerifyConfiguration?>
{
    private readonly IConfigurationService _configurationService;

    public GetVerifyConfigurationRequestHandler(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    public async Task<VerifyConfiguration?> Handle(GetVerifyConfigurationRequest request, CancellationToken cancellationToken)
    {
        return await _configurationService.GetVerifyConfiguration();
    }
}