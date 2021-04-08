using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Setting;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Command
{
    public class ProcessCommandClient : ICommandClient
    {
        private readonly ApiSetting _apiSetting;

        public ProcessCommandClient(IOptions<ApiSetting> options)
        {
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
                Arguments = string.IsNullOrEmpty(_apiSetting.CommandLineBeginArg)
                    ? command
                    : $"{_apiSetting.CommandLineBeginArg} {command}",
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
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
    }
}