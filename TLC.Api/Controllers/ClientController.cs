using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ITLClientService _clientService;

        public ClientController(ITLClientService clientService)
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
        public async Task<IActionResult> PostSendCodeRequestToClientAsync()
        {
            await _clientService.SendCodeRequestToClientAsync();
            return Ok();
        }
    }
}