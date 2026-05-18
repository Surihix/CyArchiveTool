using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;
using System.Text;

namespace CyArchiveTool.Repack
{
    internal class ZPACRepackTypeA
    {
        public static void RepackFull(string unpackedDir, bool shouldCompress)
        {
            var packFileName = $"{Path.GetFileName(unpackedDir)}";
            var packFile = Path.Combine(Path.GetDirectoryName(unpackedDir), $"{packFileName}.pack");

            SharedFunctions.CheckIfFileFolderExists(unpackedDir, false);

            var hashEntryTableCsvFile = Path.Combine(unpackedDir, $"#hash-entry-table.csv");
            if (!File.Exists(hashEntryTableCsvFile))
            {
                SharedFunctions.ErrorExit($"Error: Missing '{Path.GetFileName(hashEntryTableCsvFile)}' file in unpacked directory!");
            }

            var pathTableCsvFile = Path.Combine(unpackedDir, $"#path-table.csv");
            if (!File.Exists(pathTableCsvFile))
            {
                SharedFunctions.ErrorExit($"Error: Missing '{Path.GetFileName(pathTableCsvFile)}' file in unpacked directory!");
            }

            var hashEntries = ZPACRepackCsvHelpers.GetHashEntriesFromCSV(hashEntryTableCsvFile);
            var hashEntryTable = new HashEntryTable()
            {
                EntryCount = (uint)hashEntries.Length,
                Reserved = new byte[12],
                HashEntries = hashEntries
            };

            var filePaths = ZPACRepackCsvHelpers.GetFilePathsFromCSV(pathTableCsvFile);
            var fileEntryTable = new FileEntryTable()
            {
                FileCount = (uint)filePaths.Length,
                Reserved = new byte[12]
            };

            var zpacHeader = new ZPACHeader()
            {
                Magic = "ZPAC",
                Version = 1,
                HashTableOffset = 16,
                FileTableOffset = 16 + 16 + (hashEntryTable.EntryCount * 8)
            };

            var headerData = new byte[16];
            using (var headerWriter = new BinaryWriter(new MemoryStream(headerData)))
            {
                headerWriter.Write(Encoding.ASCII.GetBytes(zpacHeader.Magic));
                headerWriter.Write(zpacHeader.Version);
                headerWriter.Write(zpacHeader.HashTableOffset);
                headerWriter.Write(zpacHeader.FileTableOffset);
            }

            var hashEntryTableData = new byte[(int)(hashEntryTable.EntryCount * 8) + 16];
            using (var hashEntryTableWriter = new BinaryWriter(new MemoryStream(hashEntryTableData)))
            {
                hashEntryTableWriter.WriteBytesUInt32(hashEntryTable.EntryCount, false);
                hashEntryTableWriter.Write(hashEntryTable.Reserved);

                for (int i = 0; i < hashEntryTable.EntryCount; i++)
                {
                    hashEntryTableWriter.WriteBytesUInt32(hashEntryTable.HashEntries[i].StrCode32Hash, false);
                    hashEntryTableWriter.Write(hashEntryTable.HashEntries[i].UnkFlag);
                    hashEntryTableWriter.WriteBytesUInt16(hashEntryTable.HashEntries[i].FileIndex, false);
                    hashEntryTableWriter.Write(hashEntryTable.HashEntries[i].Reserved);
                }
            }

            var packDataFile = packFile + "_data";
            SharedFunctions.IfFileExistsDel(packDataFile);

            using (var fileDataWriter = new BinaryWriter(new FileStream(packDataFile, FileMode.Append, FileAccess.Write)))
            {
                var fileEntries = new FileEntry[fileEntryTable.FileCount];

                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntries, i);
                    var vPath = filePaths[i];
                    var vPathData = SharedFunctions.ShiftJISEncoding.GetBytes(vPath + "\0");

                    var currentFileEntry = new FileEntry()
                    {
                        CmpLevel = (uint)(shouldCompress == true ? 1 : 0),
                        EncFilePath = ZPACRepackHelpers.EncryptFilePath(vPathData, currentPathHash),
                        Reserved = new byte[12]
                    };

                    var isNullData = false;
                    vPath = vPath.Replace("/", Core.PathSeparatorChar);
                    ZPACRepackHelpers.DataRepack(unpackedDir, vPath, currentFileEntry, fileDataWriter, ref isNullData);

                    if (isNullData)
                    {
                        Console.WriteLine($"Unable to locate file. added null data!");
                    }

                    if (!isNullData)
                    {
                        Console.WriteLine($"Repacked {Path.Combine(packFileName, vPath)}");
                    }

                    fileEntries[i] = currentFileEntry;
                }

                fileEntryTable.FileEntries = fileEntries;
            }

            Console.WriteLine("");
            Console.WriteLine("Building entry table....");

            var fileEntryTableData = new byte[(int)(fileEntryTable.FileCount * 256) + 16];
            using (var fileEntryTableWriter = new BinaryWriter(new MemoryStream(fileEntryTableData)))
            {
                fileEntryTableWriter.WriteBytesUInt32(fileEntryTable.FileCount, false);
                fileEntryTableWriter.Write(fileEntryTable.Reserved);

                foreach (var entry in fileEntryTable.FileEntries)
                {
                    fileEntryTableWriter.WriteBytesInt32(entry.CmpSize, false);
                    fileEntryTableWriter.WriteBytesUInt32(entry.PaddingSize, false);
                    fileEntryTableWriter.WriteBytesInt32(entry.UncmpSize, false);
                    fileEntryTableWriter.WriteBytesUInt32(entry.DataOffset, false);
                    fileEntryTableWriter.WriteBytesUInt32(entry.CmpLevel, false);
                    fileEntryTableWriter.Write(entry.EncFilePath);
                    fileEntryTableWriter.Write(entry.Reserved);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Building finalized pack file....");

            var newPackFile = packFile + ".new";
            SharedFunctions.IfFileExistsDel(newPackFile);

            var oldPackFile = packFile + ".old";
            SharedFunctions.IfFileExistsDel(oldPackFile);

            using (var finalPackStream = new FileStream(newPackFile, FileMode.Append, FileAccess.Write))
            {
                finalPackStream.Write(headerData);
                finalPackStream.Write(hashEntryTableData);
                finalPackStream.Write(fileEntryTableData);

                using (var dataPackStream = new FileStream(packDataFile, FileMode.Open, FileAccess.Read))
                {
                    dataPackStream.CopyTo(finalPackStream);
                }
            }

            SharedFunctions.IfFileExistsDel(packDataFile);
            File.Move(packFile, oldPackFile);
            File.Move(newPackFile, packFile);

            Console.WriteLine("");
            Console.WriteLine($"Finished repacking files to '{Path.GetFileName(packFile)}' file");
        }
    }
}