using System;
using System.Collections.Generic;

namespace TLC.Api.Models.Vo
{
    public class TelegramHelperVo
    {
        public AccountVo AccountVo { get; private set; }
        public UserVo FromUserVo { get; private set; }
        public IEnumerable<UserVo> ToUsers { get; private set; }
        public ConnectionVo ConnectionVo { get; private set; }

        public class Builder
        {
            private TelegramHelperVo _telegramHelperVo = new TelegramHelperVo();

            public Builder WithAccountVo(AccountVo accountVo)
            {
                _telegramHelperVo.AccountVo = accountVo;
                return this;
            }

            public Builder WithFromUserVo(UserVo fromUserVo)
            {
                _telegramHelperVo.FromUserVo = fromUserVo;
                return this;
            }

            public Builder WithToUsersVo(IEnumerable<UserVo> toUsersVo)
            {
                _telegramHelperVo.ToUsers = toUsersVo;
                return this;
            }

            public Builder WithConnectionVo(ConnectionVo connectionVo)
            {
                _telegramHelperVo.ConnectionVo = connectionVo;
                return this;
            }

            public TelegramHelperVo Build()
            {
                return _telegramHelperVo;
            }
        }
    }
}
