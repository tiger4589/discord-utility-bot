using MediatR;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;

namespace UtilityBot.Domain.MediatR.ConfigurationHandler;

public class AddUserJoinRoleConfigurationRequestHandler : IRequestHandler<AddUserJoinRoleConfigurationRequest>
{
    private readonly IConfigurationService _configurationService;

    public AddUserJoinRoleConfigurationRequestHandler(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<Unit> Handle(AddUserJoinRoleConfigurationRequest request, CancellationToken cancellationToken)
    {
        await _configurationService.AddUserJoinRoleConfiguration(request.UserJoinConfiguration, request.UserJoinRole);
        return Unit.Value;
    }
}