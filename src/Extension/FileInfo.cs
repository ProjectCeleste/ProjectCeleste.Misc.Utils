using System.IO;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class FileInfoExtensions
    {
        [UsedImplicitly]
        [Pure]
        public static bool IsFileLocked([NotNull] this FileInfo file)
        {
            return FileUtils.IsFileLocked(file);
        }
    }
}