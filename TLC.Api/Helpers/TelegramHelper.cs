using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Models.Responses;
using TLSchema;
using TLSharp;

namespace TLC.Api.Helpers
{
    public class TelegramHelper : ITelegramHelper
    {
        async Task<IEnumerable<TelegramContactResponse>> ITelegramHelper.FindContactsAsync(int id, string hash)
        {
            var client = NewClient(id, hash);
            await client.ConnectAsync();

            var contacts = await client.GetContactsAsync();
            if (contacts != null)
            {
                return contacts.Users
                    .OfType<TLUser>()
                    .Select(user => BuildTelegramResponse(user));
            }

            return new List<TelegramContactResponse>();
        }

        private TelegramContactResponse BuildTelegramResponse(TLUser user)
        {
            return new TelegramContactResponse.Builder()
                .WithId(user.Id)
                .WithFirstName(user.FirstName)
                .WithLastName(user.LastName)
                .Build();
        }

        private TelegramClient NewClient(int id, string hash)
        {
            return new TelegramClient(id, hash);
        }
    }
}
