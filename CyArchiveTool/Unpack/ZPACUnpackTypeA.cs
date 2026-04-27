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
                    vPath = vPath.Replace("/", pathSeparatorChar);

                    ZPACUnpackHelpers.DataUnpack(unpackDir, vPath, ref duplicateCounter, packFileReader, currentFileEntry);
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
    }
}