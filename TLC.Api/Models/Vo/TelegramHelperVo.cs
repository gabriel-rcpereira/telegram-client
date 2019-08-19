using System;
using System.Collections.Generic;

namespace TLC.Api.Models.Vo
{
    public class TelegramHelperVo
    {
        public ClientVo Client { get; set; }
        public UserVo FromUser { get; set; }
        public IEnumerable<UserVo> ToUsers { get; set; }
        public ConnectionVo ConnectionVo { get; set; }
    }
}
