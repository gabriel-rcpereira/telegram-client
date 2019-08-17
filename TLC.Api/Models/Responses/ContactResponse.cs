using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLC.Api.Models.Responses
{
    public class ContactResponse
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        private ContactResponse() { }

        public class Builder
        {
            private ContactResponse _contactResponse = new ContactResponse();

            public Builder WithId(int id)
            {
                _contactResponse.Id = id;
                return this;
            }

            public Builder WithName(string name)
            {
                _contactResponse.Name = name;
                return this;
            }

            public Builder WithType(string type)
            {
                _contactResponse.Type = type;
                return this;
            }

            public ContactResponse Build()
            {
                return _contactResponse;
            }
        }
    }
}
