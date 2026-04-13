using CyArchiveTool.Support;

namespace CyArchiveTool
{
    internal class ZPACUnpack
    {
        public static void UnpackPackFile(string packFile, bool usePaths = false)
        {
            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);
            var unpackDir = Path.Combine(packFileDir, packFileName);

            SharedFunctions.CheckIfFileFolderExists(packFile, Enumerators.CheckType.file);

            if (Directory.Exists(unpackDir))
            {
                Console.WriteLine("Detected previous unpack. deleting....");
                Console.WriteLine("");

                Directory.Delete(unpackDir, true);
            }

            Directory.CreateDirectory(unpackDir);

            var packPathsTxtFile = Path.Combine(packFileDir, $"{packFileName}_paths.txt");

            if (usePaths && !File.Exists(packPathsTxtFile))
            {
                SharedFunctions.ErrorExit($"Unable to locate '{packFileName}_paths.txt' file. please use the normal -u function!");
            }

            string[] filePaths = Array.Empty<string>();

            if (usePaths)
            {
                filePaths = File.ReadAllLines(packPathsTxtFile);
            }

            var packPathsMappingFile = Path.Combine(unpackDir, "##path_mappings.txt");

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var hashEntryTableHeader = zpacLoadData.HashEntryTableHeader;
            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTableHeader = zpacLoadData.FileEntryTableHeader;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            if (usePaths && filePaths.Length != fileEntryTableHeader.FileCount)
            {
                SharedFunctions.ErrorExit($"'{packFileName}_paths.txt' file doesn't contain all the filepaths. please use the normal -u function!");
            }

            Console.WriteLine("");

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset;

                using (var packFilePathsWriter = new StreamWriter(packPathsMappingFile, true))
                {
                    for (int i = 0; i < fileEntryTableHeader.FileCount; i++)
                    {
                        _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset + fileEntryTable[i].DataOffset;

                        byte[] dcmpData;

                        if (fileEntryTable[i].CmpSize != 0)
                        {
                            var cmpData = packFileReader.ReadBytes(fileEntryTable[i].CmpSize);
                            dcmpData = ZPACHelpers.UncompressLZ4Data(cmpData, fileEntryTable[i].UncmpSize);
                        }
                        else
                        {
                            dcmpData = Array.Empty<byte>();
                        }

                        var vPath = $"FILE_{i}";
                        var fileExtn = ZPACHelpers.GetExtension(dcmpData);

                        if (usePaths)
                        {
                            vPath = filePaths[i];

                            if (!vPath.StartsWith("FILE_"))
                            {
                                var isValid = ZPACFileLoader.ValidatePath(vPath, i, hashEntryTable, hashEntryTableHeader.HashEntryCount);

                                if (isValid)
                                {
                                    vPath = filePaths[i].Replace("/", "\\");
                                }
                                else
                                {
                                    vPath = $"FILE_{i}";
                                }
                            }
                        }

                        if (vPath.StartsWith("FILE_"))
                        {
                            vPath += fileExtn;
                        }

                        packFilePathsWriter.WriteLine($"FILE_{i} >> {vPath}");

                        var outFile = Path.Combine(unpackDir, vPath);
                        var outFileDir = Path.GetDirectoryName(outFile);

                        if (!Directory.Exists(outFileDir))
                        {
                            Directory.CreateDirectory(outFileDir);
                        }

                        File.WriteAllBytes(outFile, dcmpData);

                        Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}   [Compressed size: {fileEntryTable[i].CmpSize}]  [Uncompressed size: {fileEntryTable[i].UncmpSize}]");
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished extracting '{Path.GetFileName(packFile)}' file");
        }
    }
}