using System;
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

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static byte[] Compress([NotNull] this byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            return CompressionUtils.Compress(input, compressionLevel);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static async Task<byte[]> CompressAsync([NotNull] this byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal, [CanBeNull] IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            return await CompressionUtils.CompressAsync(input, compressionLevel, progress, ct);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static byte[] Decompress([NotNull] this byte[] input)
        {
            return CompressionUtils.Decompress(input);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static async Task<byte[]> DecompressAsync([NotNull] this byte[] input,
            [CanBeNull] IProgress<double> progress = null, CancellationToken ct = default)
        {
            return await CompressionUtils.DecompressAsync(input, progress, ct);
        }
    }
}