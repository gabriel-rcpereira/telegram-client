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
        public async Task<IActionResult> PostForwardDailyMessageAsync()
        {
            _logger.LogInformation("Forwarding daily message.");
            await _clientService.ForwardDailyMessageAsync();
            return Ok();
        }

        [HttpPost("codes")]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostSendCodeRequestToClientAsync()
        {
            _logger.LogInformation("Sending code request to client.");
            return Ok(await _clientService.SendCodeRequestToClientAsync());
        }

        [HttpPut("codes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutCodeRequestedAsync([FromBody] ClientRequest clientRequest)
        {
            _logger.LogInformation("Updating code requested.");
            await _clientService.UpdateCodeAsync(clientRequest.PhoneCodeHash, clientRequest.Code);
            return Ok();
        }
    }
}