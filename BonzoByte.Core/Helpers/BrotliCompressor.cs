using BrotliSharpLib;
using System.Text;

namespace BonzoByte.Core.Helpers
{
    public static class BrotliCompressor
    {
        /// <summary>
        /// Compresses text content and saves it as a .br file.
        /// </summary>
        public static void CompressStringToFile(string input, string outputFilePath)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var compressed = Brotli.CompressBuffer(inputBytes, 0, inputBytes.Length, quality: 11, lgwin: 24);
            File.WriteAllBytes(outputFilePath, compressed);
        }

        /// <summary>
        /// Decompresses a .br file and returns the contents as a string.
        /// </summary>
        public static string DecompressFileToString(string inputFilePath)
        {
            var compressedBytes = File.ReadAllBytes(inputFilePath);
            var decompressed = Brotli.DecompressBuffer(compressedBytes, 0, compressedBytes.Length);
            return Encoding.UTF8.GetString(decompressed);
        }

        /// <summary>
        /// Compresses byte[] and saves as a .br file.
        /// </summary>
        public static void CompressBytesToFile(byte[] inputBytes, string outputFilePath)
        {
            var compressed = Brotli.CompressBuffer(inputBytes, 0, inputBytes.Length, quality: 11, lgwin: 24);
            File.WriteAllBytes(outputFilePath, compressed);
        }

        /// <summary>
        /// Decompresses a .br file into byte[].
        /// </summary>
        public static byte[] DecompressFileToBytes(string inputFilePath)
        {
            var compressedBytes = File.ReadAllBytes(inputFilePath);
            return Brotli.DecompressBuffer(compressedBytes, 0, compressedBytes.Length);
        }
    }
}