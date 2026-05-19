using CyArchiveTool.Support;
using System.Text;

namespace CyArchiveTool.Repack
{
    internal class ZPACRepackTypeB
    {
        public static void RepackSingle(string packFile, string unpackedDir, string virtualFilePath, string pathSeparatorChar)
        {
            virtualFilePath = virtualFilePath.Replace("/", pathSeparatorChar);
            virtualFilePath = virtualFilePath.Replace("\\", pathSeparatorChar);

            var packFileName = Path.GetFileName(unpackedDir);

            SharedFunctions.CheckIfFileFolderExists(packFile, true);
            SharedFunctions.CheckIfFileFolderExists(unpackedDir, false);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var zpacHeader = zpacLoadData.ZPACHeader;
            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            Console.WriteLine("Creating data stream....");
            Console.WriteLine("");

            var packDataFile = packFile + "_data";
            SharedFunctions.IfFileExistsDel(packDataFile);

            using (var packFileStream = new FileStream(packFile, FileMode.Open, FileAccess.Read))
            {
                using (var packFileDataStream = new FileStream(packDataFile, FileMode.Append, FileAccess.Write))
                {
                    _ = packFileStream.Seek(zpacLoadData.DataStartOffset, SeekOrigin.Begin);
                    packFileStream.CopyTo(packFileDataStream);
                }
            }

            Console.WriteLine("Creating header data....");
            Console.WriteLine("");

            var headerData = new byte[16];
            using (var headerWriter = new BinaryWriter(new MemoryStream(headerData)))
            {
                headerWriter.Write(Encoding.ASCII.GetBytes(zpacHeader.Magic));
                headerWriter.WriteBytesUInt32(zpacHeader.Version, false);
                headerWriter.WriteBytesUInt32(zpacHeader.HashTableOffset, false);
                headerWriter.WriteBytesUInt32(zpacHeader.FileTableOffset, false);
            }

            Console.WriteLine("Creating hash entry table data....");
            Console.WriteLine("");

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

            Console.WriteLine("Repacking....");

            var hasRepacked = false;

            for (int i = 0; i < fileEntryTable.FileCount; i++)
            {
                var currentFileEntry = fileEntryTable.FileEntries[i];
                var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);

                var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                vPath = vPath.Replace("/", pathSeparatorChar);

                if (vPath == virtualFilePath)
                {
                    using (var fileDataCleanStream = new FileStream(packDataFile, FileMode.Open, FileAccess.Write))
                    {
                        fileDataCleanStream.Seek(currentFileEntry.DataOffset, SeekOrigin.Begin);

                        var oldFileCleanedData = new byte[currentFileEntry.CmpSize];
                        fileDataCleanStream.Write(oldFileCleanedData, 0, currentFileEntry.CmpSize);
                    }

                    using (var fileDataStream = new FileStream(packDataFile, FileMode.Append, FileAccess.Write))
                    {
                        var isNullData = false;
                        ZPACRepackHelpers.DataRepack(unpackedDir, vPath, currentFileEntry, fileDataStream, ref isNullData);

                        if (!isNullData)
                        {
                            Console.WriteLine($"Repacked {Path.Combine(packFileName, vPath)}");
                        }
                    }

                    hasRepacked = true;
                    break;
                }
            }

            if (hasRepacked)
            {
                Console.WriteLine("");
                Console.WriteLine("Building entry table....");

                var fileEntryTableData = new byte[(int)(fileEntryTable.FileCount * 256) + 16];
                using (var fileEntryTableWriter = new BinaryWriter(new MemoryStream(fileEntryTableData)))
                {
                    fileEntryTableWriter.WriteBytesUInt32(fileEntryTable.FileCount, false);
                    fileEntryTableWriter.Write(hashEntryTable.Reserved);

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
            else
            {
                SharedFunctions.IfFileExistsDel(packDataFile);

                Console.WriteLine("");
                Console.WriteLine("Specified file does not exist. please specify a valid file path.");
            }
        }
    }
}