using System;
using System.IO;

namespace Laobian.Share.Util
{
    public static class StringUtil
    {
        public static bool EqualsIgnoreCase(string left, string right)
        {
            if (left == null && right != null) return false;

            if (left != null && right == null) return false;

            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }

        public static string GenerateRandom(int length)
        {
            var path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            return "r" + path.Substring(0, Math.Min(length-1, path.Length-1));
        }
    }
}