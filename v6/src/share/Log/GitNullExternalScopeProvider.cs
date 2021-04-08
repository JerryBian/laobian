using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class GitNullExternalScopeProvider : IExternalScopeProvider
    {
        private GitNullExternalScopeProvider()
        {
        }

        public static IExternalScopeProvider Instance { get; } = new GitNullExternalScopeProvider();

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }

        public IDisposable Push(object state)
        {
            return GitNullScope.Instance;
        }
    }
}