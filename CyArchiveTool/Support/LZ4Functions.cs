using K4os.Compression.LZ4;

namespace CyArchiveTool.Support
{
    internal class LZ4Functions
    {
        public static byte[] UncompressLZ4Data(byte[] cmpData, int uncmpSize)
        {
            var uncmpData = new byte[uncmpSize];
            LZ4Codec.Decode(cmpData, 0, cmpData.Length, uncmpData, 0, uncmpData.Length);

            return uncmpData;
        }

        public static byte[] CompressLZ4Data(byte[] dataToCmp, int dataSize)
        {
            var cmpData = new byte[dataSize];
            var cmpSize = LZ4Codec.Encode(dataToCmp, cmpData, LZ4Level.L00_FAST);

            var cmpDataFixed = new byte[cmpSize];
            Array.ConstrainedCopy(cmpData, 0, cmpDataFixed, 0, cmpSize);

            return cmpDataFixed;
        }
    }
}