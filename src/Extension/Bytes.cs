using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class BytesExtensions
    {
        public static uint GetCrc32(this byte[] data)
        {
            return Crc32Utils.GetCrc32(data);
        }

        public static async Task<uint> GetCrc32Async(this byte[] data,
            IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            return await Crc32Utils.GetCrc32Async(data, progress, ct);
        }

        public static byte[] Compress(this byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            return CompressionUtils.Compress(input, compressionLevel);
        }

        public static async Task<byte[]> CompressAsync(this byte[] input,
            CompressionLevel compressionLevel = CompressionLevel.Optimal, IProgress<double> progress = null,
            CancellationToken ct = default)
        {
            return await CompressionUtils.CompressAsync(input, compressionLevel, progress, ct);
        }

        public static byte[] Decompress(this byte[] input)
        {
            return CompressionUtils.Decompress(input);
        }

        public static async Task<byte[]> DecompressAsync(this byte[] input,
            IProgress<double> progress = null, CancellationToken ct = default)
        {
            return await CompressionUtils.DecompressAsync(input, progress, ct);
        }
    }
}