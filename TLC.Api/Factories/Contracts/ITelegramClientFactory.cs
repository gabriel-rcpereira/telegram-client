using TLSharp;

namespace TLC.Api.Factories.Contracts
{
    public interface ITelegramClientFactory
    {
        TelegramClient CreateTelegramClient(int id, string hash);
    }
}
