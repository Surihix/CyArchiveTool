namespace CyArchiveTool
{
    internal class Core
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("");
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                // Parse args
                if (args.Length == 1)
                {
                    if (args[0].Contains("-h") || args[0].Contains("-?"))
                    {
                        Help.ShowAppCommands();
                    }
                }

                if (args.Length < 2)
                {
                    Console.WriteLine("Enough arguments not specified");
                    Help.ShowAppCommands();
                }

                if (Enum.TryParse(args[0].Replace("-", ""), false, out ToolActionSwitches toolActionSwitch) == false)
                {
                    Console.WriteLine("Warning: Specified tool action was invalid!");
                    Help.ShowAppCommands();
                }

                switch (toolActionSwitch)
                {
                    case ToolActionSwitches.u:
                        CmnMethods.FileFolderExistsCheck(args[1], CmnMethods.ExistsCheckType.file);
                        ValidityCheck(args[1]);
                        CyUnpack.DecompressArchive(args[1]);
                        break;

                    case ToolActionSwitches.r:
                        if (args.Length < 3)
                        {
                            CmnMethods.ErrorExit("Warning: Enough arguments not specified for this action. Please use -? or -h switches for more information!");
                        }
                        CmnMethods.FileFolderExistsCheck(args[1], CmnMethods.ExistsCheckType.folder);
                        CmnMethods.FileFolderExistsCheck(args[2], CmnMethods.ExistsCheckType.file);
                        ValidityCheck(args[2]);
                        CyRepack.CompressArchive(args[1], args[2]);
                        break;
                }
            }
            catch (Exception ex)
            {
                CmnMethods.FileExistsDel("CrashLog.txt");

                using (StreamWriter logWriter = new("CrashLog.txt"))
                {
                    logWriter.WriteLine(ex);
                }
                CmnMethods.ErrorExit("" + ex);
            }
        }

        enum ToolActionSwitches
        {
            u,
            r,
        }


        static void ValidityCheck(string filePathVar)
        {
            using (FileStream checkFile = new(filePathVar, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader checkFileReader = new(checkFile))
                {
                    checkFileReader.BaseStream.Position = 0;
                    var zpacStrChars = checkFileReader.ReadChars(4);
                    var zpacStr = string.Join("", zpacStrChars).Replace("\0", "");

                    if (!zpacStr.Contains("ZPAC"))
                    {
                        CmnMethods.ErrorExit("This is not a valid ZOE2 MARS .pack archive file");
                    }
                }
            }
        }
    }
}