using static CyArchiveTool.Support.Enumerators;

namespace CyArchiveTool.Support
{
    internal class SharedFunctions
    {
        public static void ErrorExit(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void CheckIfFileFolderExists(string fileFolder, CheckType checkType)
        {
            switch (checkType)
            {
                case CheckType.file:
                    if (!File.Exists(fileFolder))
                    {
                        ErrorExit("Specified file does not exist");
                    }
                    break;

                case CheckType.folder:
                    if (!Directory.Exists(fileFolder))
                    {
                        ErrorExit("Specified directory does not exist");
                    }
                    break;
            }
        }

        public static void IfFileExistsDel(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}