namespace TLC.Api.Models.Responses
{
    public class ClientResponse
    {
        public string PhoneCodeHash { get; private set; }

        private ClientResponse() { }

        public class Builder
        {
            private ClientResponse _clientResponse = new ClientResponse();

            public Builder WithPhoneCodeHash(string phoneCodeHash)
            {
                _clientResponse.PhoneCodeHash = phoneCodeHash;
                return this;
            }

            public ClientResponse Build()
            {
                return _clientResponse;
            }
        }
    }
}