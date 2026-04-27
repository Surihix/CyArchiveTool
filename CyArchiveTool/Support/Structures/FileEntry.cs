namespace CyArchiveTool.Support.Structures
{
    internal class FileEntry
    {
        public int CmpSize;
        public uint UnkVal;
        public int UncmpSize;
        public uint DataOffset;
        public uint CmpLevel;
        public byte[]? EncFilePath;
        public byte[]? Reserved;
    }
}