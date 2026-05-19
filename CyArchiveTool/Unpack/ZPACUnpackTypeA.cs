using CyArchiveTool.Support;

namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackTypeA
    {
        public static void UnpackFull(string packFile, string pathSeparatorChar)
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

            Console.WriteLine("Writing HashEntryTable to csv file....");
            Console.WriteLine("");

            var hashEntryTableCsvFile = Path.Combine(unpackDir, "#hash-entry-table.csv");
            SharedFunctions.IfFileExistsDel(hashEntryTableCsvFile);

            using (var hashTableWriter = new StreamWriter(hashEntryTableCsvFile, true))
            {
                hashTableWriter.WriteLine("PathHash,Flag,FileIndex");

                foreach (var entry in hashEntryTable.HashEntries)
                {
                    hashTableWriter.WriteLine($"{entry.StrCode32Hash},{entry.UnkFlag},{entry.FileIndex}");
                }
            }

            var filePaths = new string[fileEntryTable.FileCount];

            using (var packFileReader = new BinaryReader(new FileStream(packFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset;

                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentFileEntry = fileEntryTable.FileEntries[i];

                    _ = packFileReader.BaseStream.Position = zpacLoadData.DataStartOffset + currentFileEntry.DataOffset;

                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);

                    var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                    filePaths[i] = vPath;

                    vPath = vPath.Replace("/", pathSeparatorChar);

                    ZPACUnpackHelpers.DataUnpack(unpackDir, vPath, packFileReader, currentFileEntry);
                    Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Writing paths to csv file....");
            Console.WriteLine("");

            var pathTableCsvFile = Path.Combine(unpackDir, "#path-table.csv");
            SharedFunctions.IfFileExistsDel(pathTableCsvFile);

            using (var pathsWriter = new StreamWriter(pathTableCsvFile, true))
            {
                pathsWriter.WriteLine("FileIndex,VirtualPath");

                for (int i = 0; i < filePaths.Length; i++)
                {
                    pathsWriter.WriteLine($"{i},{filePaths[i]}");
                }
            }

            Console.WriteLine($"Finished unpacking '{Path.GetFileName(packFile)}' file");
        }
    }
}