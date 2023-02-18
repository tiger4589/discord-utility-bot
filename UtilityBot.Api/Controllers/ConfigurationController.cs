using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UtilityBot.Contracts;
using UtilityBot.Domain.MediatR.ConfigurationHandler;

namespace UtilityBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("get-servers-configuration")]
        public async Task<IActionResult> GetServerConfigurations([FromBody] IList<ConnectedServer> connectedServers)
        {
            var configuration = await _mediator.Send(new ConfigurationOfAllServerRequest
            {
                ConnectedServers = connectedServers
            });

            return Ok(configuration);
        }

        [HttpPost("get-server-configuration")]
        public async Task<IActionResult> GetServerConfiguration([FromBody] ConnectedServer connectedServer)
        {
            var configuration = await _mediator.Send(new ConfigurationOfSingleServerRequest()
            {
                ConnectedServer = connectedServer
            });

            return Ok(configuration);
        }

        [HttpPost("add-user-join-role-conf")]
        public async Task<IActionResult> AddUserJoinRoleConfiguration(
            [FromBody] UserJoinRoleConfiguration joinRoleConfiguration)
        {
            await _mediator.Send(new AddUserJoinRoleConfigurationRequest
            {
                UserJoinRole = joinRoleConfiguration.UserJoinRole,
                UserJoinConfiguration = joinRoleConfiguration.UserJoinConfiguration
            });

            return Ok();
        }

        [HttpPost("add-user-join-message-conf")]
        public async Task<IActionResult> AddUserJoinMessageConfiguration(
            [FromBody] UserJoinMessageConfiguration joinMessageConfiguration)
        {
            await _mediator.Send(new AddUserJoinMessageConfigurationRequest
            {
                UserJoinConfiguration = joinMessageConfiguration.UserJoinConfiguration,
                UserJoinMessage = joinMessageConfiguration.UserJoinMessage
            });

            return Ok();
        }

        [HttpPost("add-verify-configuration")]
        public async Task<IActionResult> AddVerifyConfiguration([FromBody] VerifyConfiguration configuration)
        {
            await _mediator.Send(new AddVerifyConfigurationRequest
            {
                VerifyConfiguration = configuration
            });

            return Ok();
        }

        [HttpPost("get-verify-configuration")]
        public async Task<IActionResult> GetVerifyConfiguration()
        {
            var verifyConfiguration = await _mediator.Send(new GetVerifyConfigurationRequest());

            return Ok(verifyConfiguration);
        }
    }
}
