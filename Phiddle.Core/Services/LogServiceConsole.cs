using System;

namespace Phiddle.Core.Services
{
    public class LogServiceConsole : ILogService
    {
        public string Source { get; set; }
        public LogServiceConsole()
        {
            Source = "Phiddle.Core";
        }
        public void Debug(string source, string message)
        {
#if DEBUG
            Log("D", source, message);
#else
            // No debug messages when building Release
#endif
        }

        public void Error(string source, string message)
        {
            Log("E", source, message);
        }

        public void Error(string source, string message, Exception ex)
        {
            Log("E", source, $"{message} ({ex.Message})");
        }

        public void Info(string source, string message)
        {
            Log("I", source, message);
        }

        public void Warning(string source, string message)
        {
            Log("W", source, message);
        }

        private void Log(string level, string source, string message)
        {
            var pname = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            var pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var time = DateTime.Now;
            var src = source == string.Empty ? $"{Source}:" : $"{Source}.{source}: ";
            Console.WriteLine($"[{time:HH:mm:ss.fffff} {pname} ({pid}:{tid})] ({level}) {src}{message}");
        }
    }
}
