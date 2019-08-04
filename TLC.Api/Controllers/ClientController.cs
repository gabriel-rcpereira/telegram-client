using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Request;
using TLC.Api.Models.Requests;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
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

        [HttpPost("features/messages")]
        public async Task<IActionResult> PostForwardMessage([FromBody] TLClientRequest clientRequest)
        {
            ClientVo clientVo = CreateClientVo(clientRequest);
            await _clientService.ForwardTodayMessageAsync(clientVo);

            return Ok();
        }

        [HttpPost("features/contacts")]
        public async Task<IActionResult> PostRequestContacts([FromBody] TLClientRequest clientRequest)
        {
            ClientVo clientVo = CreateClientVo(clientRequest);
            IEnumerable<ContactResponse> clientResponse = await _clientService.GetContacts(clientVo);

            return Ok(clientResponse);
        }

        private ClientVo CreateClientVo(TLClientRequest clientRequest)
        {
            return new ClientVo.Builder()
                            .WithAccount(CreateAccountVo(clientRequest.Account))
                            .WithFromUser(CreateUserVo(clientRequest.FromUser))
                            .WithToUsers(CreateUsersVo(clientRequest.ToUsers))
                            .Build();
        }

        private AccountVo CreateAccountVo(AccountRequest account)
        {
            return new AccountVo.Builder()
                .WithId(account.Id)
                .WithHash(account.Hash)
                .Build();
        }

        private IEnumerable<UserVo> CreateUsersVo(IEnumerable<UserRequest> toUsers)
        {
            return new List<UserRequest>(toUsers)
                .ConvertAll<UserVo>(CreateUserVo);
        }

        private UserVo CreateUserVo(UserRequest user)
        {
            return new UserVo.Builder()
                .WithId(user.Id)
                .Build();
        }
    }
}