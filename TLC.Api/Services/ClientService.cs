using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly TelegramConfiguration _telegramConfiguration;
        private readonly ITelegramHelper _telegramHelper;
        private readonly IMapper _mapper;

        public ClientService(IOptions<TelegramConfiguration> clientConfiguration,
            ITelegramHelper telegramHelper,
            IMapper mapper)
        {
            _telegramConfiguration = clientConfiguration.Value;
            _telegramHelper = telegramHelper;
            _mapper = mapper;
        }

        async Task IClientService.ForwardDailyMessageAsync()
        {
            await _telegramHelper.ForwardDailyChannelMessageAsync(_mapper.Map<TelegramHelperVo>(_telegramConfiguration));
        }

        async Task<ClientResponse> IClientService.SendCodeRequestToClientAsync()
        {
            return _mapper.Map<ClientResponse>(
                await _telegramHelper.SendCodeRequestToClientAsync(
                    _mapper.Map<TelegramHelperVo>(_telegramConfiguration)));
        }

        async Task IClientService.UpdateCodeAsync(string phoneCodeHash, string code)
        {
            var telegramHelperVo = _mapper.Map<TelegramHelperVo>(_telegramConfiguration);
            telegramHelperVo.ConnectionVo = new ConnectionVo(phoneCodeHash, code);

            await _telegramHelper.UpdateCodeAsync(telegramHelperVo);
        }        
    }
}
