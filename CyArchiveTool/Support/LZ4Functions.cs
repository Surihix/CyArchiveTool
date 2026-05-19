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

        public static byte[] CompressLZ4Data(byte[] dataToCmp, int dataSize, uint cmpLevel)
        {
            var cmpData = new byte[dataSize];
            var lz4Level = LZ4Level.L00_FAST;

            switch (cmpLevel)
            {
                case 1:
                    lz4Level = LZ4Level.L00_FAST;
                    break;

                case 2:
                case 3:
                    lz4Level = LZ4Level.L03_HC;
                    break;
            }

            var cmpSize = LZ4Codec.Encode(dataToCmp, cmpData, lz4Level);

            var cmpDataFixed = new byte[cmpSize];
            Array.ConstrainedCopy(cmpData, 0, cmpDataFixed, 0, cmpSize);

            return cmpDataFixed;
        }
    }
}