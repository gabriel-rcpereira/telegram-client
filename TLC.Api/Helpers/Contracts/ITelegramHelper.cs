﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;

namespace TLC.Api.Helpers.Contracts
{
    public interface ITelegramHelper
    {
        Task<IEnumerable<TelegramContactResponse>> FindContactsAsync(TelegramHelperVo telegramHelperVo);
        Task<TelegramCodeResponse> SendCodeRequestToClientAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardDailyChannelMessageAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardLastMessageAsync(TelegramHelperVo telegramHelperVo);
        Task UpdateCodeAsync(TelegramHelperVo telegramHelperVo);
        Task ForwardLastChannelMessageAsync(TelegramHelperVo telegramHelperVo);
    }
}
