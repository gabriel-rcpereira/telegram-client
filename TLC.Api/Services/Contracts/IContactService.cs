using System.Collections.Generic;
using System.Threading.Tasks;
using TLC.Api.Models.Responses;

namespace TLC.Api.Services.Contracts
{
    public interface IContactService
    {
        Task<IEnumerable<ContactResponse>> FindContactsAsync();
    }
}
