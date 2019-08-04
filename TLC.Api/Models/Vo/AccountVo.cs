namespace TLC.Api.Models.Vo
{
    public class AccountVo
    {
        public int Id { get; private set; }

        public string Hash { get; private set; }

        private AccountVo() { }

        public class Builder
        {
            private AccountVo _accountVo = new AccountVo();

            public Builder WithId(int id)
            {
                _accountVo.Id = id;
                return this;
            }

            public Builder WithHash(string hash)
            {
                _accountVo.Hash = hash;
                return this;
            }

            public AccountVo Build()
            {
                return _accountVo;
            }
        }
    }
}
