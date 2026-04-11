namespace CyArchiveTool.Support.Structures
{
    internal class ZPACHeader
    {
        public string? Magic;
        public uint UnkVal;
        public uint HashTableOffset;
        public uint FileTableOffset;
        public uint HashTableEntryCount;
    }
}