using CyArchiveTool.Support;
using static CyArchiveTool.Support.Structures;

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

            string[] filePaths = new string[] { };

            if (usePaths)
            {
                filePaths = File.ReadAllLines(packPathsTxtFile);
            }

            var packPathsMappingFile = Path.Combine(unpackDir, "##path_mappings.txt");

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var packHeader = new PackHeader();
            var hashTableHeader = new HashTableHeader();
            var hashTable = new HashTableEntry[] { };
            var fileTableHeader = new FileTableHeader();
            var fileTable = new FileTableEntry[] { };
            long fileDataPos = 0;

            ZPACFileLoader.LoadPackFile(packFile, packHeader, hashTableHeader, ref hashTable, fileTableHeader, ref fileTable, ref fileDataPos);

            if (usePaths && filePaths.Length != fileTableHeader.FileCount)
            {
                SharedFunctions.ErrorExit($"'{packFileName}_paths.txt' file doesn't contain all the filepaths. please use the normal -u function!");
            }

            Console.WriteLine("");

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _ = packFileReader.BaseStream.Position = fileDataPos;

                using (var packFilePathsWriter = new StreamWriter(packPathsMappingFile, true))
                {
                    for (int i = 0; i < fileTableHeader.FileCount; i++)
                    {
                        _ = packFileReader.BaseStream.Position = fileDataPos + fileTable[i].DataOffset;

                        var cmpData = packFileReader.ReadBytes(fileTable[i].CmpSize);
                        var dcmpData = ZPACHelpers.UncompressLZ4Data(cmpData, fileTable[i].UncmpSize);

                        var vPath = $"FILE_{i}";
                        var fileExtn = ZPACHelpers.GetExtension(dcmpData);

                        if (usePaths)
                        {
                            vPath = filePaths[i];

                            if (!vPath.StartsWith("FILE_"))
                            {
                                var isValid = ZPACFileLoader.ValidatePathFromHashTable(vPath, i);

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

                        Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}");
                        Console.WriteLine($"Compressed size: {fileTable[i].CmpSize}");
                        Console.WriteLine($"Uncompressed size: {fileTable[i].UncmpSize}");
                        Console.WriteLine("");
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished extracting '{Path.GetFileName(packFile)}' file");
        }
    }
}