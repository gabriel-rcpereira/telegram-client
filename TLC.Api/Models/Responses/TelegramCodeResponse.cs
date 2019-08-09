namespace TLC.Api.Models.Responses
{
    public class TelegramCodeResponse
    {
        public string PhoneHashCode { get; private set; }

        private TelegramCodeResponse() { }

        public class Builder
        {
            private TelegramCodeResponse _telegramCodeResponse = new TelegramCodeResponse();

            public Builder WithPhoneHashCode(string phoneHashCode)
            {
                _telegramCodeResponse.PhoneHashCode = phoneHashCode;
                return this;
            }

            public TelegramCodeResponse Build()
            {
                return _telegramCodeResponse;
            }
        }
    }
}
