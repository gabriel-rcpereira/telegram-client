using TLSharp;

namespace TLC.Api.Factories.Contracts
{
    public interface ITelegramClientFactory
    {
        TelegramClient CreateClient(int appId, string appHash);
    }
}