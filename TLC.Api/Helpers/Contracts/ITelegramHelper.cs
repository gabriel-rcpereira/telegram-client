using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;

namespace TLC.Api.Helpers.Contracts
{
    public interface ITelegramHelper
    {
        Task<IEnumerable<TelegramContactResponse>> FindContactsAsync(int id, string hash);
    }
}
