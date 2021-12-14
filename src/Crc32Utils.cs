using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Force.Crc32;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class Crc32Utils
    {
        private const int BufferSize = 4 * FileUtils.Kb;

        public static uint GetCrc32(byte[] data)
        {
            data.ThrowIfNull(nameof(data));

            using var fs = new MemoryStream(data);
            return GetCrc32FromStream(fs);
        }

        public static uint GetCrc32FromFile(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace(nameof(fileName));

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"File '{fileName}' not found!", fileName);

            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize,
                FileOptions.SequentialScan);
            return GetCrc32FromStream(fs);
        }

        public static uint GetCrc32FromStream(Stream stream)
        {
            stream.ThrowIfNull(nameof(stream));

            var result = 0u;
            var buffer = new byte[BufferSize];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                result = Crc32Algorithm.Append(result, buffer, 0, read);
            }

            return result;
        }

        public static async Task<uint> GetCrc32Async(byte[] data,
            IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            data.ThrowIfNull(nameof(data));

            using var fs = new MemoryStream(data);
            return await GetCrc32FromStreamAsync(fs, progress, ct);
        }

        public static async Task<uint> GetCrc32FromFileAsync(string fileName,
            IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            fileName.ThrowIfNullOrWhiteSpace(nameof(fileName));

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"File '{fileName}' not found!", fileName);

            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            return await GetCrc32FromStreamAsync(fs, progress, ct);
        }

        public static async Task<uint> GetCrc32FromStreamAsync(Stream stream,
            IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            stream.ThrowIfNull(nameof(stream));

            var result = 0u;
            var buffer = new byte[BufferSize];
            int read;
            var totalRead = 0L;
            var length = stream.Length;
            var lastProgress = 0d;
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                //
                ct.ThrowIfCancellationRequested();

                totalRead += read;

                result = Crc32Algorithm.Append(result, buffer, 0, read);

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

            return result;
        }
    }
}