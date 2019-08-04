using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private TelegramClient _client;

        public TLClientService() { }

        async Task ITLClientService.ForwardTodayMessageAsync(ClientVo clientVo)
        {
            _client = NewClient(clientVo.Account.Id, clientVo.Account.Hash);
            await _client.ConnectAsync();

            var messageSentToday = await GetLastMessageSentTodayAsync(clientVo.FromUser.Id);

            if (messageSentToday != null)
            {
                clientVo.ToUsers.ToList()
                    .ForEach(user => 
                        _client.SendMessageAsync(CreateUser(user.Id), 
                            messageSentToday.Message));
            }
        }

        async Task<IEnumerable<ContactResponse>> ITLClientService.GetContacts(ClientVo clientVo)
        {
            _client = NewClient(clientVo.Account.Id, clientVo.Account.Hash);
            await _client.ConnectAsync();

            var contacts = await _client.GetContactsAsync();
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

        private TelegramClient NewClient(int id, string hash)
        {
            return new TelegramClient(id, hash);
        }

        private static TLInputPeerUser CreateUser(int userId)
        {
            return new TLInputPeerUser() { UserId = userId };
        }

        private async Task<TLMessage> GetLastMessageSentTodayAsync(int contactFromId)
        {
            var dialogs = (TLDialogs)await _client.GetUserDialogsAsync();

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
