using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("messages")]
        public async Task<IActionResult> PostForwardDailyMessageAsync()
        {
            await _clientService.ForwardDailyMessageAsync();
            return Ok();
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetContactsAsync()
        {
            return Ok(await _clientService.FindContactsAsync());
        }

        [HttpPost("codes")]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostSendCodeRequestToClientAsync()
        {
            return Ok(await _clientService.SendCodeRequestToClientAsync());
        }

        [HttpPut("codes")]
        public async Task<IActionResult> PutReceiveCodeRequestedAsync([FromBody] ClientRequest clientRequest)
        {
            await _clientService.ReceiveCodeRequestedAsync(clientRequest.PhoneCodeHash, clientRequest.Code);
            return Ok();
        }
    }
}