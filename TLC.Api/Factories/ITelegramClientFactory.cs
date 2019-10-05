using TLSharp;

namespace TLC.Api.Factories
{
    public interface ITelegramClientFactory
    {
        TelegramClient CreateClient(int appId, string appHash);
    }
}