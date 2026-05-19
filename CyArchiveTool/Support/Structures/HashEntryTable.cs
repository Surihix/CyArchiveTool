namespace CyArchiveTool.Support.Structures
{
    internal class HashEntryTable
    {
        public uint EntryCount;
        public byte[]? Reserved;
        public HashEntry[]? HashEntries;
    }
}