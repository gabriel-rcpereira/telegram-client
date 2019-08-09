using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;

namespace TLC.Api.Helpers.Contracts
{
    public interface ITelegramHelper
    {
        Task<IEnumerable<TelegramContactResponse>> FindContactsAsync(int id, string hash);
        Task<TelegramCodeResponse> SendCodeRequestToClientAsync(int id, string hash, string phoneNumber);
    }
}
