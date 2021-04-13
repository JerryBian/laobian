﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Setting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Command
{
    public class ProcessCommandClient : ICommandClient
    {
        private readonly ApiSetting _apiSetting;
        private readonly ILogger<ProcessCommandClient> _logger;

        public ProcessCommandClient(IOptions<ApiSetting> options, ILogger<ProcessCommandClient> logger)
        {
            _logger = logger;
            _apiSetting = options.Value;
        }

        public async Task<string> RunAsync(string command)
        {
            using var process = new Process();
            using var resetEvent1 = new ManualResetEventSlim(false);
            using var resetEvent2 = new ManualResetEventSlim(false);
            var startInfo = new ProcessStartInfo
            {
                FileName = _apiSetting.CommandLineName,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = string.IsNullOrEmpty(_apiSetting.CommandLineBeginArg) ? FormatCommand(command) : $"{_apiSetting.CommandLineBeginArg} {FormatCommand(command)}"
            };

            _logger.LogInformation($"FileName={startInfo.FileName}, Args={startInfo.Arguments}");
            process.StartInfo = startInfo;

            var output = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                    resetEvent1.Set();
                else
                    output.AppendLine(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                    resetEvent2.Set();
                else
                    output.AppendLine(args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            resetEvent1.Wait();
            resetEvent2.Wait();
            return output.ToString();
        }

        private string FormatCommand(string cmd)
        {
            return $"\"{cmd.Replace("\"", "\\\"")}\"";
        }
    }
}