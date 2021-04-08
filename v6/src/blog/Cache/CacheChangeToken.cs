using System;
using Microsoft.Extensions.Primitives;

namespace Laobian.Blog.Cache
{
    public class CacheChangeToken : IChangeToken
    {
        private readonly DateTime _initialAt;

        public CacheChangeToken()
        {
            _initialAt = DateTime.Now;
        }

        public bool ActiveChangeCallbacks => false;

        public bool HasChanged => GlobalFlag.HardRefreshAt > _initialAt;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }
    }
}