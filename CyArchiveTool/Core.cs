using CyArchiveTool.Support;

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
                Console.WriteLine($"[CyArchiveTool v1.0.0.6]");
                Console.WriteLine("");

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
                        ZPACUnpack.UnpackPackFile(args[1]);
                        break;

                    case ToolActionSwitches.uwp:
                        ZPACUnpack.UnpackPackFile(args[1], true);
                        break;

                    case ToolActionSwitches.r:
                        if (args.Length < 3)
                        {
                            SharedFunctions.ErrorExit("Warning: Enough arguments not specified for this action. Please use -? or -h switches for more information!");
                        }
                        ZPACRepack.RepackPackFile(args[1], args[2]);
                        break;
                }
            }
            catch (Exception ex)
            {
                SharedFunctions.IfFileExistsDel("CrashLog.txt");

                using (StreamWriter logWriter = new("CrashLog.txt"))
                {
                    logWriter.WriteLine(ex);
                }
                SharedFunctions.ErrorExit("" + ex);
            }
        }

        enum ToolActionSwitches
        {
            u,
            uwp,
            r,
            rwp
        }
    }
}