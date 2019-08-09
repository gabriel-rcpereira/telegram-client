using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLC.Api.Models.Responses
{
    public class ContactResponse
    {
        public int Id { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private ContactResponse() { }

        public class Builder
        {
            private ContactResponse _contactResponse = new ContactResponse();

            public Builder WithId(int id)
            {
                _contactResponse.Id = id;
                return this;
            }

            public Builder WithFirstName(string firstName)
            {
                _contactResponse.FirstName = firstName;
                return this;
            }

            public Builder WithLastName(string lastName)
            {
                _contactResponse.LastName = lastName;
                return this;
            }

            public ContactResponse Build()
            {
                return _contactResponse;
            }
        }
    }
}
