using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Util;

namespace Laobian.Share.Extension
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> ToPaged<T>(this IEnumerable<T> source, int chunkSize, int page)
        {
            if (page <= 0 || page > source.Count()) return source;

            return source.Skip(chunkSize * (page - 1)).Take(chunkSize);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> source, string keyword)
        {
            foreach (var item in source)
                if (StringUtil.EqualsIgnoreCase(item, keyword))
                    return true;

            return false;
        }
    }
}