using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class FileUtils
    {
        public const int Kb = 1024;

        public const int Mb = Kb * Kb;

        public const int Gb = Kb * Mb;

        [UsedImplicitly]
        public static async Task CopyFileAsync([NotNull] string sourceFile, [NotNull] string destinationFile,
            CancellationToken cancellationToken = default)
        {
            sourceFile.ThrowIfNullOrWhiteSpace(nameof(sourceFile));
            destinationFile.ThrowIfNullOrWhiteSpace(nameof(destinationFile));

            const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            const int bufferSize = 64 * Kb;
            using var sourceStream =
                new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
            using var destinationStream =
                new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize,
                    fileOptions);
            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken);
        }

        [UsedImplicitly]
        public static void MoveFile([NotNull] string sourceFile, [NotNull] string destinationFile,
            bool keepOld = true,
            [NotNull] string oldExt = ".old")
        {
            sourceFile.ThrowIfNullOrWhiteSpace(nameof(sourceFile));
            destinationFile.ThrowIfNullOrWhiteSpace(nameof(destinationFile));
            oldExt.ThrowIfNullOrWhiteSpace(nameof(oldExt));

            if (!File.Exists(sourceFile))
                throw new FileNotFoundException(string.Empty, sourceFile);

            if (File.Exists(destinationFile))
            {
                if (keepOld)
                {
                    if (File.Exists(destinationFile + oldExt))
                        File.Delete(destinationFile + oldExt);

                    File.Move(destinationFile, destinationFile + oldExt);
                }
                else
                {
                    File.Delete(destinationFile);
                }
            }

            File.Move(sourceFile, destinationFile);
        }

        [UsedImplicitly]
        [Pure]
        public static bool IsFileLocked([NotNull] string filename)
        {
            filename.ThrowIfNullOrWhiteSpace(nameof(filename));

            return IsFileLocked(new FileInfo(filename));
        }

        [UsedImplicitly]
        [Pure]
        public static bool IsFileLocked([NotNull] FileInfo fileInfo)
        {
            fileInfo.ThrowIfNull(nameof(fileInfo));

            try
            {
                using (fileInfo.Open(FileMode.Open,
                    FileAccess.ReadWrite, FileShare.None))
                {
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (IOException)
            {
                return true;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return false;
        }

        [UsedImplicitly]
        [Pure]
        public static bool IsFile([NotNull] string path)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            path = path.Trim();

            if (Directory.Exists(path))
                return false;

            if (File.Exists(path))
                return true;

            return new[] {Path.DirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString()}
                       .Any(x => !path.EndsWith(x)) && !string.IsNullOrWhiteSpace(Path.GetExtension(path));
        }
    }
}