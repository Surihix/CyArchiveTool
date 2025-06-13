namespace CyArchiveTool
{
    internal class Help
    {
        public static void ShowAppCommands()
        {
            Console.WriteLine("");
            Console.WriteLine("App Commands:");
            Console.WriteLine("-u = Unpack a .pack file");
            Console.WriteLine("-r = Repack a .pack file");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("CyArchiveTool.exe -u" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -r" + @" ""001"" " + @"""001.pack""");
            Console.WriteLine("");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}