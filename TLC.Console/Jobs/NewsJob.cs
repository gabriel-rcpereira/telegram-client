using AutoMapper;
using Microsoft.Extensions.Options;
using Quartz;
using System.Threading.Tasks;
using TLC.Business.Helpers.Contracts;
using TLC.Business.Models.Vo;
using TLC.Console.Configuration.Telegram;

namespace TLC.Console.Jobs
{
    public class NewsJob : IJob
    {
        private readonly ITelegramHelper _telegramHelper;
        private readonly TelegramConfiguration _telegramConfiguration;
        private readonly IMapper _mapper;
        
        public NewsJob(ITelegramHelper telegramHelper,
            IOptions<TelegramConfiguration> telegramConfiguration,
            IMapper mapper)
        {
            _telegramHelper = telegramHelper;
            _telegramConfiguration = telegramConfiguration.Value;
            _mapper = mapper;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            await _telegramHelper.ForwardLastMessageAsync(_mapper.Map<TelegramHelperVo>(_telegramConfiguration));
        }
    }
}
