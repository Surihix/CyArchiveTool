/// <summary>
/// Provides endianness for BinaryWriter methods.
/// </summary>
internal static class BinaryWriterHelpers
{
    /// <summary>
    /// Writes a two-byte signed integer to the current stream
    /// and advances the stream position by two bytes.
    /// </summary>
    /// <param name="valueToWrite">The two-byte signed integer to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesInt16(this BinaryWriter writerName, short valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes a two-byte unsigned integer to the current stream
    /// and advances the stream position by two bytes.
    /// </summary>
    /// <param name="valueToWrite">The two-byte unsigned integer to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesUInt16(this BinaryWriter writerName, ushort valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes a four-byte signed integer to the current stream 
    /// and advances the stream position by four bytes.
    /// </summary>
    /// <param name="valueToWrite">The four-byte signed integer to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesInt32(this BinaryWriter writerName, int valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes a four-byte unsigned integer to the current stream 
    /// and advances the stream position by four bytes.
    /// </summary>
    /// <param name="valueToWrite">The four-byte unsigned integer to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesUInt32(this BinaryWriter writerName, uint valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes an eight-byte signed integer to the current stream
    /// and advances the stream position by eight bytes.
    /// </summary>
    /// <param name="valueToWrite">The eight-byte signed integer to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesInt64(this BinaryWriter writerName, long valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes an eight-byte unsigned integer to the current stream
    /// and advances the stream position by eight bytes.
    /// </summary>
    /// <param name="valueToWrite">The eight-byte unsigned integer to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesUInt64(this BinaryWriter writerName, ulong valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes a four-byte floating-point value to the current stream
    /// and advances the stream position by four bytes.
    /// </summary>
    /// <param name="valueToWrite">The four-byte floating-point value to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesFloat(this BinaryWriter writerName, float valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }

    /// <summary>
    /// Writes an eight-byte floating-point value to the current stream
    /// and advances the stream position by eight bytes.
    /// </summary>
    /// <param name="valueToWrite">The eight-byte floating-point value to write.</param>
    /// <param name="isBigEndian">Indicates whether the bytes are written in Big Endian.</param>
    public static void WriteBytesDouble(this BinaryWriter writerName, double valueToWrite, bool isBigEndian)
    {
        var writeValueBuffer = BitConverter.GetBytes(valueToWrite);
        ReverseIfBigEndian(isBigEndian, writeValueBuffer);

        writerName.Write(writeValueBuffer);
    }


    static void ReverseIfBigEndian(bool isBigEndian, byte[] writeValueBuffer)
    {
        if (isBigEndian)
        {
            Array.Reverse(writeValueBuffer);
        }
    }
}