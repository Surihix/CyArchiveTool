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
            Console.WriteLine("-r = Repack an unpacked folder to a pack file. files are packed uncompressed.");
            Console.WriteLine("-rc = Repack an unpacked folder to a pack file. files are packed compressed.");
            Console.WriteLine("-raf = Repack a single file into a pack file");
            Console.WriteLine("-rad = Repacks specific directory along with sub directories, into a pack file");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("CyArchiveTool.exe -u" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -uaf" + @" ""001.pack""" + @" ""cygames\ui\0_005_TITLE\009_vrTitlte\vrTitle_light.win.dds""");
            Console.WriteLine("CyArchiveTool.exe -uad" + @" ""001.pack""" + @" ""cygames\ui\0_005_TITLE\*""");
            Console.WriteLine("CyArchiveTool.exe -ut" + @" ""001.pack""");
            Console.WriteLine("CyArchiveTool.exe -r" + @" ""001""");
            Console.WriteLine("CyArchiveTool.exe -rc" + @" ""001""");
            Console.WriteLine("CyArchiveTool.exe -raf" + @" ""001.pack""" + @" ""001""" + @" ""cygames\ui\0_005_TITLE\009_vrTitlte\vrTitle_light.win.dds""");
            Console.WriteLine("CyArchiveTool.exe -rad" + @" ""001.pack""" + @" ""001""" + @" ""cygames\ui\0_005_TITLE\*""");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}