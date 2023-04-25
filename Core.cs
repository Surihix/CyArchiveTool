using CyArchiveTool;


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


try
{
    if (args.Length < 2)
    {
        CmnMethods.ErrorExit("Enough arguments not specified");
    }

    var specifiedAction = args[0].Replace("-", "");
    var inFileFolder = args[1];

    var toolAction = ToolActionSwitches.d;
    if (Enum.TryParse(specifiedAction, false, out ToolActionSwitches convertedToolAction))
    {
        toolAction = convertedToolAction;
    }
    else
    {
        CmnMethods.ErrorExit("Specified tool action was invalid. has to be either -c or -d");
    }

    switch (toolAction)
    {
        case ToolActionSwitches.c:
            if (args.Length < 3)
            {
                CmnMethods.ErrorExit("pack file to repack is not specified in the argument. specify this file along with the extracted folder");
            }
            var inPackFile = args[2];
            CmnMethods.FileFolderExistsCheck(inFileFolder, CmnMethods.ExistsCheckType.folder);
            CmnMethods.FileFolderExistsCheck(inPackFile, CmnMethods.ExistsCheckType.file);
            ValidityCheck(inPackFile);
            CyRepack.CompressArchive(inFileFolder, inPackFile);
            break;

        case ToolActionSwitches.d:
            CmnMethods.FileFolderExistsCheck(inFileFolder, CmnMethods.ExistsCheckType.file);
            CyUnpack.DecompressArchive(inFileFolder);
            ValidityCheck(inFileFolder);
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


enum ToolActionSwitches
{
    c,
    d
}