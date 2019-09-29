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
using TLSchema.Channels;
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
            var client = await ConnectTelegramClientAsync(telegramHelperVo);

            IEnumerable<TelegramContactResponse> telegramContactsResponse = new List<TelegramContactResponse>();

            _logger.LogInformation("Getting the contacts.");
            var contacts = await client.GetContactsAsync();
            if (contacts != null)
            {
                telegramContactsResponse = contacts.Users
                    .OfType<TLUser>()
                    .Select(user => BuildTelegramResponse(user));
            }

            _logger.LogInformation("Getting the dialogs messages.");
            var dialogs = (TLDialogs) await client.GetUserDialogsAsync();
            if (dialogs != null)
            {
                _logger.LogInformation("Filtering the dialogs.");
                telegramContactsResponse = telegramContactsResponse.Union(dialogs.Chats
                    .OfType<TLChat>()
                    .Select(chat => BuildTelegramResponse(chat)));

                //var channelMessages = dialogs.Messages
                //    .OfType<TLMessage>()
                //    ?.Where(message => message.ToId.GetType() == typeof(TLPeerChannel))
                //    .Select(message => message.id);


                //TODO - implement the list of channels
                //var channelsDialog = dialogs.Messages.OfType<TLMessage>().Where(m => m.ToId.GetType() == typeof(TLPeerChannel));
                //if (channelsDialog != null)
                //{
                //    telegramContactsResponse = telegramContactsResponse.Union(channelsDialog.Select(message => BuildTelegramResponse(message)));
                //}
            }

            return telegramContactsResponse;
        }

        async Task<TelegramCodeResponse> ITelegramHelper.SendCodeRequestToClientAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Sending code request.");

            var client = await ConnectTelegramClientAsync(telegramHelperVo);

            _logger.LogInformation($"Sending the code to the phone. PhoneNumber: [{telegramHelperVo.Client.PhoneNumber}].");
            return BuildTelegramCodeResponse(await client.SendCodeRequestAsync(telegramHelperVo.Client.PhoneNumber));
        }
        
        async Task ITelegramHelper.ForwardDailyChannelMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Forwarding the daily message.");

            var client = await ConnectTelegramClientAsync(telegramHelperVo);

            _logger.LogInformation("Getting the dialog messages.");
            TLDialogs dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var lastMessage = FilterLastChannelMessageSentToday(telegramHelperVo.FromUser.Id, dialogs);

            if (lastMessage != null)
            {
                SendMessageAsync(telegramHelperVo.ToUsers, client, lastMessage);
            }
        }

        async Task ITelegramHelper.UpdateCodeAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Updating the code.");

            var client = await ConnectTelegramClientAsync(telegramHelperVo);

            _logger.LogInformation("Making the authentication.");
            await client.MakeAuthAsync(telegramHelperVo.Client.PhoneNumber,
                telegramHelperVo.ConnectionVo.PhoneCodeHash,
                telegramHelperVo.ConnectionVo.Code);
        }

        async Task ITelegramHelper.ForwardLastMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            _logger.LogInformation("Forwading last message.");

            var client = await ConnectTelegramClientAsync(telegramHelperVo);

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

        private static void SendMessageAsync(IEnumerable<UserVo> toUsers, TelegramClient client, TLMessage message)
        {
            toUsers.ToList()
                .ForEach(user =>
                    client.SendMessageAsync(CreateUser(user.Id),
                        $"OkiBot --> {message.Message}"));
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

        private static TLInputPeerChat CreateUser(int userId)
        {
            return new TLInputPeerChat() { ChatId = userId };
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

        private TLMessage FilterLastChannelMessageSentToday(int contactFromId, TLDialogs dialogs)
        {
            _logger.LogInformation("Filtering the dialog messages.");

            if (dialogs == null || dialogs.Messages == null)
            {
                _logger.LogInformation("There is not message.");
                return null;
            }

            _logger.LogInformation("Applying the filter to get the last message sent today.");
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            double yesterdayUnixTimestamp = DateTimeToUnixTimeStamp(yesterday);
            return dialogs.Messages
                .OfType<TLMessage>()
                .FirstOrDefault(message => message.ToId.GetType() == typeof(TLPeerChannel) &&
                    ((TLPeerChannel)message.ToId).ChannelId == contactFromId);
        }

        private TLMessage FilterLastMessageSent(int contactFromId, TLDialogs dialogs)
        {
            _logger.LogInformation("Filtering last message sent.");

            if (dialogs == null || dialogs.Messages == null)
            {
                _logger.LogInformation("There is not message.");
                return null;
            }

            _logger.LogInformation("Applying the filter.");
            DateTime someSecondsAgo = DateTime.UtcNow.Date.AddSeconds(-45);
            double someSecondsAgoUnixTimestamp = DateTimeToUnixTimeStamp(someSecondsAgo);
            _logger.LogInformation($"Some seconds ago date [{someSecondsAgo}]. Some seconds ago UnixTimestamp [{someSecondsAgoUnixTimestamp}].");
            return dialogs.Messages
                .OfType<TLMessage>()
                .FirstOrDefault(message => message.FromId == contactFromId &&
                    message.Date > someSecondsAgoUnixTimestamp);
        }

        private TLMessage FilterLastChannelMessageSent(int contactFromId, TLDialogs dialogs)
        {
            _logger.LogInformation("Filtering last message sent.");

            if (dialogs == null || dialogs.Messages == null)
            {
                _logger.LogInformation("There is not message.");
                return null;
            }

            _logger.LogInformation("Applying the filter.");
            DateTime someSecondsAgoDate = DateTime.UtcNow.Date.AddSeconds(-45);
            double someSecondsAgoUnixTimestamp = DateTimeToUnixTimeStamp(someSecondsAgoDate);

            _logger.LogInformation($"Some seconds ago date [{someSecondsAgoDate}]. Some seconds ago UnixTimestamp [{someSecondsAgoUnixTimestamp}].");

            return dialogs.Messages
                .OfType<TLMessage>()
                .FirstOrDefault(message => message.ToId.GetType() == typeof(TLPeerChannel) &&
                    ((TLPeerChannel)message.ToId).ChannelId == contactFromId &&
                    message.Date > someSecondsAgoUnixTimestamp);
        }

        private double DateTimeToUnixTimeStamp(DateTime date)
        {
            long ticks = date.Date.Ticks - DateTime.Parse(EpochUnixTimeStamp).Ticks;
            return Convert.ToDouble(ticks /= 10000000);
        }
    }
}
