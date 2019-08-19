﻿using System.Collections.Generic;

namespace TLC.Api.Configuration.Telegram
{
    public class TelegramConfiguration
    {
        public ClientConfiguration Client { get; set; }

        public IEnumerable<UserConfiguration> ToUsers { get; set; }

        public UserConfiguration FromUser { get; set; }
    }
}
