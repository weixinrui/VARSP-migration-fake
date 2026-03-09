using System;
using System.IO;
using Xunit;

namespace VARSP_migration_fake.Tests;

public class Lz4CompressorTests
{
    [Fact]
    public void Compress_ThrowsArgumentNullException_WhenInputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Lz4Compressor.Compress(null!));
    }

    [Fact]
    public void Compress_EmptyArray_ReturnsCompressedBytes()
    {
        var input = Array.Empty<byte>();
        var compressed = Lz4Compressor.Compress(input);
        Assert.NotNull(compressed);
        Assert.True(compressed.Length >= 0);
    }

    [Fact]
    public void Compress_And_Decompress_RoundTrip_PreservesData()
    {
        var input = System.Text.Encoding.UTF8.GetBytes("Hello, LZ4 compression world!");
        var compressed = Lz4Compressor.Compress(input);
        Assert.NotNull(compressed);

        var decompressed = Lz4Compressor.Decompress(compressed, input.Length);
        Assert.Equal(input, decompressed);
    }

    [Fact]
    public void Compress_And_Decompress_LargeData_RoundTrip_PreservesData()
    {
        var rng = new Random(42);
        var input = new byte[10_000];
        rng.NextBytes(input);

        var compressed = Lz4Compressor.Compress(input);
        var decompressed = Lz4Compressor.Decompress(compressed, input.Length);
        Assert.Equal(input, decompressed);
    }

    [Fact]
    public void Decompress_ThrowsArgumentNullException_WhenCompressedIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Lz4Compressor.Decompress(null!, 0));
    }

    [Fact]
    public void Decompress_ThrowsInvalidDataException_WhenOriginalLengthMismatch()
    {
        var validCompressed = Lz4Compressor.Compress(new byte[] { 1, 2, 3 });
        Assert.Throws<InvalidDataException>(() =>
            Lz4Compressor.Decompress(validCompressed, originalLength: 999));
    }

    [Fact]
    public void CompressStream_ThrowsArgumentNullException_WhenInputIsNull()
    {
        using var output = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => Lz4Compressor.CompressStream(null!, output));
    }

    [Fact]
    public void CompressStream_ThrowsArgumentNullException_WhenOutputIsNull()
    {
        using var input = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => Lz4Compressor.CompressStream(input, null!));
    }

    [Fact]
    public void DecompressStream_ThrowsArgumentNullException_WhenInputIsNull()
    {
        using var output = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => Lz4Compressor.DecompressStream(null!, output));
    }

    [Fact]
    public void DecompressStream_ThrowsArgumentNullException_WhenOutputIsNull()
    {
        using var input = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => Lz4Compressor.DecompressStream(input, null!));
    }

    [Fact]
    public void CompressStream_And_DecompressStream_RoundTrip_PreservesData()
    {
        var original = System.Text.Encoding.UTF8.GetBytes("Stream-based LZ4 compress and decompress.");
        using var inputStream = new MemoryStream(original);

        using var compressedStream = new MemoryStream();
        Lz4Compressor.CompressStream(inputStream, compressedStream);
        compressedStream.Position = 0;

        using var decompressedStream = new MemoryStream();
        Lz4Compressor.DecompressStream(compressedStream, decompressedStream);

        var result = decompressedStream.ToArray();
        Assert.Equal(original, result);
    }

    [Fact]
    public void CompressStream_EmptyStream_RoundTrips()
    {
        using var inputStream = new MemoryStream();
        using var compressedStream = new MemoryStream();
        Lz4Compressor.CompressStream(inputStream, compressedStream);
        compressedStream.Position = 0;

        using var decompressedStream = new MemoryStream();
        Lz4Compressor.DecompressStream(compressedStream, decompressedStream);
        Assert.Empty(decompressedStream.ToArray());
    }
}
