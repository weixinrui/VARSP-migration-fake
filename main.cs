using System;
using System.IO;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;

/// <summary>
/// Lz4Compressor: Static utility class for LZ4 compression and decompression
/// </summary>
public static class Lz4Compressor
{
    /// <summary>
    /// Compress a byte array using LZ4.
    /// </summary>
    /// <param name="input">Input byte array to compress.</param>
    /// <returns>Compressed byte array.</returns>
    public static byte[] Compress(byte[] input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        int maxOut = LZ4Codec.MaximumOutputSize(input.Length);
        var output = new byte[maxOut];
        var encoded = LZ4Codec.Encode(input, 0, input.Length, output, 0, maxOut);
        Array.Resize(ref output, encoded);
        return output;
    }

    /// <summary>
    /// Decompress a byte array using LZ4.
    /// </summary>
    /// <param name="compressed">The compressed byte array.</param>
    /// <param name="originalLength">The original uncompressed length.</param>
    /// <returns>Decompressed byte array.</returns>
    public static byte[] Decompress(byte[] compressed, int originalLength)
    {
        if (compressed == null) throw new ArgumentNullException(nameof(compressed));
        var output = new byte[originalLength];
        int decoded = LZ4Codec.Decode(compressed, 0, compressed.Length, output, 0, originalLength);
        if (decoded != originalLength)
            throw new InvalidDataException("Decompression size mismatch.");
        return output;
    }

    /// <summary>
    /// Compress input stream to output stream using LZ4.
    /// </summary>
    /// <param name="input">Input stream to compress.</param>
    /// <param name="output">Target output stream for compressed content.</param>
    public static void CompressStream(Stream input, Stream output)
    {
        if (input == null || output == null)
            throw new ArgumentNullException();
        using (var lz4Stream = LZ4Stream.Encode(output, leaveOpen: true))
        {
            input.CopyTo(lz4Stream);
        }
    }

    /// <summary>
    /// Decompress input stream to output stream using LZ4.
    /// </summary>
    /// <param name="input">Compressed input stream.</param>
    /// <param name="output">Target output stream for decompressed content.</param>
    public static void DecompressStream(Stream input, Stream output)
    {
        if (input == null || output == null)
            throw new ArgumentNullException();
        using (var lz4Stream = LZ4Stream.Decode(input, leaveOpen: true))
        {
            lz4Stream.CopyTo(output);
        }
    }
}