namespace CyArchiveTool.Support.Structures
{
    internal class ZPACLoadData
    {
        public ZPACHeader? ZPACHeader;
        public HashEntryTableHeader? HashEntryTableHeader;
        public HashEntry[]? HashEntryTable;
        public FileEntryTableHeader? FileEntryTableHeader;
        public FileEntryTable[]? FileEntryTable;
        public long DataStartOffset;
    }
}