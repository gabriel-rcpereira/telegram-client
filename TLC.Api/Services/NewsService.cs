using AutoMapper;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TLC.Api.Configurations.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Vo;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Services
{
    public class NewsService : INewService
    {
        private readonly ITelegramHelper _telegramHelper;
        private readonly TelegramConfiguration _telegramConfiguration;
        private readonly IMapper _mapper;

        public NewsService(ITelegramHelper telegramHelper,
            IOptions<TelegramConfiguration> telegramConfiguration,
            IMapper mapper)
        {
            _telegramHelper = telegramHelper;
            _telegramConfiguration = telegramConfiguration.Value;
            _mapper = mapper;
        }

        Task INewService.Execute()
        {
            TelegramHelperVo telegramHelperVo = _mapper.Map<TelegramHelperVo>(_telegramConfiguration);
            return _telegramHelper.ForwardLastMessageAsync(telegramHelperVo);
        }
    }
}
