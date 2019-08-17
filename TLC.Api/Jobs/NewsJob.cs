using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Vo;

namespace TLC.Api.Jobs
{
    public class NewsJob : IJob, IJobDetail
    {
        private readonly ITelegramHelper _telegramHelper;
        private readonly Client _clientConfiguration;
        
        public NewsJob(ITelegramHelper telegramHelper)
        {
            _telegramHelper = telegramHelper;
        }

        JobKey IJobDetail.Key => new JobKey("newsJob", "telegramGroup");

        string IJobDetail.Description => "It job runs each 45 seconds";

        Type IJobDetail.JobType => typeof(NewsJob);

        JobDataMap IJobDetail.JobDataMap => null;

        bool IJobDetail.Durable => false;

        bool IJobDetail.PersistJobDataAfterExecution => false;

        bool IJobDetail.ConcurrentExecutionDisallowed => true;

        bool IJobDetail.RequestsRecovery => false);
        
        async Task IJob.Execute(IJobExecutionContext context)
        {
            await _telegramHelper.ForwardLastMessageAsync(BuildTelegramHelperVo());
        }

        private TelegramHelperVo BuildTelegramHelperVo()
        {
            return new TelegramHelperVo.Builder()
                .WithAccountVo(BuildAccountVo())
                .WithFromUserVo(BuildFromUserVo())
                .WithToUsersVo(BuildToUsersVo())
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

        JobBuilder IJobDetail.GetJobBuilder()
        {
            throw new NotImplementedException();
        }

        IJobDetail IJobDetail.Clone()
        {
            throw new NotImplementedException();
        }
    }
}
