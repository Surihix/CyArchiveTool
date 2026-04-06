using CyArchiveTool.Support;
using static CyArchiveTool.Support.Structures;

namespace CyArchiveTool
{
    internal class ZPACRepack
    {
        public static void RepackPackFile(string packFile, string unpackedDir, bool usePaths = false)
        {
            var packFileDir = Path.GetDirectoryName(packFile);
            var packFileName = Path.GetFileNameWithoutExtension(packFile);

            SharedFunctions.CheckIfFileFolderExists(unpackedDir, Enumerators.CheckType.folder);

            var packPathsMappingFile = Path.Combine(unpackedDir, "##path_mappings.txt");

            if (usePaths && !File.Exists(packPathsMappingFile))
            {
                SharedFunctions.ErrorExit($"Unable to locate '##path_mappings.txt' file. please use the normal -u function!");
            }

            string[] filePaths = new string[] { };

            if (usePaths)
            {
                filePaths = File.ReadAllLines(packPathsMappingFile);
            }

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

            if (usePaths && filePaths.Length != fileTableHeader.FileCount)
            {
                SharedFunctions.ErrorExit($"'{packFileName}_paths.txt' file doesn't contain all the filepaths. please use the normal -u function!");
            }

            Console.WriteLine("");



            Console.WriteLine("");
            Console.WriteLine($"Finished repacking files to '{Path.GetFileName(packFile)}' file");
        }
    }
}