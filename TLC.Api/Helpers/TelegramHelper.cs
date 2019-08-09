using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
using TLSchema;
using TLSchema.Messages;
using TLSharp;

namespace TLC.Api.Helpers
{
    public class TelegramHelper : ITelegramHelper
    {
        private const string EpochUnixTimeStamp = "01/01/1970 00:00:00";

        async Task<IEnumerable<TelegramContactResponse>> ITelegramHelper.FindContactsAsync(int id, string hash)
        {
            var client = NewClient(id, hash);
            await client.ConnectAsync();

            var contacts = await client.GetContactsAsync();
            if (contacts != null)
            {
                return contacts.Users
                    .OfType<TLUser>()
                    .Select(user => BuildTelegramResponse(user));
            }

            return new List<TelegramContactResponse>();
        }

        async Task<TelegramCodeResponse> ITelegramHelper.SendCodeRequestToClientAsync(int id, string hash, string phoneNumber)
        {
            var client = NewClient(id, hash);
            await client.ConnectAsync();
            return BuildTelegramCodeResponse(await client.SendCodeRequestAsync(phoneNumber));
        }

        async Task ITelegramHelper.ForwardDailyMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.AccountVo.Id, telegramHelperVo.AccountVo.Hash);
            await client.ConnectAsync();

            var messageSentToday = FilterLastMessageSentToday(telegramHelperVo.FromUserVo.Id,
                (TLDialogs)await client.GetUserDialogsAsync());

            if (messageSentToday != null)
            {
                telegramHelperVo.ToUsers.ToList()
                    .ForEach(user =>
                        client.SendMessageAsync(CreateUser(user.Id),
                            messageSentToday.Message));
            }
        }

        private TelegramCodeResponse BuildTelegramCodeResponse(string phoneCodeHash)
        {
            return new TelegramCodeResponse.Builder()
                .WithPhoneHashCode(phoneCodeHash)
                .Build();
        }

        private TelegramContactResponse BuildTelegramResponse(TLUser user)
        {
            return new TelegramContactResponse.Builder()
                .WithId(user.Id)
                .WithFirstName(user.FirstName)
                .WithLastName(user.LastName)
                .Build();
        }

        private static TLInputPeerUser CreateUser(int userId)
        {
            return new TLInputPeerUser() { UserId = userId };
        }

        private TelegramClient NewClient(int id, string hash)
        {
            return new TelegramClient(id, hash);
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

        async Task ITelegramHelper.UpdateCodeAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.AccountVo.Id, telegramHelperVo.AccountVo.Hash);
            await client.ConnectAsync();
            await client.MakeAuthAsync(telegramHelperVo.AccountVo.PhoneNumber, 
                telegramHelperVo.ConnectionVo.PhoneCodeHash, 
                telegramHelperVo.ConnectionVo.Code);
        }
    }
}
