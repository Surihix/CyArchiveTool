# CyArchiveTool

This small program allows you to unpack and repack the .pack archive files that are present inside the cygames folder in the PC version of Zone of Enders 2 MARS. the program should be launched from command prompt with a few argument switches to perform a function. the list of valid argument switches are given below:

<br>**Tool actions:**
<br>``-u`` Unpack all files stored in a pac file
<br>``-r`` Repack a folder containing valid extracted files into a pac file
<br>``-?`` or ``-h`` Display the help page
<br>

## Important notes
- To display the help page, run the program with either the `-?` or `-h` switches. for example `CyArchiveTool -?`

- One big limitation with this tool is that it cannot unpack the files with proper filenames and folders, due to them being either hashed or encrypted somewhere. so each unpacked file will be named in a numerical order, with the word `FILE_` prefixed before the number in the filename.

- When repacking a file, you would have to provide the old .pack file along with the extracted folder name. the extracted folder should contain all the files and even if one file is missing, this tool will not repack the pack file. if the repacking succeeds, then you will get a new pack file with the .new extension. rename the file to .pack and use that with the game.

- Do note that this file format is not fully analysed and just with the limited amount of info that this tool works with, there were no issues seen in the game when it was launched with a .pack file that was repacked with this tool. 

## For Developers:
The following package was used for lz4 compression and decompression:
<br>**K4os.Compression.LZ4** - https://www.nuget.org/packages/K4os.Compression.LZ4
<br><br>Refer to the format structure of the .pack file from [here](FormatStruct.md).
