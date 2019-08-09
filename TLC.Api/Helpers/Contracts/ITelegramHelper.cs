using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;

namespace TLC.Api.Helpers.Contracts
{
    public interface ITelegramHelper
    {
        Task<IEnumerable<TelegramContactResponse>> FindContactsAsync(int id, string hash);
        Task<TelegramCodeResponse> SendCodeRequestToClientAsync(int id, string hash, string phoneNumber);
        Task ForwardDailyMessageAsync(TelegramHelperVo telegramHelperVo);
        Task UpdateCodeAsync(TelegramHelperVo telegramHelperVo);
    }
}
