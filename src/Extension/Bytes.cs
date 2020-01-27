using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class BytesExtensions
    {
        [UsedImplicitly]
        [Pure]
        public static uint GetCrc32([NotNull] this byte[] data)
        {
            return Crc32Utils.GetCrc32(data);
        }

        [UsedImplicitly]
        [Pure]
        public static async Task<uint> GetCrc32Async([NotNull] this byte[] data,
            [CanBeNull] IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            return await Crc32Utils.GetCrc32Async(data, progress, ct);
        }

        #region Compression

        private const int CompressBufferSize = 64 * FileUtils.Kb;

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static byte[] Compress([NotNull] this byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            input.ThrowIfNull(nameof(input));

            byte[] output;
            using (var inputStream = new MemoryStream(input))
            {
                using var final = new MemoryStream();
                {
                    using (var a = new DeflateStream(final, compressionLevel))
                    {
                        var buffer = new byte[CompressBufferSize];
                        int read;
                        var totalRead = 0L;
                        var length = inputStream.Length;

                        while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalRead += read;

                            a.Write(buffer, 0, read);

                            if (totalRead >= length)
                                break;
                        }
                    }

                    output = final.ToArray();
                }
            }

            return output;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static async Task<byte[]> CompressAsync([NotNull] this byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal, [CanBeNull] IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            input.ThrowIfNull(nameof(input));

            byte[] output;
            using (var inputStream = new MemoryStream(input))
            {
                using var final = new MemoryStream();
                {
                    using (var a = new DeflateStream(final, compressionLevel))
                    {
                        var buffer = new byte[CompressBufferSize];
                        int read;
                        var totalRead = 0L;
                        var length = inputStream.Length;
                        var lastProgress = 0d;
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

                    output = final.ToArray();
                }
            }

            return output;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static byte[] Decompress([NotNull] this byte[] input)
        {
            input.ThrowIfNull(nameof(input));

            byte[] output;
            using (var f = new MemoryStream(input))
            {
                using var a = new DeflateStream(f, CompressionMode.Decompress);
                using var final = new MemoryStream();
                var buffer = new byte[CompressBufferSize];
                int read;
                while ((read = a.Read(buffer, 0, buffer.Length)) > 0)
                {
                    final.Write(buffer, 0, read);
                }

                output = final.ToArray();
            }

            return output;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static async Task<byte[]> DecompressAsync([NotNull] this byte[] input, CancellationToken ct = default)
        {
            input.ThrowIfNull(nameof(input));

            byte[] output;
            using (var f = new MemoryStream(input))
            {
                using var a = new DeflateStream(f, CompressionMode.Decompress);
                using var final = new MemoryStream();
                var buffer = new byte[CompressBufferSize];
                int read;
                while ((read = await a.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                {
                    await final.WriteAsync(buffer, 0, read, ct);
                }

                output = final.ToArray();
            }

            return output;
        }

        #endregion
    }
}