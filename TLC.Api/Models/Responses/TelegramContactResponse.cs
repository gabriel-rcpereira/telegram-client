using TLC.Api.Models.Enums;

namespace TLC.Api.Models.Responses
{
    public class TelegramContactResponse
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public ContactType Type { get; private set; }

        private TelegramContactResponse() { }

        public class Builder
        {
            private TelegramContactResponse _telegramContact = new TelegramContactResponse();

            public Builder WithId(int id)
            {
                _telegramContact.Id = id;
                return this;
            }

            public Builder WithName(string Name)
            {
                _telegramContact.Name = Name;
                return this;
            }

            public Builder WithType(ContactType type)
            {
                _telegramContact.Type = type;
                return this;
            }

            public TelegramContactResponse Build()
            {
                return _telegramContact;
            }
        }
    }
}
