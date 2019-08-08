using TLC.Api.Factories.Contracts;
using TLSharp;

namespace TLC.Api.Factories
{
    public class TelegramClientFactory : ITelegramClientFactory
    {
        TelegramClient ITelegramClientFactory.CreateTelegramClient(int id, string hash)
        {
            return new TelegramClient(id, hash);
        }
    }
}
