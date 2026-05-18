using CyArchiveTool.Repack;
using CyArchiveTool.Support;
using CyArchiveTool.Unpack;

namespace CyArchiveTool
{
    internal class Core
    {
        public static readonly string PathSeparatorChar = Path.DirectorySeparatorChar.ToString();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("");
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine($"[CyArchiveTool v1.0.0.8]");
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

                var pathSeparatorChar = Path.DirectorySeparatorChar.ToString();

                if (SharedFunctions.ShiftJISEncoding == null)
                {
                    SharedFunctions.ShiftJISEncoding = System.Text.Encoding.UTF8;
                }

                switch (toolActionSwitch)
                {
                    case ToolActionSwitches.u:
                        CheckArgsLength(args, 2);

                        ZPACUnpackTypeA.UnpackFull(args[1], pathSeparatorChar);
                        break;

                    case ToolActionSwitches.uaf:
                        CheckArgsLength(args, 3);

                        ZPACUnpackTypeB.UnpackSingle(args[1], args[2], pathSeparatorChar);
                        break;

                    case ToolActionSwitches.uad:
                        CheckArgsLength(args, 3);

                        ZPACUnpackTypeC.UnpackDirectory(args[1], args[2], pathSeparatorChar);
                        break;

                    case ToolActionSwitches.ut:
                        CheckArgsLength(args, 2);

                        ZPACUnpackPaths.UnpackPackTables(args[1]);
                        break;

                    case ToolActionSwitches.r:
                        CheckArgsLength(args, 2);

                        ZPACRepackTypeA.RepackFull(args[1], true);
                        break;

                    case ToolActionSwitches.rc:
                        CheckArgsLength(args, 2);

                        ZPACRepackTypeA.RepackFull(args[1], false);
                        break;

                    case ToolActionSwitches.raf:
                        CheckArgsLength(args, 4);

                        Console.WriteLine("unimplemented");
                        //ZPACRepackTypeB.RepackSingle(args[1], args[2], args[3]);
                        break;

                    case ToolActionSwitches.rad:
                        CheckArgsLength(args, 4);

                        Console.WriteLine("unimplemented");
                        //ZPACRepackTypeC.RepackMultiple(args[1], args[2], args[3]);
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

        private static void CheckArgsLength(string[] args, int requiredLength)
        {
            if (args.Length < requiredLength)
            {
                SharedFunctions.ErrorExit("Warning: Enough arguments not specified for this action. Please use -? or -h switches for more information!");
            }
        }

        enum ToolActionSwitches
        {
            u,
            uaf,
            uad,
            ut,
            r,
            rc,
            raf,
            rad
        }
    }
}