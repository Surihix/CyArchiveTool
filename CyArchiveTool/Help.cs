namespace CyArchiveTool
{
    internal class Help
    {
        public static void ShowAppCommands()
        {
            Console.WriteLine("");
            Console.WriteLine("App Commands:");
            Console.WriteLine("-u = Unpack a .pack file");
            Console.WriteLine("-uwp = Unpack a .pack file with paths, if a valid paths.txt file is available next to the pack file");
            Console.WriteLine("-r = Repack a .pack file with an unpacked folder");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("CyArchiveTool.exe -u" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -uwp" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -r" + @" ""001.pack"" " + @"""001""");
            Console.WriteLine("");
            Console.WriteLine("Valid paths.txt file example: '001_paths.txt'");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}