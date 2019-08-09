namespace TLC.Api.Models.Responses
{
    public class TelegramContactResponse
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private TelegramContactResponse() { }

        public class Builder
        {
            private TelegramContactResponse _telegramContact = new TelegramContactResponse();

            public Builder WithId(int id)
            {
                _telegramContact.Id = id;
                return this;
            }

            public Builder WithFirstName(string firstName)
            {
                _telegramContact.FirstName = firstName;
                return this;
            }

            public Builder WithLastName(string lastName)
            {
                _telegramContact.LastName = lastName;
                return this;
            }

            public TelegramContactResponse Build()
            {
                return _telegramContact;
            }
        }
    }
}
