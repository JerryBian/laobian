namespace Laobian.Blog.Cache
{
    public class CacheKeyBuilder
    {
        public static string Build(params object[] parts)
        {
            return $"B:{string.Join(":", parts)}";
        }
    }
}