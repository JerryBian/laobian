using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class GitFileLog
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTime TimeStamp { get; set; }

        public string UserAgent { get; set; }

        public string RequestIp { get; set; }

        public string RequestUrl { get; set; }

        public LogLevel Level { get; set; }

        public string LoggerName { get; set; }

        public void Clone(GitFileLog log)
        {
            foreach (var propertyInfo in typeof(GitFileLog).GetProperties())
            {
                var defaultValue = propertyInfo.PropertyType.IsValueType
                    ? Activator.CreateInstance(propertyInfo.PropertyType)
                    : null;
                var logValue = propertyInfo.GetValue(log);
                if (defaultValue != logValue) propertyInfo.SetValue(this, logValue);
            }
        }
    }
}