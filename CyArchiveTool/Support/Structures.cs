namespace CyArchiveTool.Support
{
    internal class Structures
    {
        public class PackHeader
        {
            public string? Magic;
            public uint UnkVal;
            public uint HashTableOffset;
            public uint FileTableOffset;
            public uint HashTableEntryCount;
        }

        public class HashTableEntry
        {
            public uint StrCode32Hash;
            public byte UnkFlag;
            public ushort FileIndex;
            public byte Reserved;
        }

        public class HashTableHeader
        {
            public uint HashTableEntryCount;
            public byte[]? Reserved;
        }

        public class FileTableEntry
        {
            public int CmpSize;
            public uint UnkVal;
            public int UncmpSize;
            public uint DataOffset;
            public uint UnkVal2;
            public byte[]? UnkHashOrEncFilePath;
            public byte[]? Reserved;
        }

        public class FileTableHeader
        {
            public uint FileCount;
            public byte[]? Reserved;
        }
    }
}