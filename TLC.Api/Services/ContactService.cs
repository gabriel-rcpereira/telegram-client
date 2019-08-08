using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Services
{
    public class ContactService : IContactService
    {
        Task<IEnumerable<ContactResponse>> IContactService.FindContactsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
