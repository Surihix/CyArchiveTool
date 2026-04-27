namespace CyArchiveTool.Unpack
{
    internal class ZPACUnpackTypeB
    {
        public static void UnpackSingle(string packFile, string virtualFilePath, string pathSeparatorChar)
        {
            virtualFilePath = virtualFilePath.Replace("\\", pathSeparatorChar);

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
                    vPath = vPath.Replace("/", Core.PathSeparatorChar);

                    if (vPath == virtualFilePath)
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
    }
}