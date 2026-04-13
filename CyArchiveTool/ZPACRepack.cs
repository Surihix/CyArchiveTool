using CyArchiveTool.Support;
using System.Text;

namespace CyArchiveTool
{
    internal class ZPACRepack
    {
        public static void RepackPackFile(string packFile, string unpackedDir)
        {
            var packFileName = Path.GetFileNameWithoutExtension(packFile);

            SharedFunctions.CheckIfFileFolderExists(packFile, Enumerators.CheckType.file);
            SharedFunctions.CheckIfFileFolderExists(unpackedDir, Enumerators.CheckType.folder);

            var packPathsMappingFile = Path.Combine(unpackedDir, "##path_mappings.txt");

            if (!File.Exists(packPathsMappingFile))
            {
                SharedFunctions.ErrorExit($"Unable to locate '##path_mappings.txt' file inside unpacked folder!");
            }

            var filePaths = File.ReadAllLines(packPathsMappingFile);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var zpacHeader = zpacLoadData.ZPACHeader;
            var hashEntryTableHeader = zpacLoadData.HashEntryTableHeader;
            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTableHeader = zpacLoadData.FileEntryTableHeader;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            var newPackFile = packFile + ".new";
            SharedFunctions.IfFileExistsDel(newPackFile);

            var oldPackFile = packFile + ".old";
            SharedFunctions.IfFileExistsDel(oldPackFile);

            var packDataFile = packFile + "_data";
            SharedFunctions.IfFileExistsDel(packDataFile);

            if (filePaths.Length != fileEntryTableHeader.FileCount)
            {
                SharedFunctions.ErrorExit($"'{packFileName}_paths.txt' file doesn't contain all the filepaths!");
            }

            var headerData = new byte[16];
            using (var headerWriter = new BinaryWriter(new MemoryStream(headerData)))
            {
                headerWriter.Write(Encoding.ASCII.GetBytes(zpacHeader.Magic));
                headerWriter.Write(BitConverter.GetBytes(zpacHeader.Version));
                headerWriter.Write(BitConverter.GetBytes(zpacHeader.HashTableOffset));
                headerWriter.Write(BitConverter.GetBytes(zpacHeader.FileTableOffset));
            }

            var hashEntryTableData = new byte[(int)(hashEntryTableHeader.HashEntryCount * 8) + 16];
            using (var hashEntryTableWriter = new BinaryWriter(new MemoryStream(hashEntryTableData)))
            {
                hashEntryTableWriter.Write(BitConverter.GetBytes(hashEntryTableHeader.HashEntryCount));
                hashEntryTableWriter.Write(hashEntryTableHeader.Reserved);

                for (int i = 0; i < hashEntryTableHeader.HashEntryCount; i++)
                {
                    hashEntryTableWriter.Write(BitConverter.GetBytes(hashEntryTable[i].StrCode32Hash));
                    hashEntryTableWriter.Write(hashEntryTable[i].UnkFlag);
                    hashEntryTableWriter.Write(BitConverter.GetBytes(hashEntryTable[i].FileIndex));
                    hashEntryTableWriter.Write(hashEntryTable[i].Reserved);
                }
            }

            using (var newPackWriter = new FileStream(newPackFile, FileMode.Append, FileAccess.Write))
            {
                newPackWriter.Write(headerData);
                newPackWriter.Write(hashEntryTableData);
                newPackWriter.Write(BitConverter.GetBytes(fileEntryTableHeader.FileCount));
                newPackWriter.Write(fileEntryTableHeader.Reserved);

                using (var fileDataStream = new BinaryWriter(new FileStream(packDataFile, FileMode.Append, FileAccess.Write)))
                {
                    var splitChar = new string[] { " >> " };

                    for (int i = 0; i < fileEntryTableHeader.FileCount; i++)
                    {
                        var currentFileTableEntry = fileEntryTable[i];

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

                            if (dataToCmp.Length == 0)
                            {
                                outData = Array.Empty<byte>();
                            }
                            else
                            {
                                outData = ZPACHelpers.CompressLZ4Data(dataToCmp, dataToCmp.Length);
                            }

                            currentFileTableEntry.CmpSize = outData.Length;
                            currentFileTableEntry.UncmpSize = dataToCmp.Length;
                            currentFileTableEntry.DataOffset = (uint)fileDataStream.BaseStream.Position;
                            fileDataStream.Write(outData);

                            var padAmount = ZPACHelpers.ComputePadding(fileDataStream.BaseStream.Position, 16);

                            if (padAmount != 0)
                            {
                                var paddingData = new byte[padAmount];
                                fileDataStream.Write(paddingData);
                            }

                            Console.WriteLine($"Repacked {Path.Combine(packFileName, $"{vPathData[1]}")}");
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

            Console.WriteLine($"Finished repacking files to '{Path.GetFileName(packFile)}' file");
        }
    }
}