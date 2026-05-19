using CyArchiveTool.Support;

namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackTypeC
    {
        public static void UnpackDirectory(string packFile, string virtualDirectory, string pathSeparatorChar)
        {
            virtualDirectory = virtualDirectory.Replace("*", "");
            virtualDirectory = virtualDirectory.Replace("/", pathSeparatorChar);
            virtualDirectory = virtualDirectory.Replace("\\", pathSeparatorChar);

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

            Console.WriteLine("Unpacking....");

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
                    vPath = vPath.Replace("/", pathSeparatorChar);

                    var isMatchingDirectory = SharedFunctions.MatchDirectory(vPath, pathSeparatorChar, virtualDirectory);

                    if (isMatchingDirectory)
                    {
                        ZPACUnpackHelpers.DataUnpack(unpackDir, vPath, packFileReader, currentFileEntry);
                        hasExtracted = true;

                        Console.WriteLine($"Unpacked {Path.Combine(packFileName, $"{vPath}")}");
                    }
                }
            }

            Console.WriteLine("");

            if (hasExtracted)
            {
                Console.WriteLine($"Finished unpacking specificed directory from '{Path.GetFileName(packFile)}' file");
            }
            else
            {
                Console.WriteLine("Specified directory does not exist. please specify a valid directory.");
            }
        }
    }
}