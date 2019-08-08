using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Factories.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Services.Contracts;
using TLSchema;

namespace TLC.Api.Services
{
    public class ContactService : IContactService
    {
        private readonly ITelegramClientFactory _telegramClientFactory;
        private readonly Client _clientConfiguration;

        public ContactService(ITelegramClientFactory telegramClientFactory, 
            IOptions<Client> clientConfiguration)
        {
            _telegramClientFactory = telegramClientFactory;
            _clientConfiguration = clientConfiguration.Value;
        }

        async Task<IEnumerable<ContactResponse>> IContactService.FindContactsAsync()
        {
            var client = _telegramClientFactory.CreateTelegramClient(_clientConfiguration.Account.Id, 
                _clientConfiguration.Account.Hash);
            await client.ConnectAsync();

            var contacts = await client.GetContactsAsync();
            if (contacts != null)
            {
                return contacts.Users
                    .OfType<TLUser>()
                    .Select(user => CreateContactResponse(user));
            }

            return new List<ContactResponse>();
        }

        private ContactResponse CreateContactResponse(TLUser user)
        {
            return new ContactResponse.Builder()
                                    .WithId(user.Id)
                                    .WithFirstName(user.FirstName)
                                    .WithLastName(user.LastName)
                                    .Build();
        }

    }
}
