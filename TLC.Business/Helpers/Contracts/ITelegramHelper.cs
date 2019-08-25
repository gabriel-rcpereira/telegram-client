using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Business.Models.Responses;
using TLC.Business.Models.Vo;

namespace TLC.Business.Helpers.Contracts
{
    public interface ITelegramHelper
    {
        Task<IEnumerable<TelegramContactResponse>> FindContactsAsync(int id, string hash);
        Task<TelegramCodeResponse> SendCodeRequestToClientAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardDailyMessageAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardLastMessageAsync(TelegramHelperVo telegramHelperVo);
        Task UpdateCodeAsync(TelegramHelperVo telegramHelperVo);
    }
}
