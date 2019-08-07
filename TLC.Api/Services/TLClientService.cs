using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
using TLC.Api.Services.Contracts;
using TLSchema;
using TLSchema.Messages;
using TLSharp;

namespace TLC.Api.Services
{
    public class TLClientService : ITLClientService
    {
        private const string EpochUnixTimeStamp = "01/01/1970 00:00:00";

        private readonly Client _client;

        public TLClientService(Client client)
        {
            _client = client;
        }

        async Task ITLClientService.ForwardDailyMessageAsync()
        {
            var client = NewClient(_client.Account.Id, _client.Account.Hash);
            await client.ConnectAsync();
            
            var messageSentToday = FilterLastMessageSentTodayAsync(_client.FromUser.Id,
                (TLDialogs)await client.GetUserDialogsAsync());

            if (messageSentToday != null)
            {
                _client.ToUsers.ToList()
                    .ForEach(user => 
                        client.SendMessageAsync(CreateUser(user.Id), 
                            messageSentToday.Message));
            }
        }

        async Task<IEnumerable<ContactResponse>> ITLClientService.FindContactsAsync()
        {
            var client = NewClient(_client.Account.Id, _client.Account.Hash);
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

        async Task ITLClientService.SendCodeRequestToClientAsync()
        {
            var client = NewClient(_client.Account.Id, _client.Account.Hash);
            await client.ConnectAsync();
            await client.SendCodeRequestAsync(_client.FromUser.PhoneNumber);
        }

        private ContactResponse CreateContactResponse(TLUser user)
        {
            return new ContactResponse.Builder()
                                    .WithId(user.Id)
                                    .WithFirstName(user.FirstName)
                                    .WithLastName(user.LastName)
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

        private TLMessage FilterLastMessageSentTodayAsync(int contactFromId, TLDialogs dialogs)
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
