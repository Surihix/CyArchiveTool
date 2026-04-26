using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;
using System.Text;

namespace CyArchiveTool
{
    internal class ZPACUnpack
    {
        private static readonly string PathSeparatorChar = Path.DirectorySeparatorChar.ToString();
        public static void UnpackFull(string packFile)
        {
            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);
            var unpackDir = Path.Combine(packFileDir, packFileName);

            SharedFunctions.CheckIfFileFolderExists(packFile, true);

            if (Directory.Exists(unpackDir))
            {
                Console.WriteLine("Detected previous unpack. deleting....");
                Console.WriteLine("");

                Directory.Delete(unpackDir, true);
            }

            Directory.CreateDirectory(unpackDir);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            int duplicateCounter = 0;

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset;

                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentFileEntry = fileEntryTable.FileEntries[i];

                    _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset + currentFileEntry.DataOffset;

                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);

                    var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                    vPath = vPath.Replace("/", PathSeparatorChar);

                    DataUnpack(unpackDir, vPath, ref duplicateCounter, packFileReader, currentFileEntry);
                    Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished unpacking '{Path.GetFileName(packFile)}' file");

            if (duplicateCounter > 1)
            {
                Console.WriteLine($"{duplicateCounter} duplicate file(s)");
            }
        }


        public static void UnpackSingle(string packFile, string virtualFilePath)
        {
            virtualFilePath = virtualFilePath.Replace("\\", PathSeparatorChar);

            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);
            var unpackDir = Path.Combine(packFileDir, packFileName);

            if (!Directory.Exists(unpackDir))
            {
                Directory.CreateDirectory(unpackDir);
            }

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            int duplicateCounter = 0;
            bool hasExtracted = false;

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset;

                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentFileEntry = fileEntryTable.FileEntries[i];

                    _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset + currentFileEntry.DataOffset;

                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);

                    var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                    vPath = vPath.Replace("/", PathSeparatorChar);

                    if (vPath == virtualFilePath)
                    {
                        DataUnpack(unpackDir, vPath, ref duplicateCounter, packFileReader, currentFileEntry);
                        hasExtracted = true;

                        Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}");
                    }
                }
            }

            Console.WriteLine("");

            if (hasExtracted)
            {
                Console.WriteLine($"Finished unpacking specificed file from '{Path.GetFileName(packFile)}' file");

                if (duplicateCounter > 1)
                {
                    Console.WriteLine($"{duplicateCounter} duplicate file(s)");
                }
            }
            else
            {
                Console.WriteLine("Specified file does not exist. please specify a valid file path.");
            }
        }


        public static void UnpackDirectory(string packFile, string virtualDirectory)
        {
            virtualDirectory = virtualDirectory.Replace("*", "");
            virtualDirectory = virtualDirectory.Replace("/", PathSeparatorChar);

            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);
            var unpackDir = Path.Combine(packFileDir, packFileName);

            if (!Directory.Exists(unpackDir))
            {
                Directory.CreateDirectory(unpackDir);
            }

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            int duplicateCounter = 0;
            bool hasExtracted = false;

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset;

                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentFileEntry = fileEntryTable.FileEntries[i];

                    _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset + currentFileEntry.DataOffset;

                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);

                    var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                    vPath = vPath.Replace("/", PathSeparatorChar);

                    var filePathDirData = vPath.Split(PathSeparatorChar);
                    var assembledDir = new StringBuilder();
                    var assembledDirFixed = string.Empty;

                    foreach (var dir in filePathDirData)
                    {
                        assembledDir.Append(dir);
                        assembledDir.Append(PathSeparatorChar);
                        assembledDirFixed = assembledDir.ToString();

                        if (assembledDirFixed == virtualDirectory)
                        {
                            break;
                        }
                    }

                    if (assembledDirFixed == virtualDirectory)
                    {
                        DataUnpack(unpackDir, vPath, ref duplicateCounter, packFileReader, currentFileEntry);
                        hasExtracted = true;

                        Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}");
                    }
                }
            }

            Console.WriteLine("");

            if (hasExtracted)
            {
                Console.WriteLine($"Finished unpacking specificed directory from '{Path.GetFileName(packFile)}' file");

                if (duplicateCounter > 1)
                {
                    Console.WriteLine($"{duplicateCounter} duplicate file(s)");
                }
            }
            else
            {
                Console.WriteLine("Specified directory does not exist. please specify a valid directory.");
            }
        }


        private static void DataUnpack(string unpackDir, string vPath, ref int duplicateCounter, BinaryReader packFileReader, FileEntry fileEntry)
        {
            var outFile = Path.Combine(unpackDir, vPath);
            var outFileDir = Path.GetDirectoryName(outFile);

            if (!Directory.Exists(outFileDir))
            {
                Directory.CreateDirectory(outFileDir);
            }

            if (File.Exists(outFile))
            {
                File.Delete(outFile);
                duplicateCounter++;
            }

            var fileDataInPack = packFileReader.ReadBytes(fileEntry.CmpSize);

            if (fileEntry.CmpFlag == 1 && fileEntry.CmpSize != fileEntry.UncmpSize)
            {
                var dcmpData = LZ4Functions.UncompressLZ4Data(fileDataInPack, fileEntry.UncmpSize);
                File.WriteAllBytes(outFile, dcmpData);
            }
            else
            {
                File.WriteAllBytes(outFile, fileDataInPack);
            }
        }
    }
}