using Microsoft.Extensions.Logging;
using TLC.Api.Factories.Contracts;
using TLSharp;

namespace TLC.Api.Factories
{
    public class TelegramClientFactory : ITelegramClientFactory
    {
        private readonly ILogger _logger;

        public TelegramClientFactory(ILogger<TelegramClientFactory> logger)
        {
            _logger = logger;
        }

        TelegramClient ITelegramClientFactory.CreateClient(int appId, string appHash)
        {
            _logger.LogInformation("Creating a new Telegram Client instance.");
            return new TelegramClient(appId, appHash);
        }
    }
}
