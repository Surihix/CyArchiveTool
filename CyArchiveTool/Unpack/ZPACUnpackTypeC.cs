using System.Text;

namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackTypeC
    {
        public static void UnpackDirectory(string packFile, string virtualDirectory, string pathSeparatorChar)
        {
            virtualDirectory = virtualDirectory.Replace("*", "");
            virtualDirectory = virtualDirectory.Replace("/", pathSeparatorChar);

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
                    vPath = vPath.Replace("/", pathSeparatorChar);

                    var filePathDirData = vPath.Split(pathSeparatorChar);
                    var assembledDir = new StringBuilder();
                    var assembledDirFixed = string.Empty;

                    foreach (var dir in filePathDirData)
                    {
                        assembledDir.Append(dir);
                        assembledDir.Append(pathSeparatorChar);
                        assembledDirFixed = assembledDir.ToString();

                        if (assembledDirFixed == virtualDirectory)
                        {
                            break;
                        }
                    }

                    if (assembledDirFixed == virtualDirectory)
                    {
                        ZPACUnpackHelpers.DataUnpack(unpackDir, vPath, ref duplicateCounter, packFileReader, currentFileEntry);
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
    }
}