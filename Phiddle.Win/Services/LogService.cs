using Phiddle.Core.Services;

namespace Phiddle.Win.Services
{
    public class LoggingService : LoggingServiceConsole
    {
        public LoggingService()
        {
            Owner = "Phiddle.Win";
        }
    }
}