namespace CyArchiveTool.Support.Structures
{
    internal class ZPACHeader
    {
        public string? Magic;
        public uint Version;
        public uint HashTableOffset;
        public uint FileTableOffset;
    }
}