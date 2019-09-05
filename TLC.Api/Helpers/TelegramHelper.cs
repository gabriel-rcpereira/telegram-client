using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Enums;
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

        private readonly ILogger _logger;

        public TelegramHelper(ILogger<TelegramHelper> logger)
        {
            _logger = logger;
        }

        async Task<IEnumerable<TelegramContactResponse>> ITelegramHelper.FindContactsAsync(int id, string hash)
        {
            var client = NewClient(id, hash);
            await client.ConnectAsync();

            IEnumerable<TelegramContactResponse> telegramContacstResponse = new List<TelegramContactResponse>();

            var contacts = await client.GetContactsAsync();
            if (contacts != null)
            {
                telegramContacstResponse = contacts.Users
                    .OfType<TLUser>()
                    .Select(user => BuildTelegramResponse(user));
            }

            var dialogs = (TLDialogs) await client.GetUserDialogsAsync();
            if (dialogs != null)
            {
                telegramContacstResponse = telegramContacstResponse.Union(dialogs.Chats
                    .OfType<TLChat>()
                    .Select(chat => BuildTelegramResponse(chat)));
            }

            return telegramContacstResponse;
        }

        async Task<TelegramCodeResponse> ITelegramHelper.SendCodeRequestToClientAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = await GetTelegramClient(telegramHelperVo);

            _logger.LogInformation($"Sending the code to the phone. PhoneNumber: [{telegramHelperVo.Client.PhoneNumber}].");
            return BuildTelegramCodeResponse(await client.SendCodeRequestAsync(telegramHelperVo.Client.PhoneNumber));
        }
        
        async Task ITelegramHelper.ForwardDailyMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);
            await client.ConnectAsync();

            var messageSentToday = FilterLastMessageSentToday(telegramHelperVo.FromUser.Id,
                (TLDialogs)await client.GetUserDialogsAsync());

            if (messageSentToday != null)
            {
                telegramHelperVo.ToUsers.ToList()
                    .ForEach(user =>
                        client.SendMessageAsync(CreateUser(user.Id),
                            messageSentToday.Message));
            }
        }

        async Task ITelegramHelper.UpdateCodeAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);
            await client.ConnectAsync();
            await client.MakeAuthAsync(telegramHelperVo.Client.PhoneNumber,
                telegramHelperVo.ConnectionVo.PhoneCodeHash,
                telegramHelperVo.ConnectionVo.Code);
        }

        async Task ITelegramHelper.ForwardLastMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);
            await client.ConnectAsync();

            var lastMessage = FilterLastMessageSent(telegramHelperVo.FromUser.Id,
                (TLDialogs)await client.GetUserDialogsAsync());

            if (lastMessage != null)
            {
                telegramHelperVo.ToUsers.ToList()
                    .ForEach(user =>
                        client.SendMessageAsync(CreateUser(user.Id),
                            $"OkiBot --> {lastMessage.Message}"));
            }
        }

        private TelegramCodeResponse BuildTelegramCodeResponse(string phoneCodeHash)
        {
            return new TelegramCodeResponse()
            {
                PhoneHashCode = phoneCodeHash
            };
        }

        private TelegramContactResponse BuildTelegramResponse(TLUser user)
        {
            return new TelegramContactResponse.Builder()
                .WithId(user.Id)
                .WithName($"{user.FirstName} {user.LastName}")
                .WithType(ContactType.Contact)
                .Build();
        }

        private TelegramContactResponse BuildTelegramResponse(TLChat chat)
        {
            return new TelegramContactResponse.Builder()
                .WithId(chat.Id)
                .WithName(chat.Title)
                .WithType(ContactType.Channel)
                .Build();
        }

        private static TLInputPeerUser CreateUser(int userId)
        {
            return new TLInputPeerUser() { UserId = userId };
        }

        private TelegramClient NewClient(int id, string hash)
        {
            _logger.LogInformation("Creating a new Telegram Client.");
            return new TelegramClient(id, hash);
        }

        private async Task<TelegramClient> GetTelegramClient(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);

            _logger.LogInformation("Connecting the Telegram Client.");
            await client.ConnectAsync();

            return client;
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

        private TLMessage FilterLastMessageSent(int contactFromId, TLDialogs dialogs)
        {
            if (dialogs == null)
            {
                return null;
            }

            DateTime yesterday = DateTime.UtcNow.Date.AddSeconds(-45);
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
