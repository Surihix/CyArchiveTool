using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;

namespace CyArchiveTool.Repack
{
    internal class ZPACRepackHelpers
    {
        public static void DataRepack(string unpackedDir, string vPath, FileEntry currentFileEntry, BinaryWriter fileDataWriter, ref FileEntryTable fileEntryTable, int index, ref bool isNullData)
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
            currentFileEntry.DataOffset = (uint)fileDataWriter.BaseStream.Position;
            fileDataWriter.Write(dataToPack);

            fileEntryTable.FileEntries[index] = currentFileEntry;

            var padAmount = ZPACHelpers.ComputePadding(fileDataWriter.BaseStream.Position, 16);

            if (padAmount != 0)
            {
                fileDataWriter.Write(new byte[padAmount]);
            }
        }
    }
}