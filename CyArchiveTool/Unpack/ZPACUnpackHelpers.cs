using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;

namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackHelpers
    {
        public static void DataUnpack(string unpackDir, string vPath, ref int duplicateCounter, BinaryReader packFileReader, FileEntry fileEntry)
        {
            var outFile = Path.Combine(unpackDir, vPath);
            var outFileDir = Path.GetDirectoryName(outFile);

            if (!Directory.Exists(outFileDir))
            {
                Directory.CreateDirectory(outFileDir);
            }

            if (File.Exists(outFile))
            {
                File.Delete(outFile);
                duplicateCounter++;
            }

            var fileDataInPack = packFileReader.ReadBytes(fileEntry.CmpSize);

            if (fileEntry.CmpLevel != 0 && fileEntry.CmpSize != fileEntry.UncmpSize)
            {
                var dcmpData = LZ4Functions.UncompressLZ4Data(fileDataInPack, fileEntry.UncmpSize);
                File.WriteAllBytes(outFile, dcmpData);
            }
            else
            {
                File.WriteAllBytes(outFile, fileDataInPack);
            }
        }
    }
}