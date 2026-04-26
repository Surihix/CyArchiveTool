namespace CyArchiveTool.Support.Structures
{
    internal class ZPACLoadData
    {
        public ZPACHeader? ZPACHeader;
        public HashEntryTable? HashEntryTable;
        public FileEntryTable? FileEntryTable;
        public long DataStartOffset;
    }
}