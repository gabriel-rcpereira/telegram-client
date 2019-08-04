using Newtonsoft.Json;
using System.Collections.Generic;
using TLC.Api.Models.Requests;

namespace TLC.Api.Models.Request
{
    public class TLClientRequest
    {
        [JsonProperty("account")]
        public AccountRequest Account { get; set; }

        [JsonProperty("toUsers")]
        public IEnumerable<UserRequest> ToUsers { get; set; }

        [JsonProperty("fromUser")]
        public UserRequest FromUser { get; set; }
    }
}
