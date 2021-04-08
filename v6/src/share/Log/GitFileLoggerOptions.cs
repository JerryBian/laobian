using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class GitFileLoggerOptions
    {
        public LogLevel MinLevel { get; set; } = LogLevel.Information;

        public string LoggerName { get; set; }

        public string LoggerDir { get; set; }
    }
}