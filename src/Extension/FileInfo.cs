using System.IO;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class FileInfoExtensions
    {
        public static bool IsFileLocked(this FileInfo file)
        {
            return FileUtils.IsFileLocked(file);
        }
    }
}