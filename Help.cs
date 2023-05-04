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
            Console.WriteLine("CyArchiveTool.exe -u" + @" ""fileName.pack""");
            Console.WriteLine("CyArchiveTool.exe -r" + @" ""extractedFolderName"" " + @"""fileName.pack""");
            Console.WriteLine("");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}