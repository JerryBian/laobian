using System;
using System.Threading;

namespace Laobian.Share
{
    public class GlobalFlag
    {
        public static DateTime HardRefreshAt { get; set; }

        public static ManualResetEventSlim PullGitFileEvent { get; } = new ManualResetEventSlim(false);
    }
}