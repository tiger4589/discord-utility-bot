using MediatR;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class AddUserJoinMessageConfigurationRequestHandler : IRequestHandler<AddUserJoinMessageConfigurationRequest>
{
    private readonly IConfigurationService _configurationService;

    public AddUserJoinMessageConfigurationRequestHandler(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<Unit> Handle(AddUserJoinMessageConfigurationRequest request, CancellationToken cancellationToken)
    {
        await _configurationService.AddUserJoinMessageConfiguration(request.UserJoinConfiguration,
            request.UserJoinMessage);
        return Unit.Value;
    }
}