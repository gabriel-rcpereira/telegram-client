using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Services
{
    public class ContactService : IContactService
    {
        private readonly ITelegramHelper _telegramHelper;
        private readonly Client _clientConfiguration;

        public ContactService(ITelegramHelper telegramHelper, 
            IOptions<Client> clientConfiguration)
        {
            _telegramHelper = telegramHelper;
            _clientConfiguration = clientConfiguration.Value;
        }

        async Task<IEnumerable<ContactResponse>> IContactService.FindContactsAsync()
        {
            var telegramContacts = await _telegramHelper
                .FindContactsAsync(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
            return telegramContacts.Select(contact => BuildContactResponse(contact));            
        }

        private ContactResponse BuildContactResponse(TelegramContactResponse telegramContactResponse)
        {
            return new ContactResponse.Builder()
                .WithId(telegramContactResponse.Id)
                .WithName(telegramContactResponse.Name)
                .WithType(telegramContactResponse.Type == Models.Enums.ContactType.Channel ? "Channel" : "Contact")
                .Build();
        }
    }
}
