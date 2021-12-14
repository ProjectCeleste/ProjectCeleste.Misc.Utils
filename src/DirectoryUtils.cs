using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class DirectoryUtils
    {
        public static async Task CopyDirectoryAsync(string sourceDirectory,
            string destinationDirectory,
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

        public static void CopyDirectory(string sourceDirectory, string destinationDirectory)
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

        public static void CleanUpDirectory(string sourceDirectory, string pattern = "*",
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

        public static void MoveDirectory(string sourceDirectory, string destinationDirectory,
            bool keepOldFile = true,
            string oldFileExt = ".old")
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

        public static bool IsDirectory(string path)
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