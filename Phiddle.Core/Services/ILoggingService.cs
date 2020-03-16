using System;

namespace Phiddle.Core.Services
{
    public interface ILoggingService
    {
        string Owner { get; set; }
        void Info(string source, string message);
        void Debug(string source, string message);
        void Warning(string source, string message);
        void Error(string source, string message);
        void Error(string source, string message, Exception ex);
    }
}
