using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Laobian.Share.Util;

namespace Laobian.Share.Log
{
    public class GitFileLoggerProcessor : IDisposable
    {
        private readonly ConcurrentQueue<GitFileLog> _messageQueue;
        private readonly GitFileLoggerOptions _options;
        private readonly Thread _underlingThread;
        private bool _stop;

        public GitFileLoggerProcessor(GitFileLoggerOptions options)
        {
            _options = options;
            _messageQueue = new ConcurrentQueue<GitFileLog>();

            _underlingThread = new Thread(Process)
            {
                IsBackground = true,
                Name = "Git file logs processing thread"
            };
            _underlingThread.Start();
        }

        public void Dispose()
        {
            _stop = true;
            try
            {
                _underlingThread.Join(TimeSpan.FromSeconds(15));
            }
            catch (ThreadStateException)
            {
            }
        }

        public void Add(GitFileLog log)
        {
            if (!_stop)
                try
                {
                    _messageQueue.Enqueue(log);
                    return;
                }
                catch (Exception)
                {
                }

            try
            {
                ProcessLogs(log);
            }
            catch (Exception)
            {
            }
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    var logs = new List<GitFileLog>();
                    while (_messageQueue.TryDequeue(out var log)) logs.Add(log);

                    if (logs.Any()) ProcessLogs(logs.ToArray());

                    if (_stop) return;

                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {
                _stop = true;
            }
        }

        private void ProcessLogs(params GitFileLog[] logs)
        {
            if (logs == null || !logs.Any()) return;

            if (string.IsNullOrEmpty(_options.LoggerDir))
            {
                foreach (var log in logs) Console.WriteLine(log);

                return;
            }

            foreach (var log in logs)
            {
                var dir = Path.Combine(_options.LoggerDir, log.TimeStamp.Year.ToString(),
                    log.TimeStamp.Month.ToString("D2"));
                Directory.CreateDirectory(dir);
                File.AppendAllLines(Path.Combine(dir, $"{log.TimeStamp.ToString("yyyy-MM-dd")}.txt"),
                    new[] {JsonUtil.Serialize(log)});
            }
        }
    }
}