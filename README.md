# CyArchiveTool

This tool allows you to unpack and repack the .pack archive files that are present inside the cygames folder in the PC version of Zone of Enders 2 MARS. with this tool, you can extract all of the files that are present inside the .pack file as well as repack the same extracted .pack file.

Use the following commands with this tool:
<br>For unpacking a .pack file: ```CyArchiveTool -d "fileName.pack" ```
<br>For repacking a .pack file: ```CyArchiveTool -c "extractedFolderName" "oldPackfileName.pack" ```

<br>**Note:** On repacking a file, you would have to provide the old .pack file along with the extracted folder name. the extracted folder should contain all the files and even if one file is missing, this tool will not repack the file. after the repacking succeeds, you will get a new pack file with the .new extension. rename the file to .pack and use that with the game.
<br><br>
**Limitations:** Do note that this file format is not fully analysed and just with the limited amount of info that this tool works with, there were no issues seen in the game when it was launched with a .pack file that was repacked with this tool. one big limitation is that this tool cannot extract the files with proper filenames and folders as they are either hashed or encrypted somewhere. you would have to manually check each extracted file from the .pack file to locate a file that you are interested in. 

## For Developers:
The following package was used for lz4 compression and decompression:
<br>**K4os.Compression.LZ4** - https://www.nuget.org/packages/K4os.Compression.LZ4
<br><br>Refer to the format structure of the .pack file from [here](FormatStruct.md).
