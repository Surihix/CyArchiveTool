using CyArchiveTool.Support;

namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackPaths
    {
        public static void UnpackPackTables(string packFile)
        {
            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);

            SharedFunctions.CheckIfFileFolderExists(packFile, true);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            Console.WriteLine("Writing HashEntryTable to csv file....");
            Console.WriteLine("");

            var hashEntryTableCsvFile = Path.Combine(packFileDir, $"{packFileName}_hash-entry-table.csv");
            SharedFunctions.IfFileExistsDel(hashEntryTableCsvFile);

            using (var hashTableWriter = new StreamWriter(hashEntryTableCsvFile, true))
            {
                hashTableWriter.WriteLine("PathHash,Flag,FileIndex");

                foreach (var entry in hashEntryTable.HashEntries)
                {
                    hashTableWriter.WriteLine($"{entry.StrCode32Hash},{entry.UnkFlag},{entry.FileIndex}");
                }
            }

            Console.WriteLine("Reading paths....");
            Console.WriteLine("");

            var filePaths = new string[fileEntryTable.FileCount];

            for (int i = 0; i < fileEntryTable.FileCount; i++)
            {
                var currentFileEntry = fileEntryTable.FileEntries[i];
                var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);
                var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);
                filePaths[i] = vPath;
            }

            Console.WriteLine("Writing paths to csv file....");
            Console.WriteLine("");

            var pathTableCsvFile = Path.Combine(packFileDir, $"{packFileName}_path-table.csv");
            SharedFunctions.IfFileExistsDel(pathTableCsvFile);

            using (var pathsWriter = new StreamWriter(pathTableCsvFile, true))
            {
                pathsWriter.WriteLine("FileIndex,VirtualPath");

                for (int i = 0; i < filePaths.Length; i++)
                {
                    pathsWriter.WriteLine($"{i},{filePaths[i]}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished unpacking paths from '{Path.GetFileName(packFile)}' file");
        }
    }
}