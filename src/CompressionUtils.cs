using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class CompressionUtils
    {
        private const int CompressBufferSize = 4 * FileUtils.Kb;

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static byte[] Compress([NotNull] byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            input.ThrowIfNullOrEmpty(nameof(input));

            byte[] output;
            using (var inputStream = new MemoryStream(input, false))
            {
                using var final = new MemoryStream();
                {
                    Compress(inputStream, final, compressionLevel);
                    output = final.ToArray();
                }
            }

            return output;
        }

        [UsedImplicitly]
        public static void Compress([NotNull] Stream inputStream, [NotNull] Stream outputStream,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            inputStream.ThrowIfNull(nameof(inputStream));
            outputStream.ThrowIfNull(nameof(outputStream));

            var buffer = new byte[CompressBufferSize];
            int read;
            using var a = new DeflateStream(outputStream, compressionLevel);
            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                a.Write(buffer, 0, read);
            }
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static async Task<byte[]> CompressAsync([NotNull] byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal, [CanBeNull] IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            input.ThrowIfNullOrEmpty(nameof(input));

            byte[] output;
            using (var inputStream = new MemoryStream(input, false))
            {
                using var outputStream = new MemoryStream();
                await CompressAsync(inputStream, outputStream, compressionLevel, progress, ct);
                output = outputStream.ToArray();
            }

            return output;
        }


        [UsedImplicitly]
        public static async Task CompressAsync([NotNull] Stream inputStream, [NotNull] Stream outputStream,
            CompressionLevel compressionLevel = CompressionLevel.Optimal, [CanBeNull] IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            inputStream.ThrowIfNull(nameof(inputStream));
            outputStream.ThrowIfNull(nameof(outputStream));

            var buffer = new byte[CompressBufferSize];
            int read;
            var totalRead = 0L;
            var length = inputStream.Length;
            var lastProgress = 0d;
            using var a = new DeflateStream(outputStream, compressionLevel);
            while ((read = await inputStream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                ct.ThrowIfCancellationRequested();

                totalRead += read;

                await a.WriteAsync(buffer, 0, read, ct);

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

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static byte[] Decompress([NotNull] this byte[] input)
        {
            input.ThrowIfNull(nameof(input));

            byte[] output;
            using (var inputStream = new MemoryStream(input, false))
            {
                using var outputStream = new MemoryStream();
                Decompress(inputStream, outputStream);
                output = outputStream.ToArray();
            }

            return output;
        }

        [UsedImplicitly]
        public static void Decompress([NotNull] Stream inputStream, [NotNull] Stream outputStream)
        {
            inputStream.ThrowIfNull(nameof(inputStream));
            outputStream.ThrowIfNull(nameof(outputStream));

            using var a = new DeflateStream(inputStream, CompressionMode.Decompress);
            var buffer = new byte[CompressBufferSize];
            int read;
            while ((read = a.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, read);
            }
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static async Task<byte[]> DecompressAsync([NotNull] this byte[] input,
            [CanBeNull] IProgress<double> progress = null, CancellationToken ct = default)
        {
            input.ThrowIfNull(nameof(input));

            byte[] output;
            using (var inputStream = new MemoryStream(input, false))
            {
                using var outputStream = new MemoryStream();
                await DecompressAsync(inputStream, outputStream, progress, ct);
                output = outputStream.ToArray();
            }

            return output;
        }

        [UsedImplicitly]
        public static async Task DecompressAsync([NotNull] Stream inputStream, [NotNull] Stream outputStream,
            [CanBeNull] IProgress<double> progress = null, CancellationToken ct = default)
        {
            inputStream.ThrowIfNull(nameof(inputStream));
            outputStream.ThrowIfNull(nameof(outputStream));

            using var a = new DeflateStream(inputStream, CompressionMode.Decompress);
            var buffer = new byte[CompressBufferSize];
            int read;
            while ((read = await a.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                await outputStream.WriteAsync(buffer, 0, read, ct);
            }
        }
    }
}