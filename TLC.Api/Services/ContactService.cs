using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configurations.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Services
{
    public class ContactService : IContactService
    {
        private readonly ITelegramHelper _telegramHelper;
        private readonly TelegramConfiguration _telegramConfiguration;
        private readonly IMapper _mapper;

        public ContactService(ITelegramHelper telegramHelper, 
            IOptions<TelegramConfiguration> telegramConfiguration,
            IMapper mapper)
        {
            _telegramHelper = telegramHelper;
            _telegramConfiguration = telegramConfiguration.Value;
            _mapper = mapper;
        }

        async Task<IEnumerable<ContactResponse>> IContactService.FindContactsAsync()
        {
            var telegramHelperVo = _mapper.Map<TelegramHelperVo>(_telegramConfiguration);
            return (await _telegramHelper
                .FindContactsAsync(telegramHelperVo))
                .Select(contact => _mapper.Map<ContactResponse>(contact));            
        }        
    }
}
