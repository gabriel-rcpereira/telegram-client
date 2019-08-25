using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Business.Helpers.Contracts;
using TLC.Business.Models.Enums;
using TLC.Business.Models.Responses;
using TLC.Business.Models.Vo;
using TLSchema;
using TLSchema.Messages;
using TLSharp;

namespace TLC.Business.Helpers
{
    public class TelegramHelper : ITelegramHelper
    {
        private const string EpochUnixTimeStamp = "31/12/1969 21:00:00";
        
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
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);
            await client.ConnectAsync();
            return BuildTelegramCodeResponse(await client.SendCodeRequestAsync(telegramHelperVo.Client.PhoneNumber));
        }

        async Task ITelegramHelper.ForwardDailyMessageAsync(TelegramHelperVo telegramHelperVo)
        {
            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);
            await client.ConnectAsync();

            var messageSentToday = FilterLastMessageSentToday(telegramHelperVo.FromUser.Id,
                (TLDialogs)await client.GetUserDialogsAsync());

            if (messageSentToday == null)
            {
                return;
            }
            
            telegramHelperVo.ToUsers.ToList()
                .ForEach(user =>
                    client.SendMessageAsync(CreateUser(user.Id),
                        messageSentToday.Message));
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
            Console.WriteLine("Starting the forwarding message method.");

            var client = NewClient(telegramHelperVo.Client.Id, telegramHelperVo.Client.Hash);
            await client.ConnectAsync();

            try
            {
                var lastMessage = FilterLastMessageSent(telegramHelperVo.FromUser.Id,
                    (TLDialogs)await client.GetUserDialogsAsync());

                if (lastMessage == null)
                {
                    Console.WriteLine("Message was not found.");
                    return;
                }
                Console.WriteLine("Message was found.");
                telegramHelperVo.ToUsers.ToList()
                    .ForEach(user =>
                        client.SendMessageAsync(CreateUser(user.Id),
                            $"Hi, OkiBot here! --> {lastMessage.Message}"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error {0}", e.Message);
                throw;
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

        private static TLInputPeerChat CreateUser(int chatId)
        {
            return new TLInputPeerChat() { ChatId = chatId };
        }

        private TelegramClient NewClient(int id, string hash)
        {
            return new TelegramClient(id, hash);
        }

        private TLMessage FilterLastMessageSentToday(int channelId, TLDialogs dialogs)
        {
            if (dialogs == null)
            {
                return null;
            }

            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            double yesterdayUnixTimestamp = DateTimeToUnixTimeStamp(yesterday);
            return dialogs.Messages
                .OfType<TLMessage>()
                ?.Where(message => message.Date > yesterdayUnixTimestamp)
                ?.Where(message => (message.ToId as TLPeerChannel).ChannelId == channelId)
                ?.FirstOrDefault();
        }

        private TLMessage FilterLastMessageSent(int channelId, TLDialogs dialogs)
        {
            if (dialogs == null || dialogs.Messages == null)
            {
                return null;
            }

            DateTime someSecondsAgo = DateTime.Now.AddSeconds(-30);
            double someSecondsAgoUnixTimestamp = DateTimeToUnixTimeStamp(someSecondsAgo);
            return dialogs.Messages
                ?.OfType<TLMessage>()
                ?.Where(messages => messages.ToId == null)
                ?.Where(message => message.Date >= someSecondsAgoUnixTimestamp)
                ?.Where(message => (message.ToId as TLPeerChannel).ChannelId == channelId)
                ?.FirstOrDefault();
        }

        private double DateTimeToUnixTimeStamp(DateTime date)
        {
            long ticks = date.Ticks - DateTime.Parse(EpochUnixTimeStamp).Ticks;
            return Convert.ToDouble(ticks /= 10000000);
        }
    }
}
