using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using ZipFile = System.IO.Compression.ZipFile;

namespace ProjectCeleste.Misc.Utils
{
    public static class ZipFileUtils
    {
        private const int BufferSize = 4 * FileUtils.Kb;

        [UsedImplicitly]
        [Obsolete("Use System.IO.Compression.ZipFile.ExtractToDirectory() instead")]
        public static void ExtractZipFile([NotNull] string archiveFileName, [NotNull] string outFolder)
        {
            archiveFileName.ThrowIfNullOrWhiteSpace(nameof(archiveFileName));
            outFolder.ThrowIfNullOrWhiteSpace(nameof(outFolder));

            using var zipFile = ZipFile.OpenRead(archiveFileName);
            foreach (var zipEntry in zipFile.Entries)
            {
                var filePath = Path.Combine(outFolder, zipEntry.Name);
                var directoryName = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrWhiteSpace(directoryName))
                    Directory.CreateDirectory(directoryName);

                using var a = zipEntry.Open();
                using var fileStreamFinal = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None, BufferSize, FileOptions.SequentialScan);
                using var final = new BinaryWriter(fileStreamFinal);
                var buffer = new byte[BufferSize];
                int read;
                while ((read = a.Read(buffer, 0, buffer.Length)) > 0)
                {
                    final.Write(buffer, 0, read);
                }
            }
        }

        [UsedImplicitly]
        public static async Task ExtractZipFileAsync([NotNull] string archiveFileName, [NotNull] string outFolder,
            [CanBeNull] IProgress<double> progress = null, CancellationToken ct = default)
        {
            archiveFileName.ThrowIfNullOrWhiteSpace(nameof(archiveFileName));
            outFolder.ThrowIfNullOrWhiteSpace(nameof(outFolder));

            using var zipFile = ZipFile.OpenRead(archiveFileName);
            foreach (var zipEntry in zipFile.Entries)
            {
                var length = zipEntry.Length;

                var filePath = Path.Combine(outFolder, zipEntry.Name);
                var directoryName = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrWhiteSpace(directoryName))
                    Directory.CreateDirectory(directoryName);

                using var a = zipEntry.Open();
                using var fileStreamFinal = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None, BufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
                var buffer = new byte[BufferSize];
                int read;
                var totalRead = 0L;
                var lastProgress = 0d;
                while ((read = await a.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                {
                    ct.ThrowIfCancellationRequested();

                    totalRead += read;

                    await fileStreamFinal.WriteAsync(buffer, 0, read, ct);

                    if (progress != null)
                    {
                        var newProgress = (double) totalRead / length * 100;

                        if (newProgress - lastProgress > 1)
                        {
                            progress.Report(newProgress);
                            lastProgress = newProgress;
                        }
                    }

                    if (totalRead >= length)
                        break;
                }
            }
        }

        [UsedImplicitly]
        public static void CompressDirectory([NotNull] string directory, [NotNull] string outFileName,
            CompressionLevel compressionLevel = CompressionLevel.BestCompression)
        {
            directory.ThrowIfNullOrWhiteSpace(nameof(directory));
            outFileName.ThrowIfNullOrWhiteSpace(nameof(outFileName));

            if (directory.EndsWith($"{Path.DirectorySeparatorChar}") ||
                directory.EndsWith($"{Path.AltDirectorySeparatorChar}"))
            {
                directory = directory.Substring(0, directory.Length - 1);
            }

            using var zip = new Ionic.Zip.ZipFile
            {
                CompressionLevel = compressionLevel,
                UseZip64WhenSaving = Zip64Option.AsNecessary
            };
            foreach (var f in Directory.GetFiles(directory, "*",
                SearchOption.AllDirectories).ToArray())
            {
                var directoryName = Path.GetDirectoryName(f);
                if (directoryName == null) continue;
                zip.AddFile(f, directoryName.Replace(directory, string.Empty));
            }

            zip.Save(outFileName);
        }

        [UsedImplicitly]
        public static void CompressFiles([NotNull] string baseDirectory, [NotNull] string outFileName,
            [NotNull] IEnumerable<FileInfo> fileInfos,
            CompressionLevel compressionLevel = CompressionLevel.BestCompression)
        {
            baseDirectory.ThrowIfNullOrWhiteSpace(nameof(baseDirectory));
            fileInfos.ThrowIfNull(nameof(fileInfos));
            outFileName.ThrowIfNullOrWhiteSpace(nameof(outFileName));

            if (baseDirectory.EndsWith($"{Path.DirectorySeparatorChar}") ||
                baseDirectory.EndsWith($"{Path.AltDirectorySeparatorChar}"))
            {
                baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 1);
            }

            using var zip = new Ionic.Zip.ZipFile
            {
                CompressionLevel = compressionLevel,
                UseZip64WhenSaving = Zip64Option.AsNecessary
            };
            foreach (var f in fileInfos)
            {
                var directoryName = Path.GetDirectoryName(f.FullName);
                zip.AddFile(f.FullName, directoryName?.Replace(baseDirectory, string.Empty));
            }

            zip.Save(outFileName);
        }

        [UsedImplicitly]
        public static void CompressFiles([NotNull] IEnumerable<KeyValuePair<string, string>> mappedPathsToContents,
            [NotNull] string outFileName,
            CompressionLevel compressionLevel = CompressionLevel.BestCompression)
        {
            mappedPathsToContents.ThrowIfNull(nameof(mappedPathsToContents));
            outFileName.ThrowIfNullOrWhiteSpace(nameof(outFileName));

            using var zip = new Ionic.Zip.ZipFile
            {
                CompressionLevel = compressionLevel,
                UseZip64WhenSaving = Zip64Option.AsNecessary
            };
            foreach (var entry in mappedPathsToContents)
                zip.AddEntry(entry.Key, entry.Value);

            zip.Save(outFileName);
        }
    }
}