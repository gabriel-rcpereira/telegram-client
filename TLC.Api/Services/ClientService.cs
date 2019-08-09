using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Factories.Contracts;
using TLC.Api.Helpers.Contracts;
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
        private readonly ITelegramClientFactory _telegramClientFactory;
        private readonly ITelegramHelper _telegramHelper;

        public ClientService(IOptions<Client> clientConfiguration,
            ITelegramClientFactory telegramClientFactory,
            ITelegramHelper telegramHelper)
        {
            _clientConfiguration = clientConfiguration.Value;
            _telegramClientFactory = telegramClientFactory;
            _telegramHelper = telegramHelper;
        }

        async Task IClientService.ForwardDailyMessageAsync()
        {
            var client = _telegramClientFactory.CreateTelegramClient(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
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

        async Task<ClientResponse> IClientService.SendCodeRequestToClientAsync()
        {
            var telegramCodeResponse = await _telegramHelper.SendCodeRequestToClientAsync(_clientConfiguration.Account.Id,
                _clientConfiguration.Account.Hash,
                _clientConfiguration.Account.PhoneNumber);

            return BuildClientResponse(telegramCodeResponse);
        }

        async Task IClientService.ReceiveCodeRequestedAsync(string phoneCodeHash, string code)
        {
            var client = _telegramClientFactory.CreateTelegramClient(_clientConfiguration.Account.Id, _clientConfiguration.Account.Hash);
            await client.ConnectAsync();
            await client.MakeAuthAsync(_clientConfiguration.Account.PhoneNumber, phoneCodeHash, code);
        }
        
        private static ClientResponse BuildClientResponse(TelegramCodeResponse telegramCodeResponse)
        {
            return new ClientResponse.Builder()
                .WithPhoneCodeHash(telegramCodeResponse.PhoneHashCode)
                .Build();
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
