using CyArchiveTool.Support;
using System.Text;

namespace CyArchiveTool
{
    internal class ZPACRepack
    {
        private static readonly string PathSeparatorChar = Path.DirectorySeparatorChar.ToString();

        public static void RepackFull(string packFile, string unpackedDir)
        {
            var packFileName = Path.GetFileNameWithoutExtension(packFile);

            SharedFunctions.CheckIfFileFolderExists(packFile, true);
            SharedFunctions.CheckIfFileFolderExists(unpackedDir, false);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var zpacHeader = zpacLoadData.ZPACHeader;
            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            var newPackFile = packFile + ".new";
            SharedFunctions.IfFileExistsDel(newPackFile);

            var oldPackFile = packFile + ".old";
            SharedFunctions.IfFileExistsDel(oldPackFile);

            var packDataFile = packFile + "_data";
            SharedFunctions.IfFileExistsDel(packDataFile);

            var headerData = new byte[16];
            using (var headerWriter = new BinaryWriter(new MemoryStream(headerData)))
            {
                headerWriter.Write(Encoding.ASCII.GetBytes(zpacHeader.Magic));
                headerWriter.WriteBytesUInt32(zpacHeader.Version, false);
                headerWriter.WriteBytesUInt32(zpacHeader.HashTableOffset, false);
                headerWriter.WriteBytesUInt32(zpacHeader.FileTableOffset, false);
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

            using (var fileDataStream = new BinaryWriter(new FileStream(packDataFile, FileMode.Append, FileAccess.Write)))
            {
                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentFileEntry = fileEntryTable.FileEntries[i];

                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);

                    var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                    vPath = vPath.Replace("/", PathSeparatorChar);

                    var outFile = Path.Combine(unpackedDir, vPath);

                    var fileData = Array.Empty<byte>();
                    var dataToPack = Array.Empty<byte>();
                    var isNullData = false;

                    if (File.Exists(outFile))
                    {
                        fileData = File.ReadAllBytes(outFile);
                    }

                    if (!File.Exists(outFile))
                    {
                        isNullData = true;
                        fileData = new byte[16];
                    }

                    if (currentFileEntry.CmpLevel == 0 || isNullData)
                    {
                        dataToPack = fileData;
                    }

                    if (currentFileEntry.CmpLevel != 0 && !isNullData)
                    {
                        dataToPack = LZ4Functions.CompressLZ4Data(fileData, fileData.Length, currentFileEntry.CmpLevel);
                    }

                    currentFileEntry.CmpSize = dataToPack.Length;
                    currentFileEntry.UncmpSize = fileData.Length;
                    currentFileEntry.DataOffset = (uint)fileDataStream.BaseStream.Position;
                    fileDataStream.Write(dataToPack);

                    fileEntryTable.FileEntries[i] = currentFileEntry;

                    var padAmount = ZPACHelpers.ComputePadding(fileDataStream.BaseStream.Position, 16);

                    if (padAmount != 0)
                    {
                        fileDataStream.Write(new byte[padAmount]);
                    }

                    if (isNullData)
                    {
                        Console.WriteLine($"Unable to locate file. added null data!");
                    }

                    if (!isNullData)
                    {
                        Console.WriteLine($"Repacked {Path.Combine(packFileName, vPath)}");
                    }                    
                }
            }

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
                    fileEntryTableWriter.WriteBytesUInt32(entry.UnkVal, false);
                    fileEntryTableWriter.WriteBytesInt32(entry.UncmpSize, false);
                    fileEntryTableWriter.WriteBytesUInt32(entry.DataOffset, false);
                    fileEntryTableWriter.WriteBytesUInt32(entry.CmpLevel, false);
                    fileEntryTableWriter.Write(entry.EncFilePath);
                    fileEntryTableWriter.Write(entry.Reserved);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Building finalized pack file....");

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