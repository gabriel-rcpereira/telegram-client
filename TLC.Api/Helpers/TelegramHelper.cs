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

        async Task<IEnumerable<TelegramContactResponse>> ITelegramHelper.FindContactsAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Finding the contacts.");

            using (var client = await ConnectTelegramClientAsync(telegramHelperVo))
            {
                return (await GetContactsAsync(client)).Union(await GetContactsFromChatAsync(client));
            }
        }

        async Task<TelegramCodeResponse> ITelegramHelper.StartAuthenticationAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Starting authentication.");

            using (var client = await ConnectTelegramClientAsync(telegramHelperVo))
            {
                _logger.LogDebug($"Sending the code to phone. PhoneNumber: [{telegramHelperVo.Client.PhoneNumber}].");
                string phoneCodeHash = await client.SendCodeRequestAsync(telegramHelperVo.Client.PhoneNumber);
                return BuildTelegramCodeResponse(phoneCodeHash);
            }
        }
        
        async Task ITelegramHelper.ForwardDailyChannelMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Forwarding the daily message.");

            using (var client = await ConnectTelegramClientAsync(telegramHelperVo))
            {
                var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
                var lastMessage = FilterLastChannelMessageSentToday(telegramHelperVo.FromUser.Id, dialogs);

                if (String.IsNullOrEmpty(lastMessage.Message))
                {
                    _logger.LogDebug("There is no message to forward.");
                }
                else
                {
                    SendMessageAsync(telegramHelperVo.ToUsers, client, lastMessage);
                }
            }
        }

        async Task ITelegramHelper.MakeAuthenticationAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Making authentication.");

            using (var client = await ConnectTelegramClientAsync(telegramHelperVo))
            {
                await client.MakeAuthAsync(telegramHelperVo.Client.PhoneNumber,
                    telegramHelperVo.ConnectionVo.PhoneCodeHash,
                    telegramHelperVo.ConnectionVo.Code);
            }
        }

        async Task ITelegramHelper.ForwardLastMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Forwading last message.");

            using (var client = await ConnectTelegramClientAsync(telegramHelperVo))
            {
                var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
                var lastMessage = FilterLastChannelMessageSent(telegramHelperVo.FromUser.Id, dialogs);

                if (string.IsNullOrEmpty(lastMessage.Message))
                {
                    _logger.LogDebug("There is no message to forward.");
                }
                else
                {
                    SendMessageAsync(telegramHelperVo.ToUsers, client, lastMessage);
                }
            }
        }

        async Task ITelegramHelper.ForwardLastChannelMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Forwading last channel message.");

            var client = await ConnectTelegramClientAsync(telegramHelperVo);

            TLDialogs dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var lastMessage = FilterLastChannelMessageSent(telegramHelperVo.FromUser.Id, dialogs);

            if (lastMessage != null)
            {
                SendMessageAsync(telegramHelperVo.ToUsers, client, lastMessage);          
            }
        }

        private void SendMessageAsync(IEnumerable<UserVo> toUsers, TelegramClient client, TLMessage message)
        {
            _logger.LogInformation("Forwarding message filtered.");

            toUsers.ToList()
                .ForEach(user =>
                    client.SendMessageAsync(CreateUser(user.Id),
                        $"OkiBot --> {message.Message}"));
        }
        
        private TelegramClient NewClient(int id, string hash)
        {
            _logger.LogInformation("Creating a new Telegram Client.");
            TelegramClient telegramClient = null;
            try
            {
                telegramClient = new TelegramClient(id, hash);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"An error happened: {e.Message}", e);
            }
            return telegramClient;
        }

        private async Task<TelegramClient> ConnectTelegramClientAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);

            _logger.LogInformation("Connecting the Telegram Client.");
            await client.ConnectAsync();

            return client;
        }

        private static TLInputPeerChat CreateUser(int userId)
        {
            return new TLInputPeerChat() { ChatId = userId };
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
            _logger.LogInformation("Building telegram response.");

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

        private TLMessage FilterLastChannelMessageSentToday(int contactFromId, TLDialogs dialogs)
        {
            _logger.LogDebug("Appling the filter to get messages sent today.");

            IEnumerable<TLMessage> messages = FilterChannelMessages(contactFromId, dialogs);
            double yesterdayUnixTimestamp = GetYesterdayDateAsUnixTimestamp();
            return messages?.FirstOrDefault(message => message.Date >= yesterdayUnixTimestamp) ?? new TLMessage();
        }

        private double DateTimeToUnixTimeStamp(DateTime date)
        {
            long ticks = date.Date.Ticks - DateTime.Parse(EpochUnixTimeStamp).Ticks;
            return Convert.ToDouble(ticks /= 10000000);
        }

        private double GetSomeSecondsAgoAsUnixTimestamp()
        {
            DateTime someSecondsAgo = DateTime.UtcNow.Date.AddSeconds(-45);
            double someSecondsAgoUnixTimestamp = DateTimeToUnixTimeStamp(someSecondsAgo);
            _logger.LogDebug($"The original date [{someSecondsAgo}]. The date converted to UnixTimestamp [{someSecondsAgoUnixTimestamp}].");
            return someSecondsAgoUnixTimestamp;
        }

        private double GetYesterdayDateAsUnixTimestamp()
        {
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            double yesterdayUnixTimestamp = DateTimeToUnixTimeStamp(yesterday);
            _logger.LogDebug($"The original date [{yesterday}]. The date converted to UnixTimestamp [{yesterdayUnixTimestamp}].");
            return yesterdayUnixTimestamp;
        }

        private static IEnumerable<TLMessage> FilterChannelMessages(int contactFromId, TLDialogs dialogs)
        {
            return dialogs?.Messages
                .OfType<TLMessage>()
                .Where(message => message.ToId.GetType() == typeof(TLPeerChannel) &&
                    ((TLPeerChannel)message.ToId).ChannelId == contactFromId);
        }

        private TLMessage FilterLastChannelMessageSent(int contactFromId, TLDialogs dialogs)
        {
            double someSecondsAgoUnixTimestamp = GetSomeSecondsAgoAsUnixTimestamp();
            IEnumerable<TLMessage> messages = FilterChannelMessages(contactFromId, dialogs);
            return messages?.FirstOrDefault(message => message.Date > someSecondsAgoUnixTimestamp) ?? new TLMessage();
        }

        private async Task<IEnumerable<TelegramContactResponse>> GetContactsFromChatAsync(TelegramClient client)
        {
            _logger.LogInformation("Getting the contacts from dialogs chats.");

            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            return dialogs?.Chats
                    .OfType<TLChat>()
                    .Select(chat => BuildTelegramResponse(chat)) ?? new List<TelegramContactResponse>();
        }

        private async Task<IEnumerable<TelegramContactResponse>> GetContactsAsync(TelegramClient client)
        {
            _logger.LogDebug("Getting the contacts.");

            var contacts = await client.GetContactsAsync();
            return contacts?.Users
                .OfType<TLUser>()
                .Select(user => BuildTelegramResponse(user)) ?? new List<TelegramContactResponse>();
        }
    }
}
