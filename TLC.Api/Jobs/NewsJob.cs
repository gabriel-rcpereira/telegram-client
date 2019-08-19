using AutoMapper;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Vo;

namespace TLC.Api.Jobs
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
