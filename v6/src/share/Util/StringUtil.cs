using System;

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
    }
}