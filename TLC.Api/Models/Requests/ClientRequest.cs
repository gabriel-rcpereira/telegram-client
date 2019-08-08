namespace TLC.Api.Models.Requests
{
    public class ClientRequest
    {
        public string Code { get; set; }

        public string PhoneCodeHash { get; internal set; }
    }
}
