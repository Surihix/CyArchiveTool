using CyArchiveTool.Support;
using System.Text;
using static CyArchiveTool.Support.Structures;

namespace CyArchiveTool
{
    internal class ZPACRepack
    {
        public static void RepackPackFile(string packFile, string unpackedDir)
        {
            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);

            SharedFunctions.CheckIfFileFolderExists(unpackedDir, Enumerators.CheckType.folder);

            var packPathsMappingFile = Path.Combine(unpackedDir, "##path_mappings.txt");

            if (!File.Exists(packPathsMappingFile))
            {
                SharedFunctions.ErrorExit($"Unable to locate '##path_mappings.txt' file!");
            }

            var filePaths = File.ReadAllLines(packPathsMappingFile);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var packHeader = new PackHeader();
            var hashTableHeader = new HashTableHeader();
            var hashTable = new HashTableEntry[] { };
            var fileTableHeader = new FileTableHeader();
            var fileTable = new FileTableEntry[] { };
            long fileDataPos = 0;

            ZPACFileLoader.LoadPackFile(packFile, packHeader, hashTableHeader, ref hashTable, fileTableHeader, ref fileTable, ref fileDataPos);

            var newPackFile = packFile + ".new";
            SharedFunctions.IfFileExistsDel(newPackFile);

            var oldPackFile = packFile + ".old";
            SharedFunctions.IfFileExistsDel(oldPackFile);

            var packDataFile = packFile + "_data";
            SharedFunctions.IfFileExistsDel(packDataFile);

            if (filePaths.Length != fileTableHeader.FileCount)
            {
                SharedFunctions.ErrorExit($"'{packFileName}_paths.txt' file doesn't contain all the filepaths!");
            }

            Console.WriteLine("");

            var headerData = new byte[16];
            using (var headerWriter = new BinaryWriter(new MemoryStream(headerData)))
            {
                headerWriter.Write(Encoding.ASCII.GetBytes(packHeader.Magic));
                headerWriter.Write(BitConverter.GetBytes(packHeader.UnkVal));
                headerWriter.Write(BitConverter.GetBytes(packHeader.HashTableOffset));
                headerWriter.Write(BitConverter.GetBytes(packHeader.FileTableOffset));
            }

            var hashTableData = new byte[(int)(hashTableHeader.HashTableEntryCount * 8) + 16];
            using (var hashTableWriter = new BinaryWriter(new MemoryStream(hashTableData)))
            {
                hashTableWriter.Write(BitConverter.GetBytes(hashTableHeader.HashTableEntryCount));
                hashTableWriter.Write(hashTableHeader.Reserved);

                for (int i = 0; i < hashTableHeader.HashTableEntryCount; i++)
                {
                    hashTableWriter.Write(BitConverter.GetBytes(hashTable[i].StrCode32Hash));
                    hashTableWriter.Write(hashTable[i].UnkFlag);
                    hashTableWriter.Write(BitConverter.GetBytes(hashTable[i].FileIndex));
                    hashTableWriter.Write(hashTable[i].Reserved);
                }
            }

            using (var newPackWriter = new FileStream(newPackFile, FileMode.Append, FileAccess.Write))
            {
                newPackWriter.Write(headerData);
                newPackWriter.Write(hashTableData);
                newPackWriter.Write(BitConverter.GetBytes(fileTableHeader.FileCount));
                newPackWriter.Write(fileTableHeader.Reserved);

                using (var fileDataStream = new BinaryWriter(new FileStream(packDataFile, FileMode.Append, FileAccess.Write)))
                {
                    var splitChar = new string[] { " >> " };

                    for (int i = 0; i < fileTableHeader.FileCount; i++)
                    {
                        var currentFileTableEntry = fileTable[i];

                        var vPathData = filePaths[i].Split(splitChar, StringSplitOptions.None);

                        if (vPathData.Length < 2)
                        {
                            SharedFunctions.ErrorExit($"Unable to find mapped file path for 'FILE_{i}'");
                        }

                        var outFile = Path.Combine(unpackedDir, vPathData[1]);
                        byte[] dataToCmp;
                        byte[] outData;

                        if (File.Exists(outFile))
                        {
                            dataToCmp = File.ReadAllBytes(outFile);
                            outData = ZPACHelpers.CompressLZ4Data(dataToCmp, dataToCmp.Length);

                            currentFileTableEntry.CmpSize = outData.Length;
                            currentFileTableEntry.UncmpSize = dataToCmp.Length;
                            currentFileTableEntry.DataOffset = (uint)fileDataStream.BaseStream.Position;
                            fileDataStream.Write(outData);

                            Console.WriteLine($"Repacked {Path.Combine(packFileName, $"{vPathData[1]}")}");
                            Console.WriteLine($"Compressed size: {fileTable[i].CmpSize}");
                            Console.WriteLine($"Uncompressed size: {fileTable[i].UncmpSize}");
                        }
                        else
                        {
                            outData = new byte[16];

                            currentFileTableEntry.CmpSize = 16;
                            currentFileTableEntry.UncmpSize = 16;
                            currentFileTableEntry.DataOffset = (uint)fileDataStream.BaseStream.Position;
                            fileDataStream.Write(outData);

                            Console.WriteLine($"Unable to locate file. added null data!");
                        }

                        newPackWriter.Write(BitConverter.GetBytes(currentFileTableEntry.CmpSize));
                        newPackWriter.Write(BitConverter.GetBytes(currentFileTableEntry.UnkVal));
                        newPackWriter.Write(BitConverter.GetBytes(currentFileTableEntry.UncmpSize));
                        newPackWriter.Write(BitConverter.GetBytes(currentFileTableEntry.DataOffset));
                        newPackWriter.Write(BitConverter.GetBytes(currentFileTableEntry.UnkVal2));
                        newPackWriter.Write(currentFileTableEntry.UnkHashOrEncFilePath);
                        newPackWriter.Write(currentFileTableEntry.Reserved);

                        Console.WriteLine("");
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Building finalized pack file....");
            Console.WriteLine("");

            using (var finalPackStream = new FileStream(newPackFile, FileMode.Append, FileAccess.Write))
            {
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