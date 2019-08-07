using System.Collections.Generic;

namespace TLC.Api.Configuration.Telegram
{
    public class Client
    {
        public Account Account { get; set; }

        public IEnumerable<User> ToUsers { get; set; }

        public User FromUser { get; set; }
    }
}
