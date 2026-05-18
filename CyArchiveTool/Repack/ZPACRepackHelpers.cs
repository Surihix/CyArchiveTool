using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;

namespace CyArchiveTool.Repack
{
    internal class ZPACRepackHelpers
    {
        public static byte[] EncryptFilePath(byte[] vPathData, uint pathHash)
        {
            var encPathData = new byte[224];
            var xorValue = CygamesIVTable.IVs[pathHash & 0x3FF];
            var dataIndex = 0;

            for (int i = 0; i < vPathData.Length; i++)
            {
                var currentByte = vPathData[i];
                encPathData[i] = (byte)(xorValue ^ currentByte);

                xorValue = encPathData[i];
                dataIndex++;
            }

            xorValue = encPathData[dataIndex - 1];

            for (int i = dataIndex; i < encPathData.Length; i++)
            {
                encPathData[i] = (byte)(xorValue ^ CygamesIVTable.IVs[i]);
                xorValue = encPathData[i];
            }

            return encPathData;
        }

        public static void DataRepack(string unpackedDir, string vPath, FileEntry currentFileEntry, FileStream fileDataStream, ref bool isNullData)
        {
            var outFile = Path.Combine(unpackedDir, vPath);

            var fileData = Array.Empty<byte>();
            var dataToPack = Array.Empty<byte>();

            if (File.Exists(outFile))
            {
                fileData = File.ReadAllBytes(outFile);
            }

            if (!File.Exists(outFile))
            {
                isNullData = true;
                fileData = new byte[16];
            }

            if (currentFileEntry.CmpLevel == 0 || isNullData)
            {
                dataToPack = fileData;
            }

            if (currentFileEntry.CmpLevel != 0 && !isNullData)
            {
                dataToPack = LZ4Functions.CompressLZ4Data(fileData, fileData.Length, currentFileEntry.CmpLevel);
            }

            currentFileEntry.CmpSize = dataToPack.Length;
            currentFileEntry.UncmpSize = fileData.Length;
            currentFileEntry.DataOffset = (uint)fileDataStream.Position;
            fileDataStream.Write(dataToPack, 0, currentFileEntry.CmpSize);

            var padAmount = ZPACHelpers.ComputePadding(fileDataStream.Position, 16);
            currentFileEntry.PaddingSize = (uint)padAmount;

            if (padAmount != 0)
            {
                var paddingData = new byte[padAmount];
                fileDataStream.Write(paddingData, 0, paddingData.Length);
            }
        }
    }
}