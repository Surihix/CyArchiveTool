namespace CyArchiveTool.Support.Structures
{
    internal class FileEntryTable
    {
        public uint FileCount;
        public byte[]? Reserved;
        public FileEntry[]? FileEntries;
    }
}