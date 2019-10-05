using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger _logger;

        public ContactController(IContactService contactService, ILogger logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContactsAsync()
        {
            _logger.LogInformation("Getting the contacts data.");
            return Ok(await _contactService.FindContactsAsync());
        }
    }
}