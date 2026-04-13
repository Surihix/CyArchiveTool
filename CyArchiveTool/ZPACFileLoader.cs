using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;
using System.Text;

namespace CyArchiveTool
{
    internal class ZPACFileLoader
    {
        public static ZPACLoadData LoadPackFile(string packFile)
        {
            var zpacLoadData = new ZPACLoadData();

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var zpacHeader = new ZPACHeader
                {
                    Magic = Encoding.ASCII.GetString(packFileReader.ReadBytes(4))
                };

                if (zpacHeader.Magic != "ZPAC")
                {
                    SharedFunctions.ErrorExit("This is not a valid ZOE2 MARS .pack archive file");
                }

                zpacHeader.Version = packFileReader.ReadUInt32();
                zpacHeader.HashTableOffset = packFileReader.ReadUInt32();
                zpacHeader.FileTableOffset = packFileReader.ReadUInt32();

                _ = packFileReader.BaseStream.Position = zpacHeader.HashTableOffset;
                var hashEntryTableHeader = new HashEntryTableHeader
                {
                    HashEntryCount = packFileReader.ReadUInt32(),
                    Reserved = packFileReader.ReadBytes(12)
                };

                var hashEntryTable = LoadHashEntryTable(hashEntryTableHeader, packFileReader);
                Console.WriteLine($"HashTableEntry Count: {hashEntryTableHeader.HashEntryCount}");

                _ = packFileReader.BaseStream.Position = zpacHeader.FileTableOffset;
                var fileEntryTableHeader = new FileEntryTableHeader()
                {
                    FileCount = packFileReader.ReadUInt32(),
                    Reserved = packFileReader.ReadBytes(12)
                };
                var fileEntryTable = LoadFileEntryTable(fileEntryTableHeader, packFileReader);
                Console.WriteLine($"File Count: {fileEntryTableHeader.FileCount}");

                zpacLoadData.ZPACHeader = zpacHeader;
                zpacLoadData.HashEntryTableHeader = hashEntryTableHeader;
                zpacLoadData.HashEntryTable = hashEntryTable;
                zpacLoadData.FileEntryTableHeader = fileEntryTableHeader;
                zpacLoadData.FileEntryTable = fileEntryTable;
                zpacLoadData.DataStartOffset = packFileReader.BaseStream.Position;
            }

            return zpacLoadData;
        }

        private static HashEntry[] LoadHashEntryTable(HashEntryTableHeader hashTableHeader, BinaryReader packFileReader)
        {
            var hashEntryTable = new HashEntry[hashTableHeader.HashEntryCount];

            for (int i = 0; i < hashTableHeader.HashEntryCount; i++)
            {
                var currentHashTableEntry = new HashEntry()
                {
                    StrCode32Hash = packFileReader.ReadUInt32(),
                    UnkFlag = packFileReader.ReadByte(),
                    FileIndex = packFileReader.ReadUInt16(),
                    Reserved = packFileReader.ReadByte()
                };

                hashEntryTable[i] = currentHashTableEntry;
            }

            return hashEntryTable;
        }

        private static FileEntry[] LoadFileEntryTable(FileEntryTableHeader fileTableHeader, BinaryReader packFileReader)
        {
            var fileEntryTable = new FileEntry[fileTableHeader.FileCount];

            for (int i = 0; i < fileTableHeader.FileCount; i++)
            {
                var fileEntry = new FileEntry()
                {
                    CmpSize = packFileReader.ReadInt32(),
                    UnkVal = packFileReader.ReadUInt32(),
                    UncmpSize = packFileReader.ReadInt32(),
                    DataOffset = packFileReader.ReadUInt32(),
                    UnkVal2 = packFileReader.ReadUInt32(),
                    UnkHashOrEncFilePath = packFileReader.ReadBytes(224),
                    Reserved = packFileReader.ReadBytes(12)
                };

                fileEntryTable[i] = fileEntry;
            }

            return fileEntryTable;
        }

        public static bool ValidatePath(string vPath, int fileIndex, HashEntry[]? hashEntryTable, uint hashEntryCount)
        {
            var isValid = false;

            for (int i = 0; i < hashEntryCount; i++)
            {
                if (hashEntryTable[i].StrCode32Hash != 0 && hashEntryTable[i].FileIndex == fileIndex)
                {
                    if (StrCode32(vPath) == hashEntryTable[i].StrCode32Hash)
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