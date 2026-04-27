using CyArchiveTool.Support;

namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackPaths
    {
        public static void UnpackPaths(string packFile)
        {
            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);

            SharedFunctions.CheckIfFileFolderExists(packFile, true);

            Console.WriteLine("Loading pack file....");
            Console.WriteLine("");

            var zpacLoadData = ZPACFileLoader.LoadPackFile(packFile);

            var hashEntryTable = zpacLoadData.HashEntryTable;
            var fileEntryTable = zpacLoadData.FileEntryTable;

            var packFilePathsFile = Path.Combine(packFileDir, $"{packFileName}_paths.txt");

            using (var packFilePathsWriter = new StreamWriter(packFilePathsFile, true, System.Text.Encoding.UTF8))
            {
                for (int i = 0; i < fileEntryTable.FileCount; i++)
                {
                    var currentFileEntry = fileEntryTable.FileEntries[i];
                    var currentPathHash = ZPACFileLoader.GetPathHashByFileIndex(hashEntryTable.HashEntries, i);
                    var vPath = ZPACFileLoader.GetDecryptedPath(currentFileEntry.EncFilePath, currentPathHash);

                    packFilePathsWriter.WriteLine(vPath);
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished unpacking paths from '{Path.GetFileName(packFile)}' file");
        }
    }
}