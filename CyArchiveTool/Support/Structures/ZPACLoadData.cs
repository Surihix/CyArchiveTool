namespace CyArchiveTool.Support.Structures
{
    internal class ZPACLoadData
    {
        public ZPACHeader? ZPACHeader;
        public HashEntryTableHeader? HashEntryTableHeader;
        public HashEntry[]? HashEntryTable;
        public FileEntryTableHeader? FileEntryTableHeader;
        public FileEntry[]? FileEntryTable;
        public long DataStartOffset;
    }
}