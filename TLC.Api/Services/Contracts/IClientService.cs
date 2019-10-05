using System.Threading.Tasks;
using TLC.Api.Models.Responses;

namespace TLC.Api.Services.Contracts
{
    public interface IClientService
    {
        Task ForwardDailyChannelMessageAsync();
        Task<ClientResponse> StartAuthenticationAsync();
        Task MakeAuthenticationAsync(string phoneCodeHash, string code);
    }
}
