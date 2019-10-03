using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;

namespace TLC.Api.Helpers.Contracts
{
    public interface ITelegramHelper
    {
        Task<IEnumerable<TelegramContactResponse>> FindContactsAsync(TelegramHelperVo telegramHelperVo);
        Task<TelegramCodeResponse> StartAuthenticationAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardDailyChannelMessageAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardLastMessageAsync(TelegramHelperVo telegramHelperVo);
        Task MakeAuthenticationAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardLastChannelMessageAsync(TelegramHelperVo telegramHelperVo);
    }
}
