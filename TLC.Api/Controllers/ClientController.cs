using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostForwardDailyMessageAsync()
        {
            await _clientService.ForwardDailyMessageAsync();
            return Ok();
        }

        [HttpPost("codes")]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostSendCodeRequestToClientAsync()
        {
            return Ok(await _clientService.SendCodeRequestToClientAsync());
        }

        [HttpPut("codes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutReceiveCodeRequestedAsync([FromBody] ClientRequest clientRequest)
        {
            await _clientService.UpdateCodeAsync(clientRequest.PhoneCodeHash, clientRequest.Code);
            return Ok();
        }
    }
}