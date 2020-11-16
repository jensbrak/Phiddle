using Phiddle.Core.Services;

namespace Phiddle.Win.Services
{
    public class LoggingService : LogServiceConsole
    {
        public LoggingService()
        {
            Source = "Phiddle.Win";
        }
    }
}