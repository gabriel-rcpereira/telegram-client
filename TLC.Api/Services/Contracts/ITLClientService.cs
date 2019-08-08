using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;

namespace TLC.Api.Services.Contracts
{
    public interface ITLClientService
    {
        Task ForwardDailyMessageAsync();
        Task<IEnumerable<ContactResponse>> FindContactsAsync();
        Task SendCodeRequestToClientAsync();
        Task ReceiveCodeRequestedAsync(string phoneCodeHash, string code);
    }
}
