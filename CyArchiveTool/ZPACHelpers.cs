using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;
using System.Text;

namespace CyArchiveTool
{
    internal class ZPACHelpers
    {
        public static bool ValidatePath(string vPath, int fileIndex, HashEntry[]? hashEntryTable, uint hashEntryCount)
        {
            var isValid = false;

            for (int i = 0; i < hashEntryCount; i++)
            {
                if (hashEntryTable[i].StrCode32Hash != 0 && hashEntryTable[i].FileIndex == fileIndex)
                {
                    if (StrCode32(vPath) == hashEntryTable[i].StrCode32Hash)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return isValid;
        }

        private static uint StrCode32(string stringVal)
        {
            var stringBytes = SharedFunctions.ShiftJISEncoding.GetBytes(stringVal);
            uint c = stringBytes[0];
            int n = 0;
            uint id = 0;

            for (int i = 0; i < stringBytes.Length; i++)
            {
                c = stringBytes[i];
                id += ((id << (int)(c & 0x0F)) | ((id >> 3) + (c << (n & 0x0F)) + c));
                n++;
            }

            return id;
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

        public static long ComputePadding(long position, int padWidth)
        {
            long padAmount = 0;

            if (position % padWidth != 0)
            {
                var remainder = position % padWidth;
                var increaseBytes = padWidth - remainder;

                var newPos = position + increaseBytes;
                padAmount = newPos - position;
            }

            return padAmount;
        }
    }
}