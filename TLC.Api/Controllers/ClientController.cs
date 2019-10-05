using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TLC.Api.Models.Requests;
using TLC.Api.Models.Responses;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger _logger;

        public ClientController(IClientService clientService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpPost("messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostForwardDailyChannelMessageAsync()
        {
            _logger.LogInformation("Post request to forward daily channel message.");
            await _clientService.ForwardDailyChannelMessageAsync();
            return Ok();
        }

        [HttpPost("codes")]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostStartAuthenticationAsync()
        {
            _logger.LogInformation("Posting starting authentication using the parameters configured.");
            return Ok(await _clientService.StartAuthenticationAsync());
        }

        [HttpPut("codes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutMakeAuthenticationAsync([FromBody] ClientRequest clientRequest)
        {
            _logger.LogInformation("Puting the client code to make the authentication.");
            await _clientService.MakeAuthenticationAsync(clientRequest.PhoneCodeHash, clientRequest.Code);
            return Ok();
        }
    }
}