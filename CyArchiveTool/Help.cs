namespace CyArchiveTool
{
    internal class Help
    {
        public static void ShowAppCommands()
        {
            Console.WriteLine("");
            Console.WriteLine("App Commands:");
            Console.WriteLine("-u = Unpack a .pack file");
            Console.WriteLine("-uaf = Unpack a single file from a pack file");
            Console.WriteLine("-uad = Unpacks a specific directory along with sub directories, from a pack file");
            Console.WriteLine("-r = Repack a .pack file with an unpacked folder");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("CyArchiveTool.exe -u" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -uaf" + @" ""001.pack""" + @" ""cygames\ui\0_005_TITLE\009_vrTitlte\vrTitle_light.win.dds""");
            Console.WriteLine("CyArchiveTool.exe -uad" + @" ""001.pack""" + @" ""cygames\ui\0_005_TITLE\*""");
            Console.WriteLine("CyArchiveTool.exe -r" + @" ""001.pack"" " + @"""001""");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}