using Newtonsoft.Json;

namespace TLC.Api.Configuration.Telegram
{
    public class User
    {
        public int Id { get; set; }

        public string PhoneNumber { get; internal set; }
    }
}
