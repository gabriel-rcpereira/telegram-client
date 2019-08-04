using System.Collections.Generic;

namespace TLC.Api.Models.Vo
{
    public class ClientVo
    {
        public AccountVo Account { get; private set; }

        public UserVo FromUser { get; private set; }

        public IEnumerable<UserVo> ToUsers { get; private set; }

        private ClientVo(){ }

        public class Builder
        {
            private ClientVo _clientVo = new ClientVo();
            
            public Builder WithAccount(AccountVo account)
            {
                _clientVo.Account = account;
                return this;
            }

            public Builder WithFromUser(UserVo fromUser)
            {
                _clientVo.FromUser = fromUser;
                return this;
            }

            public Builder WithToUsers(IEnumerable<UserVo> toUsers)
            {
                _clientVo.ToUsers = toUsers;
                return this;
            }

            public ClientVo Build()
            {
                return _clientVo;
            }
        }
    }
}
