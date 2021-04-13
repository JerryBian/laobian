namespace Laobian.Share.Util
{
    public static class IpUtil
    {
        public static bool IsLocal(string remoteAddress, string localAddress)
        {
            return !string.IsNullOrEmpty(remoteAddress) && (remoteAddress == "127.0.0.1" || remoteAddress == "::1" ||
                                                            remoteAddress == localAddress);
        }
    }
}