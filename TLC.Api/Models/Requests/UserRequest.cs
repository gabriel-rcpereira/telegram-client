using Newtonsoft.Json;

namespace TLC.Api.Models.Requests
{
    public class UserRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
