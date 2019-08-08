using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Models.Responses;
using TLC.Api.Services.Contracts;
using TLSchema;
using TLSchema.Messages;
using TLSharp;

namespace TLC.Api.Services
{
    public class ClientService : IClientService
    {
        private const string EpochUnixTimeStamp = "01/01/1970 00:00:00";

        private readonly Client _clientConfiguration;

        public ClientService(IOptions<Client> clientConfiguration)
        {
            _clientConfiguration = clientConfiguration.Value;
        }

        async Task IClientService.ForwardDailyMessageAsync()
        {
            var client = NewClient(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
            await client.ConnectAsync();
            
            var messageSentToday = FilterLastMessageSentToday(_clientConfiguration.FromUser.Id,
                (TLDialogs)await client.GetUserDialogsAsync());

            if (messageSentToday != null)
            {
                _clientConfiguration.ToUsers.ToList()
                    .ForEach(user => 
                        client.SendMessageAsync(CreateUser(user.Id), 
                            messageSentToday.Message));
            }
        }

        async Task<IEnumerable<ContactResponse>> IClientService.FindContactsAsync()
        {
            var client = NewClient(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
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

        async Task<ClientResponse> IClientService.SendCodeRequestToClientAsync()
        {
            var client = NewClient(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
            await client.ConnectAsync();
            string phoneCodeHash = await client.SendCodeRequestAsync(_clientConfiguration.Account.PhoneNumber);

            return CreateClientResponse(phoneCodeHash);
        }

        async Task IClientService.ReceiveCodeRequestedAsync(string phoneCodeHash, string code)
        {
            var client = NewClient(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
            await client.ConnectAsync();
            await client.MakeAuthAsync(_clientConfiguration.Account.PhoneNumber, phoneCodeHash, code);
        }

        private ContactResponse CreateContactResponse(TLUser user)
        {
            return new ContactResponse.Builder()
                                    .WithId(user.Id)
                                    .WithFirstName(user.FirstName)
                                    .WithLastName(user.LastName)
                                    .Build();
        }
        
        private static ClientResponse CreateClientResponse(string phoneCodeHash)
        {
            return new ClientResponse.Builder()
                            .WithPhoneCodeHash(phoneCodeHash)
                            .Build();
        }

        private TelegramClient NewClient(int id, string hash)
        {
            return new TelegramClient(id, hash);
        }

        private static TLInputPeerUser CreateUser(int userId)
        {
            return new TLInputPeerUser() { UserId = userId };
        }

        private TLMessage FilterLastMessageSentToday(int contactFromId, TLDialogs dialogs)
        {
            if (dialogs == null)
            {
                return null;
            }

            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            double yesterdayUnixTimestamp = DateTimeToUnixTimeStamp(yesterday);
            return dialogs.Messages
                .OfType<TLMessage>()
                .Where(message => message.FromId == contactFromId &&
                    message.Date > yesterdayUnixTimestamp)
                .FirstOrDefault();
        }

        private double DateTimeToUnixTimeStamp(DateTime date)
        {
            long ticks = date.Date.Ticks - DateTime.Parse(EpochUnixTimeStamp).Ticks;
            return Convert.ToDouble(ticks /= 10000000);
        }
    }
}
