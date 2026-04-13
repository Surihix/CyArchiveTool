namespace CyArchiveTool
{
    internal class Help
    {
        public static void ShowAppCommands()
        {
            Console.WriteLine("");
            Console.WriteLine("App Commands:");
            Console.WriteLine("-u = Unpack a .pack file");
            Console.WriteLine("-uwp = Unpack a .pack file with paths along with a valid paths.txt file");
            Console.WriteLine("-r = Repack a .pack file with an unpacked folder");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("CyArchiveTool.exe -u" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -uwp" + @" ""001.pack"" ""001_paths.txt""");
            Console.WriteLine("CyArchiveTool.exe -r" + @" ""001.pack"" " + @"""001""");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}