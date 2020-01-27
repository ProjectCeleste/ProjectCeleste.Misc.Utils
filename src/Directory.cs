using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class DirectoryUtils
    {
        [UsedImplicitly]
        public static async Task CopyDirectoryAsync([NotNull] string sourceDirectory,
            [NotNull] string destinationDirectory,
            CancellationToken cancellationToken = default)
        {
            sourceDirectory.ThrowIfNullOrWhiteSpace(nameof(sourceDirectory));
            destinationDirectory.ThrowIfNullOrWhiteSpace(nameof(destinationDirectory));

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            foreach (var folder in Directory.EnumerateDirectories(sourceDirectory))
            {
                var name = Path.GetFileName(folder);
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var destFolder = Path.Combine(destinationDirectory, name);
                await CopyDirectoryAsync(folder, destFolder, cancellationToken);
            }

            Directory.EnumerateFiles(sourceDirectory).AsParallel().ForAll(async file =>
            {
                var name = Path.GetFileName(file);
                if (string.IsNullOrWhiteSpace(name))
                    return;

                var destFile = Path.Combine(destinationDirectory, name);
                await FileUtils.CopyFileAsync(file, destFile, cancellationToken);
            });
        }

        [UsedImplicitly]
        public static void CopyDirectory([NotNull] string sourceDirectory, [NotNull] string destinationDirectory)
        {
            sourceDirectory.ThrowIfNullOrWhiteSpace(nameof(sourceDirectory));
            destinationDirectory.ThrowIfNullOrWhiteSpace(nameof(destinationDirectory));

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            foreach (var folder in Directory.EnumerateDirectories(sourceDirectory))
            {
                var name = Path.GetFileName(folder);
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var destFolder = Path.Combine(destinationDirectory, name);
                CopyDirectory(folder, destFolder);
            }

            Directory.EnumerateFiles(sourceDirectory).AsParallel().ForAll(file =>
            {
                var name = Path.GetFileName(file);
                if (string.IsNullOrWhiteSpace(name))
                    return;

                var destFile = Path.Combine(destinationDirectory, name);
                File.Copy(file, destFile, true);
            });
        }

        [UsedImplicitly]
        public static void CleanUpDirectory([NotNull] string sourceDirectory, [NotNull] string pattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            sourceDirectory.ThrowIfNullOrWhiteSpace(nameof(sourceDirectory));
            pattern.ThrowIfNullOrWhiteSpace(nameof(pattern));

            Directory.EnumerateFiles(sourceDirectory, pattern, searchOption).AsParallel().ForAll(file =>
            {
                try
                {
                    File.Delete(file);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                    //
                }
#pragma warning restore CA1031 // Do not catch general exception types
            });
        }

        [UsedImplicitly]
        public static void MoveDirectory([NotNull] string sourceDirectory, [NotNull] string destinationDirectory,
            bool keepOldFile = true,
            [NotNull] string oldFileExt = ".old")
        {
            sourceDirectory.ThrowIfNullOrWhiteSpace(nameof(sourceDirectory));
            destinationDirectory.ThrowIfNullOrWhiteSpace(nameof(destinationDirectory));
            oldFileExt.ThrowIfNullOrWhiteSpace(nameof(oldFileExt));

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            foreach (var folder in Directory.EnumerateDirectories(sourceDirectory))
            {
                var name = Path.GetFileName(folder);
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                var destFolder = Path.Combine(destinationDirectory, name);
                MoveDirectory(folder, destFolder, keepOldFile, oldFileExt);
            }

            Directory.EnumerateFiles(sourceDirectory).AsParallel().ForAll(file =>
            {
                var name = Path.GetFileName(file);
                if (string.IsNullOrWhiteSpace(name))
                    return;

                var destFile = Path.Combine(destinationDirectory, name);

                FileUtils.MoveFile(file, destFile, keepOldFile, oldFileExt);
            });
        }

        [UsedImplicitly]
        [Pure]
        public static bool IsDirectory([NotNull] string path)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            path = path.Trim();

            if (Directory.Exists(path))
                return true;

            if (File.Exists(path))
                return false;

            return new[] {Path.DirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString()}
                       .Any(x => path.EndsWith(x)) || string.IsNullOrWhiteSpace(Path.GetExtension(path));
        }
    }
}