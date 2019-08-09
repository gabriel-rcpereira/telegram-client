using System;

namespace TLC.Api.Models.Vo
{
    public class ConnectionVo
    {
        public string PhoneCodeHash { get; private set; }
        public string Code { get; private set; }

        public class Builder
        {
            private ConnectionVo _codeVo = new ConnectionVo();

            public Builder WithPhoneCodeHash(string phoneCodeHash)
            {
                _codeVo.PhoneCodeHash = phoneCodeHash;
                return this;
            }

            public Builder WithCode(string code)
            {
                _codeVo.Code = code;
                return this;
            }

            public ConnectionVo Build()
            {
                return _codeVo;
            }
        }
    }
}
