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
                var hashEntryTable = LoadHashEntryTable(packFileReader);
                if (hashEntryTable.HashEntries == null || hashEntryTable.HashEntries.Length == 0)
                {
                    SharedFunctions.ErrorExit("Error: Failed to load Hash entry table!");
                }

                Console.WriteLine($"HashTableEntry Count: {hashEntryTable.EntryCount}");

                _ = packFileReader.BaseStream.Position = zpacHeader.FileTableOffset;
                var fileEntryTable = LoadFileEntryTable(packFileReader);
                if (fileEntryTable.FileEntries == null || fileEntryTable.FileEntries.Length == 0)
                {
                    SharedFunctions.ErrorExit("Error: Failed to load File entry table!");
                }

                Console.WriteLine($"File Count: {fileEntryTable.FileCount}");
                Console.WriteLine("");

                zpacLoadData.ZPACHeader = zpacHeader;
                zpacLoadData.HashEntryTable = hashEntryTable;
                zpacLoadData.FileEntryTable = fileEntryTable;
                zpacLoadData.DataStartOffset = packFileReader.BaseStream.Position;
            }

            return zpacLoadData;
        }

        private static HashEntryTable LoadHashEntryTable(BinaryReader packFileReader)
        {
            var hashEntryTable = new HashEntryTable()
            {
                EntryCount = packFileReader.ReadUInt32(),
                Reserved = packFileReader.ReadBytes(12)
            };

            var hashEntries = new HashEntry[hashEntryTable.EntryCount];

            for (int i = 0; i < hashEntryTable.EntryCount; i++)
            {
                var currentHashEntry = new HashEntry()
                {
                    StrCode32Hash = packFileReader.ReadUInt32(),
                    UnkFlag = packFileReader.ReadByte(),
                    FileIndex = packFileReader.ReadUInt16(),
                    Reserved = packFileReader.ReadByte()
                };

                hashEntries[i] = currentHashEntry;
            }

            hashEntryTable.HashEntries = hashEntries;

            return hashEntryTable;
        }

        private static FileEntryTable LoadFileEntryTable(BinaryReader packFileReader)
        {
            var fileEntryTable = new FileEntryTable()
            {
                FileCount = packFileReader.ReadUInt32(),
                Reserved = packFileReader.ReadBytes(12)
            };

            var fileEntries = new FileEntry[fileEntryTable.FileCount];

            for (int i = 0; i < fileEntryTable.FileCount; i++)
            {
                var currentFileEntry = new FileEntry()
                {
                    CmpSize = packFileReader.ReadInt32(),
                    UnkVal = packFileReader.ReadUInt32(),
                    UncmpSize = packFileReader.ReadInt32(),
                    DataOffset = packFileReader.ReadUInt32(),
                    CmpFlag = packFileReader.ReadUInt32(),
                    EncFilePath = packFileReader.ReadBytes(224),
                    Reserved = packFileReader.ReadBytes(12)
                };

                fileEntries[i] = currentFileEntry;
            }

            fileEntryTable.FileEntries = fileEntries;

            return fileEntryTable;
        }

        public static uint GetPathHashByFileIndex(HashEntry[] hashEntryTable, int fileIndex)
        {
            var pathHash = uint.MinValue;

            for (int i = 0; i < hashEntryTable.Length; i++)
            {
                if (hashEntryTable[i].StrCode32Hash != 0 && hashEntryTable[i].FileIndex == fileIndex)
                {
                    pathHash = hashEntryTable[i].StrCode32Hash;
                }
            }

            return pathHash;
        }

        private static Encoding? PathEncoding = CodePagesEncodingProvider.Instance.GetEncoding(932);
        public static string GetDecryptedPath(byte[] encFilePathData, uint pathHash)
        {
            if (PathEncoding == null)
            {
                PathEncoding = Encoding.UTF8;
            }

            var xorValue = CygamesIVTable.IVs[pathHash & 0x3FF];
            var decFilePathData = new byte[encFilePathData.Length];
            var length = 0;

            for (int j = 0; j < encFilePathData.Length; j++)
            {
                var currentByte = encFilePathData[j];
                decFilePathData[j] = (byte)(xorValue ^ currentByte);

                if (decFilePathData[j] == 0)
                {
                    break;
                }

                xorValue = currentByte;
                length++;
            }

            return PathEncoding.GetString(decFilePathData, 0, length);
        }
    }
}