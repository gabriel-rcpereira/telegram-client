using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Factories.Contracts;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
using TLC.Api.Services.Contracts;
using TLSchema;
using TLSchema.Messages;
using TLSharp;

namespace TLC.Api.Services
{
    public class ClientService : IClientService
    {
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
            await _telegramHelper.ForwardDailyMessageAsync(BuildTelegramHelperVo());
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

        private TelegramHelperVo BuildTelegramHelperVo()
        {
            return new TelegramHelperVo.Builder()
                .WithAccountVo(BuildAccountVo())
                .WithFromUserVo(BuildFromUserVo())
                .WithToUsers(BuildToUsersVo())
                .Build();
        }

        private IEnumerable<UserVo> BuildToUsersVo()
        {
            return _clientConfiguration.ToUsers.Select(user =>
                BuildUserVo(user));
        }

        private static UserVo BuildUserVo(User users)
        {
            return new UserVo.Builder()
                                .WithId(users.Id)
                                .Build();
        }

        private UserVo BuildFromUserVo()
        {
            return BuildUserVo(_clientConfiguration.FromUser);
        }

        private AccountVo BuildAccountVo()
        {
            return new AccountVo.Builder()
                .WithId(_clientConfiguration.Account.Id)
                .WithHash(_clientConfiguration.Account.Hash)
                .Build();
        }
    }
}
