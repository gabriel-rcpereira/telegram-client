using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLC.Api.Models.Vo;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly Client _clientConfiguration;
        private readonly ITelegramHelper _telegramHelper;
        private readonly IMapper _mapper;

        public ClientService(IOptions<Client> clientConfiguration,
            ITelegramHelper telegramHelper,
            IMapper mapper)
        {
            _clientConfiguration = clientConfiguration.Value;
            _telegramHelper = telegramHelper;
            _mapper = mapper;
        }

        async Task IClientService.ForwardDailyMessageAsync()
        {
            await _telegramHelper.ForwardDailyMessageAsync(BuildTelegramHelperVo());
        }

        async Task<ClientResponse> IClientService.SendCodeRequestToClientAsync()
        {
            return _mapper.Map<ClientResponse>(
                await _telegramHelper.SendCodeRequestToClientAsync(BuildTelegramHelperVo()));
        }

        async Task IClientService.UpdateCodeAsync(string phoneCodeHash, string code)
        {
            await _telegramHelper.UpdateCodeAsync(BuildTelegramHelperVo(phoneCodeHash, code));
        }
        
        private static ClientResponse BuildClientResponse(TelegramCodeResponse telegramCodeResponse)
        {
            return new ClientResponse.Builder()
                .WithPhoneCodeHash(telegramCodeResponse.PhoneHashCode)
                .Build();
        }

        private TelegramHelperVo BuildTelegramHelperVo(string phoneCodeHash = "", string code = "")
        {
            return new TelegramHelperVo.Builder()
                .WithAccountVo(BuildAccountVo())
                .WithFromUserVo(BuildFromUserVo())
                .WithToUsersVo(BuildToUsersVo())
                .WithConnectionVo(BuildConnectionVo(phoneCodeHash, code))
                .Build();
        }

        private ConnectionVo BuildConnectionVo(string phoneCodeHash, string code)
        {
            return new ConnectionVo.Builder()
                .WithPhoneCodeHash(phoneCodeHash)
                .WithCode(code)
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
                .WithPhoneNumber(_clientConfiguration.Account.PhoneNumber)
                .Build();
        }
    }
}
