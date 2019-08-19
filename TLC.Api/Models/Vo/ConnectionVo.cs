using System;

namespace TLC.Api.Models.Vo
{
    public class ConnectionVo
    {
        public string PhoneCodeHash { get; private set; }
        public string Code { get; private set; }

        public ConnectionVo(string phoneCodeHash, string code)
        {
            PhoneCodeHash = phoneCodeHash;
            Code = code;
        }
    }
}
