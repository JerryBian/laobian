using System.IO;

namespace Laobian.Share.Util
{
    public static class DirectoryUtil
    {
        public static void Delete(string dir)
        {
            File.SetAttributes(dir, FileAttributes.Normal);

            var files = Directory.GetFiles(dir);
            var dirs = Directory.GetDirectories(dir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var item in dirs)
            {
                Delete(item);
            }

            Directory.Delete(dir, false);
        }
    }
}