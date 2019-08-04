using Newtonsoft.Json;

namespace TLC.Api.Models.Requests
{
    public class AccountRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}
