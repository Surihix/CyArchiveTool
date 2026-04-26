using System.Text;

namespace CyArchiveTool.Support
{
    internal class SharedFunctions
    {
        public static Encoding? ShiftJISEncoding = CodePagesEncodingProvider.Instance.GetEncoding(932);

        public static void ErrorExit(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void CheckIfFileFolderExists(string fileFolder, bool isFile)
        {
            if (isFile)
            {
                if (!File.Exists(fileFolder))
                {
                    ErrorExit($"Error: Specified '{Path.GetFileName(fileFolder)}' file does not exist");
                }
            }

            if (!isFile)
            {
                if (!Directory.Exists(fileFolder))
                {
                    ErrorExit($"Error: Specified '{Path.GetFileName(fileFolder)}' directory does not exist");
                }
            }
        }

        public static void IfFileExistsDel(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static string UTF8toShiftJIS(string utf8StringVal)
        {
            var convertedPathData = Encoding.Convert(Encoding.UTF8, ShiftJISEncoding, Encoding.UTF8.GetBytes(utf8StringVal));
            return ShiftJISEncoding.GetString(convertedPathData);
        }
    }
}