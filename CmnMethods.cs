namespace CyArchiveTool
{
    internal class CmnMethods
    {
        public static void ErrorExit(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void FileFolderExistsCheck(string inFileOrFolderVar, ExistsCheckType existsCheckTypeVar)
        {
            switch (existsCheckTypeVar)
            {
                case ExistsCheckType.file:
                    if (!File.Exists(inFileOrFolderVar))
                    {
                        ErrorExit("Specified file does not exist");
                    }
                    break;

                case ExistsCheckType.folder:
                    if (!Directory.Exists(inFileOrFolderVar))
                    {
                        ErrorExit("Specified directory does not exist");
                    }
                    break;
            }
        }

        public enum ExistsCheckType
        {
            file,
            folder
        }

        public static void FileExistsDel(string filePathVar)
        {
            if (File.Exists(filePathVar))
            {
                File.Delete(filePathVar);
            }
        }

        public static void ReadBytesUInt32(BinaryReader readerNameVar, uint readerPos, out uint readVal)
        {
            readerNameVar.BaseStream.Position = readerPos;
            readVal = readerNameVar.ReadUInt32();
        }
    }
}