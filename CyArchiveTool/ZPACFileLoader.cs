using CyArchiveTool.Support;
using System.Text;
using static CyArchiveTool.Support.Structures;

namespace CyArchiveTool
{
    internal class ZPACFileLoader
    {
        private static HashTableEntry[]? ZPACHashTable { get; set; }
        public static void LoadPackFile(string packFile, PackHeader packHeader, HashTableHeader hashTableHeader, ref HashTableEntry[] hashTable, FileTableHeader fileTableHeader, ref FileTableEntry[] fileTable, ref long fileDataPos)
        {
            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                packHeader.Magic = Encoding.ASCII.GetString(packFileReader.ReadBytes(4));

                if (packHeader.Magic != "ZPAC")
                {
                    SharedFunctions.ErrorExit("This is not a valid ZOE2 MARS .pack archive file");
                }

                packHeader.UnkVal = packFileReader.ReadUInt32();
                packHeader.HashTableOffset = packFileReader.ReadUInt32();
                packHeader.FileTableOffset = packFileReader.ReadUInt32();

                _ = packFileReader.BaseStream.Position = packHeader.HashTableOffset;
                LoadHashTable(hashTableHeader, ref hashTable, packFileReader);
                Console.WriteLine($"HashTableEntry Count: {hashTableHeader.HashTableEntryCount}");

                _ = packFileReader.BaseStream.Position = packHeader.FileTableOffset;
                LoadFileTable(fileTableHeader, ref fileTable, packFileReader, ref fileDataPos);
                Console.WriteLine($"File Count: {fileTableHeader.FileCount}");
            }
        }

        private static void LoadHashTable(HashTableHeader hashTableHeader, ref HashTableEntry[] hashTable, BinaryReader packFileReader)
        {
            hashTableHeader.HashTableEntryCount = packFileReader.ReadUInt32();
            hashTableHeader.Reserved = packFileReader.ReadBytes(12);
            hashTable = new HashTableEntry[hashTableHeader.HashTableEntryCount];
            ZPACHashTable = new HashTableEntry[hashTableHeader.HashTableEntryCount];

            for (int i = 0; i < hashTableHeader.HashTableEntryCount; i++)
            {
                var currentHashTableEntry = new HashTableEntry()
                {
                    StrCode32Hash = packFileReader.ReadUInt32(),
                    UnkFlag = packFileReader.ReadByte(),
                    FileIndex = packFileReader.ReadUInt16(),
                    Reserved = packFileReader.ReadByte()
                };

                hashTable[i] = currentHashTableEntry;
                ZPACHashTable[i] = currentHashTableEntry;
            }
        }

        private static void LoadFileTable(FileTableHeader fileTableHeader, ref FileTableEntry[] fileTable, BinaryReader packFileReader, ref long fileDataPos)
        {
            fileTableHeader.FileCount = packFileReader.ReadUInt32();
            fileTableHeader.Reserved = packFileReader.ReadBytes(12);
            fileTable = new FileTableEntry[fileTableHeader.FileCount];

            for (int i = 0; i < fileTableHeader.FileCount; i++)
            {
                var fileTableEntry = new FileTableEntry()
                {
                    CmpSize = packFileReader.ReadInt32(),
                    UnkVal = packFileReader.ReadUInt32(),
                    UncmpSize = packFileReader.ReadInt32(),
                    DataOffset = packFileReader.ReadUInt32(),
                    UnkVal2 = packFileReader.ReadUInt32(),
                    UnkHashOrEncFilePath = packFileReader.ReadBytes(224),
                    Reserved = packFileReader.ReadBytes(12)
                };

                fileTable[i] = fileTableEntry;
            }

            fileDataPos = packFileReader.BaseStream.Position;
        }

        public static bool ValidatePathFromHashTable(string vPath, int fileIndex)
        {
            var isValid = false;

            if (ZPACHashTable != null && ZPACHashTable.Length == 0)
            {
                SharedFunctions.ErrorExit("Path validation failed as hash table was not loaded properly!");
            }

            for (int i = 0; i < ZPACHashTable.Length; i++)
            {
                if (ZPACHashTable[i].StrCode32Hash != 0 && ZPACHashTable[i].FileIndex == fileIndex)
                {
                    if (StrCode32(vPath) == ZPACHashTable[i].StrCode32Hash)
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
            var stringBytes = Encoding.ASCII.GetBytes(stringVal);
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
    }
}