using System;

namespace Laobian.Share.Log
{
    public class GitNullScope : IDisposable
    {
        private GitNullScope()
        {
        }

        public static GitNullScope Instance { get; } = new GitNullScope();

        public void Dispose()
        {
        }
    }
}