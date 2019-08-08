using System.Threading.Tasks;
using TLC.Api.Models.Responses;

namespace TLC.Api.Services.Contracts
{
    public interface IClientService
    {
        Task ForwardDailyMessageAsync();
        Task<ClientResponse> SendCodeRequestToClientAsync();
        Task ReceiveCodeRequestedAsync(string phoneCodeHash, string code);
    }
}
