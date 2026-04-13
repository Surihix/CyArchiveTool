using K4os.Compression.LZ4;
using System.Text;

namespace CyArchiveTool
{
    internal class ZPACHelpers
    {
        public static byte[] UncompressLZ4Data(byte[] cmpData, int uncmpSize)
        {
            var uncmpData = new byte[uncmpSize];
            LZ4Codec.Decode(cmpData, 0, cmpData.Length, uncmpData, 0, uncmpData.Length);

            return uncmpData;
        }

        public static string GetExtension(byte[] fileData)
        {
            var fileExtn = string.Empty;

            if (fileData.Length < 4)
            {
                return fileExtn;
            }

            var readHeader = Encoding.ASCII.GetString(fileData, 0, 4).Replace("\0", "");

            switch (readHeader)
            {
                case "DDS ":
                    fileExtn = ".dds";
                    break;
                case "DXBC":
                    fileExtn = ".bin";
                    break;
                case "mdzx":
                    fileExtn = ".mdzx";
                    break;
                case "mdl0":
                    fileExtn = ".mdlbin";
                    break;
                default:
                    fileExtn = ".bin";
                    break;
            }

            if (readHeader.StartsWith("{"))
            {
                fileExtn = ".json";
            }

            return fileExtn;
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