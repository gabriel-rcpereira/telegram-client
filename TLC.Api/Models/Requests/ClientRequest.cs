﻿using System.ComponentModel.DataAnnotations;

namespace TLC.Api.Models.Requests
{
    public class ClientRequest
    {
        public string Code { get; set; }
        public string PhoneCodeHash { get; set; }
    }
}
