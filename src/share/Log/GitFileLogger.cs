using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class GitFileLogger : ILogger
    {
        private readonly GitFileLoggerProcessor _queueProcessor;

        public GitFileLogger(GitFileLoggerProcessor queueProcessor)
        {
            _queueProcessor = queueProcessor;
        }

        public IExternalScopeProvider ScopeProvider { get; set; }

        public GitFileLoggerOptions Options { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ScopeProvider?.Push(state) ?? GitNullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= Options.MinLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            try
            {
                var log = GetScopeInfo();
                log.Message = formatter(state, exception);
                log.Exception = exception;
                log.Level = logLevel;
                log.TimeStamp = DateTime.Now;
                _queueProcessor.Add(log);
            }
            catch (Exception)
            {
            }
        }

        private GitFileLog GetScopeInfo()
        {
            var log = new GitFileLog();
            if (ScopeProvider == null) return log;

            ScopeProvider.ForEachScope((o, otherLog) =>
            {
                if (o is GitFileLog logObj) log.Clone(logObj);
            }, log);
            return log;
        }
    }
}